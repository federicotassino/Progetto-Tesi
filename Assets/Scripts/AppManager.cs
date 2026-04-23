using Microsoft.MixedReality.WorldLocking.Core;
using MixedReality.Toolkit.Examples.Demos;
using MixedReality.Toolkit.SpatialManipulation;
using MixedReality.Toolkit.UX;
using MixedReality.Toolkit.UX.Experimental;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

[System.Serializable]
public struct ArtifactsStruct
{
    public GameObject artifacts;
    public GameObject artifactsPanel;
    public Transform artifactVirualizedList;
    //public Transform artifactContentTransform;
    public Transform artifactScrollView;
    public PressableButton artifactButtonPrefab;
    public GameObject artifactBackButton;
    public GameObject artifactTitle;
    public GameObject artifactText;
    public GameObject searchBar;
    public GameObject startNavigationButton;
    public GameObject stopNavigationButton;
    public GameObject artifactTarget;
    public GameObject solverIndicator;
    public GameObject navigationText;
    public GameObject depositButton;
    public GameObject depositInLastShelfButton;
    public GameObject depositList;
    public GameObject depositInShelfButton;
    public GameObject withdrawButton;
    public AudioSource triggerEntered;
    public GameObject[] artifactDepositedUI;
}

public class AppManager : MonoBehaviour
{
    private List<GameObject> shelvesButtonCreated = new();
    private GameObject lastShelfPositioned;
    private List<GameObject> allShelves = new();
    private GameObject lastToggledPin;
    private List<GameObject> allArtifacts = new();
    private List<GameObject> artifactsOnList = new();
    private GameObject artifactSelected;
    private readonly string artifactGeneralText = "Selezionare il reperto a cui si è interessati oppure effettuare una ricerca tramite la barra";
    private readonly string artifactTitle = "Reperto: ";
    private readonly string artifactShelfYes = "Il reperto si trova nello scaffale: ";
    private readonly string artifactShelfNo = "Il reperto non si trova in nessuno scaffale";
    private readonly string artifactShelfLast = "Riposizionare il reperto nell'utlimo scaffale in cui si trovava";
    private readonly string initialDepositText = "Il reperto non è mai stato depositato nel magazzino. Procedere al primo deposito?";
    private readonly string textDeposit = "Deposita reperto in un nuovo scaffale";
    private readonly string initialDepositButtonText = "Deposita reperto in uno scaffale";
    private readonly string artifactNavigation = "Dirigersi verso: ";
    private readonly string[] targetReached = new string[2] {"Reperto raggiunto!", "Scaffale raggiunto. Procedere al deposito!"};
    private readonly List<Transform> currentPath = new();
    private List<Transform> recentPath = new();
    private int step = 0;
    private readonly string indicatorTag = "Target";
    private readonly List<Transform> depositPath = new();
    private VirtualizedScrollRectListTester vsrlt;
    //private int depositStep = 0;

    [SerializeField] private GameObject homePanel;
    //Shelves panel
    [SerializeField] private GameObject firstText;
    [SerializeField] private GameObject positionButton;
    [SerializeField] private GameObject secondText;
    private readonly string firstTextString = "Scegliere quale elemento posizionare";
    private readonly string firstTextString_2 = "Scegliere se posizionare l'elemento ";
    private readonly string secondTextString = "Oppure navigare tra le sottocategorie di ";
    [SerializeField] private Transform virtualizedList;
    [SerializeField] private GameObject positioningText;
    [SerializeField] private GameObject positioningButton;

    //Warehouse
    [SerializeField] private GameObject warehouse;
    [SerializeField] public GameObject shelvesListPanel;
    [SerializeField] private PressableButton buttonPrefabShelves;
    [SerializeField] private Transform scrollView;
    [SerializeField] private GameObject returnButton;
    [SerializeField] private GameObject positioningSphere;
    [SerializeField] private GameObject sphereIndicator;
    [SerializeField] private GameObject debugCube;

    //Artifacts
    [SerializeField] public ArtifactsStruct A_Menu;
    

    // Start is called before the first frame update
    void Start()
    {
        WorldLockingManager.GetInstance().Load();

        //PlayerPrefs.DeleteKey("Franco (1)");
        //PlayerPrefs.DeleteKey("Franco (2)");


        shelvesListPanel.SetActive(false);
        positioningSphere.SetActive(false);
        sphereIndicator.SetActive(false);

        //settaggio velocità e distanze dei vari pannelli
        float speed = 0.3f;
        float minDistance = 0.6f;
        float maxDistance = 1f;
        float defaultDistance = 0.8f;
        homePanel.GetComponentInChildren<Follow>().MoveLerpTime = speed;
        homePanel.GetComponentInChildren<Follow>().RotateLerpTime = speed;
        homePanel.GetComponentInChildren<Follow>().MinDistance = minDistance;
        homePanel.GetComponentInChildren<Follow>().MaxDistance = maxDistance;
        homePanel.GetComponentInChildren<Follow>().DefaultDistance = defaultDistance;
        A_Menu.artifactsPanel.GetComponentInChildren<Follow>().MoveLerpTime = speed;
        A_Menu.artifactsPanel.GetComponentInChildren<Follow>().RotateLerpTime = speed;
        A_Menu.artifactsPanel.GetComponentInChildren<Follow>().MinDistance = minDistance;
        A_Menu.artifactsPanel.GetComponentInChildren<Follow>().MaxDistance = maxDistance;
        A_Menu.artifactsPanel.GetComponentInChildren<Follow>().DefaultDistance = defaultDistance;
        shelvesListPanel.GetComponentInChildren<Follow>().MoveLerpTime = speed;
        shelvesListPanel.GetComponentInChildren<Follow>().RotateLerpTime = speed;
        shelvesListPanel.GetComponentInChildren<Follow>().MinDistance = minDistance;
        shelvesListPanel.GetComponentInChildren<Follow>().MaxDistance = maxDistance;
        shelvesListPanel.GetComponentInChildren<Follow>().DefaultDistance = defaultDistance;

        A_Menu.artifactsPanel.SetActive(false);
        A_Menu.artifactBackButton.SetActive(false);
        A_Menu.artifactTarget.SetActive(false);
        A_Menu.solverIndicator.SetActive(false);
        A_Menu.navigationText.SetActive(false);
        A_Menu.depositButton.SetActive(false);
        A_Menu.depositInShelfButton.SetActive(false);
        A_Menu.depositList.SetActive(false);
        A_Menu.depositInLastShelfButton.SetActive(false);
        A_Menu.withdrawButton.SetActive(false);
        A_Menu.triggerEntered.Stop();

        foreach (var obj in A_Menu.artifactDepositedUI)
        {
            obj.SetActive(false);
        }

        GetAllShelves(warehouse);

        //per ogni scaffale presente nella lista chiamo la funzione per posizionarlo nell'ultima posizione salvata
        allShelves.ForEach((item) => SetInitialTransform(item));
        
        positioningText.gameObject.SetActive(false);
        positioningButton.transform.parent.gameObject.SetActive(false);

        //Artifacts setup
        GetAllArtifacts(A_Menu.artifacts);

        vsrlt = A_Menu.depositList.GetComponentInChildren<VirtualizedScrollRectListTester>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //posiziona gli scaffali - chiamata nello start e da ResetShelfPosition
    public void SetInitialTransform(GameObject shelf)
    {
        string shelfTransform = PlayerPrefs.GetString(shelf.name);
        if (!string.IsNullOrEmpty(shelfTransform))
        {
            //Debug.Log("Settaggio iniziale scaffale " + shelf.name);

            string[] transform = shelfTransform.Split('/');
            string[] position = transform[0].Split('_');
            string[] rotation = transform[1].Split('_');

            shelf.transform.SetPositionAndRotation(new Vector3(float.Parse(position[0]), float.Parse(position[1]), float.Parse(position[2])), 
                new Quaternion(float.Parse(rotation[0]), float.Parse(rotation[1]), float.Parse(rotation[2]), float.Parse(rotation[3])));

            //reset scala
            //scaffale.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    //chiamata alla Manipulation Ended di uno shelf (non c'è più)
    /*public void ShelfPositioned()
    {
        if (!savePanel.activeSelf)
        {
            foreach (GameObject item in allShelves)
            {
                if (item.gameObject != lastShelfPositioned && item.GetComponent<ObjectManipulator>() != null)
                    item.GetComponent<ObjectManipulator>().enabled = false;
            }

            savePanel.SetActive(true);
        }
    }*/

    public void SaveNewPositions()
    {
        WorldLockingManager.GetInstance().Save();
        Debug.Log("salvataggio mondo");

        //SaveTransformObject(lastShelfPositioned);

        Transform[] allChildren = lastShelfPositioned.GetComponentsInChildren<Transform>();
        foreach (Transform t in allChildren)
        {
            Debug.Log("Saving position of child: " + t.gameObject.name);
            SaveTransformObject(t.gameObject);
        }

        ResetPanelAfterPositioning();
    }

    //salva la nuova posizione dell'ultimo scaffale con cui si è interagito
    public void SaveTransformObject(GameObject objectToSave)
    {
        //WorldLockingManager.GetInstance().Save();
        //Debug.Log("salvataggio mondo");

        objectToSave.transform.GetPositionAndRotation(out var positionTemp, out var rotationTemp);

        string objectPosition = positionTemp.x + "_" + positionTemp.y + "_" + positionTemp.z;
        string objectRotation = rotationTemp.x + "_" + rotationTemp.y + "_" + rotationTemp.z + "_" + rotationTemp.w;
        string objectTransform = objectPosition + "/" + objectRotation;
        PlayerPrefs.SetString(objectToSave.name, objectTransform);
        Debug.Log("Salvataggio " + objectToSave.name + ": " + objectTransform);

        //lastShelfPositioned.GetComponent<BoundsControl>().HandlesActive = false;
        //lastShelfPositioned.GetComponent<ObjectManipulator>().HostTransform = lastShelfPositioned.transform;

        if (objectToSave.TryGetComponent<ParentConstraint>(out var parentConstraint))
            Destroy(parentConstraint);

        //curatorPanel.GetComponentInChildren<Follow>().IgnoreDistanceClamp = false;  DA RIMETTERE

        //ResetPanelAfterPositioning();
    }

    //chiamata dai bottoni StopPositioningButton, BackButton e CloseButton del pannello per annullare il salvataggio della nuova posizione
    public void ResetShelfPosition()
    {
        //if messo per evitare di resettare tutto quando chiamata da BackButton e CloseButton se non si stava posizionando
        if (positioningSphere.activeSelf)
        {
            ResetPanelAfterPositioning();

            SetInitialTransform(lastShelfPositioned);
        } 
    }

    //spegne l'UI del posizionamento e riaccende quella della navigazione della warehouse
    public void ResetPanelAfterPositioning()
    {
        string resetText = positioningText.GetComponent<TextMeshProUGUI>().text;
        string toRemove = " \"" + lastShelfPositioned.name + "\"";
        positioningText.GetComponent<TextMeshProUGUI>().text = resetText.Replace(toRemove, "", System.StringComparison.OrdinalIgnoreCase);
        positioningText.gameObject.SetActive(false);
        positioningButton.transform.parent.gameObject.SetActive(false);
        firstText.gameObject.SetActive(true);
        positionButton.transform.parent.gameObject.SetActive(true);
        secondText.gameObject.SetActive(true);
        if (lastShelfPositioned.transform.childCount > 0)
            virtualizedList.gameObject.SetActive(true);

        Destroy(lastShelfPositioned.GetComponent<ParentConstraint>());
        positioningSphere.SetActive(false);
        sphereIndicator.SetActive(false);
    }

    //aggiunge alla lista allShelves tutti gli elementi che compongono la warehouse
    public void GetAllShelves(GameObject wh)
    {
        Transform[] allChildren = wh.GetComponentsInChildren<Transform>();

        foreach (Transform t in allChildren)
        {
            if (t != wh.transform)
            {
                //Debug.Log(t.name);
                t.AddComponent<StorageContainer>();
                allShelves.Add(t.gameObject);
                
                if (t.childCount == 0)
                {
                    if (t.gameObject.TryGetComponent<StorageContainer>(out var st))
                    {
                        st.SetIsShelf(true);
                        Debug.Log(t.gameObject.name + " isShelf value: " + st.GetIsShelf());
                    }
                }
            }   
        }
        Debug.Log("Storage Container number: " + allShelves.Count);
    }

    //gestisce come viene popolata la ScrollView degli scaffali con i vari button degli elementi della warehouse
    public void CreateShelvesScrollView(GameObject parent)
    {
        if (shelvesButtonCreated.Count > 0) 
        {
            //ClearScrollView(scrollView);
            shelvesButtonCreated.Clear();
        }

        if (parent.name == warehouse.name)
        {
            secondText.gameObject.SetActive(false);
            firstText.GetComponent<TextMeshProUGUI>().text = firstTextString;
            positionButton.transform.parent.gameObject.SetActive(false);
            virtualizedList.gameObject.SetActive(true);
            returnButton.SetActive(false);
        }
        else
        {
            returnButton.SetActive(true);
            secondText.gameObject.SetActive(true);
            firstText.GetComponent<TextMeshProUGUI>().text = firstTextString_2 + "\"" + lastShelfPositioned.name + "\"";
            positionButton.transform.parent.gameObject.SetActive(true);
            TextMeshProUGUI txt = positionButton.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = "Posiziona " + lastShelfPositioned.name;

            if(parent.transform.childCount > 0)
            {
                secondText.GetComponent<TextMeshProUGUI>().text = secondTextString + "\"" + lastShelfPositioned.name + "\"";
                virtualizedList.gameObject.SetActive(true);
            } 
            else
            {
                secondText.GetComponent<TextMeshProUGUI>().text = "\"" + lastShelfPositioned.name + "\"" + " non possiede altre sottocategorie";
                virtualizedList.gameObject.SetActive(false);
            }
        }

        if (virtualizedList.gameObject.activeSelf)
        {
            /*int i = 0;
            foreach (Transform t in parent.transform)
            {
                PressableButton newButton = Instantiate(buttonPrefabShelves, contentTransform);
                newButton.name = t.name;
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = t.name;
                int index = i;
                newButton.OnClicked.AddListener(() => OnShelfButtonClicked(index));
                shelvesButtonCreated.Add(t.gameObject);

                i++;
                //Debug.Log("Button " + index + " created");
            }

            contentTransform.GetComponentInParent<VirtualizedScrollRectList>().SetItemCount(i);
            contentTransform.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1f;*/

            /*int i = 0;
            foreach (Transform t in parent.transform)
            {
                shelvesButtonCreated.Add(t.gameObject);
                i++;
                //Debug.Log("Button " + index + " created");
            }*/

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                shelvesButtonCreated.Add(parent.transform.GetChild(i).gameObject);
            }

            shelvesButtonCreated.Sort((x, y) => x.name.CompareTo(y.name));

            VirtualizedScrollRectListTester list = scrollView.GetComponent<VirtualizedScrollRectListTester>();
            list.SetWords(shelvesButtonCreated);
        } 
    }

    //chiamata quando si clicca su un button della ScrollView
    public void OnShelfButtonClicked(int index)
    {
        //Debug.Log("Button " + index + " clicked");
        
        lastShelfPositioned = shelvesButtonCreated[index];

        //ClearScrollView();

        //if (buttonList[index].transform.childCount > 0)
        //{
        CreateShelvesScrollView(shelvesButtonCreated[index]);
        //}
    }

    //svuota la scrollView prima di ripopolarla
    public void ClearScrollView(Transform content)
    {
        if (content.transform.childCount > 0)
        {
            foreach (Transform t in content.transform)
                Destroy(t.gameObject);
        }
    }

    //ripopola la ScrollView tornando indietro di un livello nella gerarchia di warehouse
    public void BackButtonShelves()
    {
        if (positioningSphere.activeSelf)
            ResetShelfPosition();
        
        lastShelfPositioned = lastShelfPositioned.transform.parent.gameObject;
        CreateShelvesScrollView(lastShelfPositioned);
    }

    //chiamata quando si clicca sul button per posizionare un elemento della warehouse
    public void StartPositioning()
    {
        positioningSphere.SetActive(true);
        sphereIndicator.SetActive(true);
        positioningText.GetComponent<TextMeshProUGUI>().text += " \"" + lastShelfPositioned.name + "\"";
        positioningText.gameObject.SetActive(true);
        positioningButton.transform.parent.gameObject.SetActive(true);
        positioningSphere.transform.position = lastShelfPositioned.transform.position;


        ConstraintSource source = new()
        {
            sourceTransform = positioningSphere.transform,
            weight = 1f
        };
        ParentConstraint pc = lastShelfPositioned.AddComponent<ParentConstraint>();
        pc.AddSource(source);
        pc.enabled = true;
        pc.constraintActive = true;

        firstText.gameObject.SetActive(false);
        positionButton.transform.parent.gameObject.SetActive(false);
        secondText.gameObject.SetActive(false);
        virtualizedList.gameObject.SetActive(false);
    }

    //cambia lo stato del pin del pannello
    public void PinPanel(GameObject panel)
    {
        Follow follow = panel.GetComponent<Follow>();
        follow.IgnoreDistanceClamp = !follow.IgnoreDistanceClamp;
    }

    //gestisce l'attivazione e disattivazione visiva del pin
    public void ChangeToggle(GameObject toggle)
    {
        if (lastToggledPin != null && lastToggledPin != toggle)
            lastToggledPin.SetActive(false);

        lastToggledPin = toggle;

        if (toggle.activeSelf)
        {
            toggle.SetActive(false);
            //Debug.Log("toggle disattivo");
        }
            
        else
        {
            toggle.SetActive(true);
            //Debug.Log("toggle attivo");
        }
            
    }

    //resetta la gestione del pin di un pannello nel momento in cui si chiude
    public void ResetPin(GameObject panel)
    {
        Follow follow = panel.GetComponent<Follow>();
        follow.IgnoreDistanceClamp = false;
        if(lastToggledPin != null)
            lastToggledPin.SetActive(false);
    }

    //aggiunge tutti gli elementi che compongono l'empty Artifacts alla lista allArtifacts e artifactsOnList (per la prima visualizzazione)
    public void GetAllArtifacts(GameObject ar)
    {
        for (int i = 0; i < ar.transform.childCount; i++)
        {
            allArtifacts.Add(ar.transform.GetChild(i).gameObject);
        }

        Debug.Log("Artifacts number: " + allArtifacts.Count);

        allArtifacts.Sort((x,y) => x.name.CompareTo(y.name));
        allArtifacts.ForEach(x => SetArtifactsShelf(x));
    }

    public void SetArtifactsShelf(GameObject artifact)
    {
        string shelfID = PlayerPrefs.GetString(artifact.name);
        Debug.Log("ShelfID: " +  shelfID);
        if(shelfID != "")
            artifact.GetComponent<Artifact>().SetShelfID(shelfID);
    }

    //crea la scrollView con i reperti
    public void CreateArtifactScrollView()
    {
        A_Menu.artifactTitle.GetComponent<TextMeshProUGUI>().text = artifactGeneralText;
        A_Menu.artifactVirualizedList.gameObject.SetActive(true);
        A_Menu.searchBar.SetActive(true);
        A_Menu.artifactBackButton.SetActive(false);
        A_Menu.startNavigationButton.SetActive(false);
        A_Menu.stopNavigationButton.SetActive(false);
        A_Menu.artifactText.SetActive(false);
        A_Menu.navigationText.SetActive(false);
        //artifactSelected = null;

        /*if (A_Menu.artifactContentTransform.childCount > 0)
        {
            ClearScrollView(A_Menu.artifactContentTransform.transform);
        }

        int i = 0;
        foreach (var artifact in allArtifacts)
        {
            PressableButton newButton = Instantiate(A_Menu.artifactButtonPrefab, A_Menu.artifactContentTransform);
            newButton.name = artifact.name;
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = artifact.name;
            int index = i;
            newButton.OnClicked.AddListener(() => OnArtifactButtonClicked(index));
            //artifactsButtonCreated.Add(newButton.gameObject);

            i++;
        }
        Debug.Log("Number of artifact buttons created: " + i);

        A_Menu.artifactContentTransform.GetComponentInParent<VirtualizedScrollRectList>().SetItemCount(i);
        A_Menu.artifactContentTransform.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1f;*/

        VirtualizedScrollRectListTester list = A_Menu.artifactScrollView.GetComponent<VirtualizedScrollRectListTester>();
        list.SetWords(allArtifacts);
        UpdateArtifactList(allArtifacts);
    }

    //gestione del pulsante per tornare indietro nelle varie situazioni in cui può essere cliccato
    public void BackButtonArtifact()
    {
        Debug.Log("Deposit List value: " +  vsrlt.GetForDeposit());
        if (!vsrlt.GetForDeposit())
        {
            A_Menu.artifactVirualizedList.gameObject.SetActive(true);
            A_Menu.searchBar.SetActive(true);
            A_Menu.artifactText.SetActive(false);
            A_Menu.startNavigationButton.SetActive(false);
            A_Menu.artifactBackButton.SetActive(false);

            //if(A_Menu.stopNavigationButton.activeSelf)

            //int i = 0;
            //while (allArtifacts[i] != artifactSelected)
            //{ i++; }
            //OnArtifactButtonClicked(i);

            if (A_Menu.depositList.activeSelf)
            {
                int i = 0;
                while (allArtifacts[i] != artifactSelected)
                { i++; }

                Debug.Log(DateTime.Now.ToString("HH:mm:ss.fff") + ". Deposit list - Attiva");
                OnArtifactButtonClicked(i);
            }
            else
            { 
                artifactSelected = null;
                Debug.Log(DateTime.Now.ToString("HH:mm:ss.fff") + ". Deposit list - Non attiva");
            }
            StopNavigation();
            A_Menu.depositList.SetActive(false);
            
        }
        else
        {
            Debug.Log("Back");
            if (!A_Menu.depositList.activeSelf)
            {
                DepositConfirmed();
                StopNavigation();
                return;
            }
            else
                vsrlt.Back();
        }
    }

    //chiamata quando si clicca sul bottone di un reperto
    public void OnArtifactButtonClicked(int index)
    {
        Debug.Log("Button clicked: " + index + " - " + artifactsOnList[index].name);

        currentPath.Clear();
        string shelfID = artifactsOnList[index].GetComponent<Artifact>().GetShelfID();
        A_Menu.artifactVirualizedList.gameObject.SetActive(false);
        A_Menu.searchBar.SetActive(false);
        A_Menu.artifactTitle.GetComponent<TextMeshProUGUI>().text = artifactTitle + artifactsOnList[index].name;
        artifactSelected = artifactsOnList[index];
        A_Menu.artifactText.SetActive(true);
        A_Menu.artifactBackButton.SetActive(true);

        if (!string.IsNullOrEmpty(shelfID))
        {
            A_Menu.artifactText.GetComponent<TextMeshProUGUI>().text = artifactShelfYes + shelfID;
            A_Menu.startNavigationButton.SetActive(true);
            
            GameObject shelf = FindChildRecursive(warehouse.transform, shelfID);
            CalculatePath(shelf.transform);

            Debug.Log("Path: " + string.Join(" - ", currentPath.Select(go => go.name)));
        }
        else
        {
            A_Menu.artifactText.GetComponent<TextMeshProUGUI>().text = artifactShelfNo;
            A_Menu.depositButton.SetActive(true);

            string lastShelfID = PlayerPrefs.GetString(artifactsOnList[index].name + "_Last");
            if (!string.IsNullOrEmpty(lastShelfID))
            {
                A_Menu.depositButton.GetComponentInChildren<TextMeshProUGUI>().text = textDeposit;
                A_Menu.artifactText.GetComponent<TextMeshProUGUI>().text += ".\n" + artifactShelfLast + " (" + lastShelfID + ")?";
                A_Menu.depositInLastShelfButton.SetActive(true);
            }
            else
            {
                A_Menu.artifactText.GetComponent<TextMeshProUGUI>().text = initialDepositText;
                A_Menu.depositButton.GetComponentInChildren<TextMeshProUGUI>().text = initialDepositButtonText;
            }
        }
    }

    public void UpdateArtifactList(List<GameObject> list)
    {
        artifactsOnList.Clear();
        foreach (GameObject go in list)
        {
            //allArtifacts.Find(x => x.name == go.name);
            artifactsOnList.Add(allArtifacts.Find(x => x.name == go.name));
        }
            

        Debug.Log(string.Join(" - ", artifactsOnList.Select(x => x.name)));
    }

    //trova il gameobject dello scaffale dall'ID salvato nel reperto
    public GameObject FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child.gameObject;

            GameObject result = FindChildRecursive(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    //calcola il tutti gli step del percorso per raggiungere il reperto
    public void CalculatePath(Transform current)
    {
        if (!A_Menu.depositList.activeSelf)
        {
            while (current.gameObject.name != warehouse.name)
            {
                currentPath.Add(current);
                current = current.parent;
            }
            currentPath.Reverse();
        }
        else
        {
            while (current.gameObject.name != warehouse.name)
            {
                currentPath.Add(current);
                current = current.parent;
            }
            currentPath.Reverse();
        }
        
    }

    //inizia la navigazione per portare l'utente al reperto
    public void StartNavigation()
    {
        Debug.Log("Start navigation. Path count = " + currentPath.Count);
        A_Menu.startNavigationButton.SetActive(false);
        A_Menu.stopNavigationButton.SetActive(true);
        A_Menu.solverIndicator.SetActive(true);
        A_Menu.artifactTarget.GetComponent<Follow>().enabled = true;
        A_Menu.artifactTarget.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

       //se parte del percorso del nuovo reperto è uguale a qullo del reperto precedente si saltano quei passaggi
       step = 0;
        if (recentPath.Count > 0)
        {
            while (step < currentPath.Count && step < recentPath.Count)
            {
                if (currentPath[step].name == recentPath[step].name)
                {
                    Debug.Log("Passaggio saltato. " + currentPath[step].name + " - " + recentPath[step].name);
                    step++;
                }
                else
                {
                    /*for (int i = 0; i < recentPath.Count; i++)
                    {
                        if (i >= step)
                        {
                            Debug.Log("Removing " + recentPath[i].name);
                            recentPath.RemoveAt(i);
                            i--;
                        }
                    }*/
                    if (step < recentPath.Count)
                    {
                        recentPath.RemoveRange(step, recentPath.Count - step);
                    }
                    Debug.Log($"Recent path: {string.Join(", ", recentPath.Select(x => x.name))}");
                    break;
                }
            }

            if (currentPath.Count == recentPath.Count)
            {
                Debug.Log("Step--");
                step--;
                recentPath.RemoveRange(step, recentPath.Count - step);
            }
        }
        NextStep();
    }

    //gestione del passaggio del prossimo punto da raggiungere (chiamata anche dal bottone Skip Step nella scena) e del punto di arrivo
    public void NextStep()
    {
        if (step < currentPath.Count)
        {
            A_Menu.artifactTarget.GetComponent<ArtifactIndicator>().SetTargetPosition(currentPath[step]);
            A_Menu.artifactTarget.transform.position = currentPath[step].position;
            Debug.Log("Next step: " + step + " - " + currentPath[step].name);
            A_Menu.artifactTarget.SetActive(true);
            A_Menu.navigationText.SetActive(true);
            A_Menu.navigationText.GetComponent<TextMeshProUGUI>().text = artifactNavigation + currentPath[step].name;

            if (currentPath[step].gameObject.TryGetComponent<StorageContainer>(out var st) )
            {
                if (st.GetIsShelf())
                {
                    A_Menu.artifactTarget.GetComponent<Follow>().enabled = false;
                    A_Menu.artifactTarget.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                    Debug.Log("Rotazione");
                }
                    
            }

            //Ogni volta che si raggiunge la freccia target quel passaggio (che corrisponde a step - 1) viene aggiunto al recentPath.
            //La seconda condizione dell'if serve ad evitare doppioni quando si inizia la navigazione saltando gli step già fatti per l'ultimo reperto !recentPath.Contains(currentPath[step - 1])
            if (step - 1 >= 0 && recentPath.LastOrDefault() != currentPath[step - 1])
                recentPath.Add(currentPath[step - 1]);
            Debug.Log($"Recent path: {string.Join(", ", recentPath.Select(x => x.name))}");
        }
        else
        {
            Debug.Log("Reperto raggiunto! =)");
            //A_Menu.artifactTarget.SetActive(false);
            //A_Menu.solverIndicator.SetActive(false);
            A_Menu.artifactTarget.GetComponent<Follow>().enabled = false;
            A_Menu.artifactTarget.SetActive(true);

            //nel caso tutti gli step siano stati skippati perché si naviga verso lo stesso scaffale
            A_Menu.artifactTarget.GetComponent<ArtifactIndicator>().SetTargetPosition(currentPath[step-1]);
            A_Menu.artifactTarget.transform.position = currentPath[step-1].position;
            
            A_Menu.stopNavigationButton.SetActive(false);
            if (vsrlt.GetForDeposit())
                A_Menu.navigationText.GetComponent<TextMeshProUGUI>().text = targetReached[1];
            else
                A_Menu.navigationText.GetComponent<TextMeshProUGUI>().text = targetReached[0];

            //salvataggio del percorso dell'ultimo reperto raggiunto per saltare eventuali step uguali per il prossimo reperto
            if (recentPath.Count > 0)
                recentPath.Clear();

            foreach (var item in currentPath)
                recentPath.Add(item);

            //VirtualizedScrollRectListTester vsrlt = A_Menu.depositList.GetComponentInChildren<VirtualizedScrollRectListTester>();
            if (!vsrlt.GetForDeposit())
                A_Menu.withdrawButton.SetActive(true);
            else
                A_Menu.depositInShelfButton.SetActive(true);
        }

        if (step <= currentPath.Count)
            step++;
    }

    //quando la freccia target entra nel trigger si passa al punto successivo da raggiungere
    private void OnTriggerEnter(Collider other)
    {
        //step-- serve per la gestione del 
        int tmp = step - 1;
        if (other.gameObject.CompareTag(indicatorTag) && tmp < currentPath.Count)
        {

            //A_Menu.artifactIndicator.GetComponent<ArtifactIndicator>().SetMove(false);
            A_Menu.artifactTarget.SetActive(false);
            A_Menu.triggerEntered.Play();
            Debug.Log("Collider di " + other.gameObject.name + ". Step = " + step);
            NextStep();
        }
    }

    //chiamata quando si interrompe la navigazione verso un reperto o uno scaffale, ma anche dalla funzione del back button e dalla X del pannello reperti
    public void StopNavigation()
    {
        A_Menu.artifactTarget.SetActive(false);
        A_Menu.solverIndicator.SetActive(false);
        A_Menu.stopNavigationButton.SetActive(false);
        A_Menu.navigationText.SetActive(false);
        A_Menu.depositButton.SetActive(false);
        A_Menu.depositInLastShelfButton.SetActive(false);
        A_Menu.depositInShelfButton.SetActive(false);
        foreach (var obj in A_Menu.artifactDepositedUI)
        {
            obj.SetActive(false);
        }

        if (vsrlt.GetForDeposit())
        {
            A_Menu.artifactText.SetActive(true);
            A_Menu.artifactText.GetComponent<TextMeshProUGUI>().text = artifactShelfNo;
        }
            

        if (A_Menu.artifactVirualizedList.gameObject.activeSelf)
        {
            Debug.Log("Deposit list - navigation if");
            A_Menu.artifactTitle.GetComponent<TextMeshProUGUI>().text = artifactGeneralText;
        }
        else
        {
            if (artifactSelected != null)
            {
                Debug.Log("Deposit list - navigation else");
                string shelfID = artifactSelected.GetComponent<Artifact>().GetShelfID();

                if (!string.IsNullOrEmpty(shelfID))
                {
                    A_Menu.startNavigationButton.SetActive(true);
                    Debug.Log("Deposit list - navigation button on");
                }
                else
                {
                    A_Menu.depositButton.SetActive(true);
                    Debug.Log("Deposit list - deposit button on");

                    string lastShelfID = PlayerPrefs.GetString(artifactSelected.name + "_Last");
                    if (!string.IsNullOrEmpty(lastShelfID))
                    {
                        A_Menu.depositButton.GetComponentInChildren<TextMeshProUGUI>().text = textDeposit;
                        A_Menu.artifactText.GetComponent<TextMeshProUGUI>().text += ".\n" + artifactShelfLast + " (" + lastShelfID + ")?";
                        A_Menu.depositInLastShelfButton.SetActive(true);
                    }
                    else
                    {
                        A_Menu.artifactText.GetComponent<TextMeshProUGUI>().text = initialDepositText;
                        A_Menu.depositButton.GetComponentInChildren<TextMeshProUGUI>().text = initialDepositButtonText;
                    }
                }
            }
        }

        step = 0;


        A_Menu.withdrawButton.SetActive(false);
        //A_Menu.depositButton.SetActive(false);
        A_Menu.depositList.SetActive(false);
        //VirtualizedScrollRectListTester vsrlt = A_Menu.depositList.GetComponentInChildren<VirtualizedScrollRectListTester>();
        vsrlt.SetForDeposit(false);

        Debug.Log("Deposit list - Stop Navigation");
    }

    public void ResetPath()
    {
        step = 0;
        A_Menu.artifactTarget.GetComponent<Follow>().enabled = true;
        A_Menu.artifactTarget.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        recentPath.Clear();
        NextStep();
    }

    public void WithdrawArtifact()
    {
        PlayerPrefs.DeleteKey(artifactSelected.name);
        artifactSelected.GetComponent<Artifact>().SetShelfID("");
        BackButtonArtifact();
    }

    public GameObject GetArtifactSelected()
    { return artifactSelected; }

    public void ResetArtifactSelected()
        { artifactSelected = null; }

    public void StartDepositNavigation(GameObject shelf)
    {
        //depositPath.Clear();
        //depositStep = 0;
        //VirtualizedScrollRectListTester vsrlt = A_Menu.depositList.GetComponentInChildren<VirtualizedScrollRectListTester>();
        A_Menu.artifactText.SetActive(false);
        A_Menu.depositList.SetActive(false);
        currentPath.Clear();
        CalculatePath(shelf.transform);
        StartNavigation();
    }

    public void DepositSucceded()
    {
        foreach (var obj in A_Menu.artifactDepositedUI)
        {
            obj.SetActive(true);
        }

        A_Menu.depositList.SetActive(false);
        A_Menu.artifactText.SetActive(false);
        vsrlt.DepositFinished();
    }

    public void DepositConfirmed()
    {
        //A_Menu.depositList.SetActive(true);
        A_Menu.artifactText.SetActive(true);

        foreach (var obj in A_Menu.artifactDepositedUI)
        {
            obj.SetActive(false);
        }

        Debug.Log("Deposit confirmed");
        A_Menu.artifactText.SetActive(true);
        A_Menu.artifactText.GetComponent<TextMeshProUGUI>().text = artifactShelfNo;

        //if (A_Menu.artifactVirualizedList.gameObject.activeSelf)
        //{
        //    A_Menu.artifactText.SetActive(true);
        //    A_Menu.artifactText.GetComponent<TextMeshProUGUI>().text = artifactGeneralText;
        //}

        //VirtualizedScrollRectListTester vsrlt = A_Menu.depositList.GetComponentInChildren<VirtualizedScrollRectListTester>();
        //vsrlt.DepositInShelf();
    }

    public void SearchArtifact(GameObject inputText)
    {
        string txt = inputText.GetComponent<MRTKTMPInputField>().text;
        Debug.Log("Searching: \"" + txt + "\"");
        A_Menu.artifactScrollView.GetComponent<VirtualizedScrollRectListTester>().Searching(txt);
    }

    public List<GameObject> GetShelvesList()
    {
        return allShelves;
    }

    public List<GameObject> GetArtifactsList()
    { return allArtifacts; }

    //funzione di debug chiamata da handmenu per visualizzare la posizione di tutti gli shelves
    public void VisualizeAllShelves(bool visualize)
    {
        string emptyName = "Empty debug";
        if(visualize)
        {
            GameObject emptyDebug = new GameObject(emptyName);
            foreach(GameObject child in allShelves)
            {
                GameObject newCube = Instantiate(debugCube, emptyDebug.transform);
                newCube.name = "Debug_" + child.name;
                newCube.transform.position = child.transform.position;
            }
        }
        else
        {
            GameObject emptyDebug = GameObject.Find(emptyName);
            Destroy(emptyDebug);
        }
    }
}

