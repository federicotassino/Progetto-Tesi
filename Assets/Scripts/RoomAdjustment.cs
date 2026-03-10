using Microsoft.MixedReality.WorldLocking.Core;
using MixedReality.Toolkit;
using MixedReality.Toolkit.SpatialManipulation;
using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class RoomAdjustment : MonoBehaviour
{
    private const float moveObjectCurator = 0.01f;
    private const float rotateObjectCurator = 1f;

    private bool curatorMode = false;
    private GameObject lastClickedObjectModel;
    private GameObject lastObjectCurator;
    //private bool insideTrigger = false;
    private string lastTriggerEnteredName = "";
    private bool objectClicked = false;
    private bool objectHovered = false;
    private Material lastHoveredObjectMaterial = null;
    private AudioSource audioDescription;

    public GameObject listaReperti;
    public Material hoverMaterial;
    public GameObject exitTriggerReperto;
    public GameObject objectPanelEmpty;
    public GameObject curatorPanel;
    public GameObject unityAxis;
    public GameObject messagePanel;
    public GameObject toggleButton;
    public Material[] materialsMummia;
    //public AudioSource mummiaAudioDescription;
    public GameObject audioguidaButton;


    // Start is called before the first frame update
    void Start()
    {
        WorldLockingManager.GetInstance().Load();
        //PlayerPrefs.DeleteAll();

        int i = 0;
        foreach (Transform item in listaReperti.transform)
        {
            i++;
            GameObject reperto = item.gameObject;
            Debug.Log("reperto: " + reperto.name);
            SetInitialTransform(reperto);
        }

        Debug.Log("numero reperti: " + i);
        
        objectPanelEmpty.SetActive(false);
        //objectPanelEmpty.GetComponentInChildren<Follow>().enabled = false;
        //objectPanelEmpty.GetComponentInChildren<SolverHandler>().enabled = false;
        objectPanelEmpty.GetComponentInChildren<Follow>().IgnoreDistanceClamp = true;
        curatorPanel.SetActive(false);
        //curatorPanel.GetComponentInChildren<Follow>().IgnoreDistanceClamp = true;
        exitTriggerReperto.SetActive(false);
        messagePanel.SetActive(false);
        messagePanel.GetComponentInChildren<Follow>().enabled = false;
        messagePanel.GetComponentInChildren<SolverHandler>().enabled = false;
        unityAxis.SetActive(false);

        SetApplicationMode(false); //ottimizzabile per non fare due cicli su lista reperti?
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BoundsManipulation()
    {
        /*if (curatorMode)
        {
            lastObjectCurator.GetComponent<ParentConstraint>().constraintActive = false;
            ConstraintSource source = new()
            {
                sourceTransform = lastObjectCurator.transform,
                weight = 1f
            };
            ParentConstraint parentConstraint = unityAxis.AddComponent<ParentConstraint>();
            parentConstraint.AddSource(source);
            parentConstraint.constraintActive = true;
            parentConstraint.rotationAxis = UnityEngine.Animations.Axis.None;
        }*/
    }

    //se in modalità curatore apre il pannello curatore per le modifiche su posizione e rotazione
    public void EndOfManipulation(GameObject objectToSave)
    {
        if (curatorMode)
        {
            if (!curatorPanel.activeSelf)
            {
                curatorPanel.transform.position = this.transform.position + 0.5f * this.transform.forward + 0.2f * this.transform.right;
                curatorPanel.transform.forward = this.transform.forward;
                curatorPanel.SetActive(true);
                lastObjectCurator = objectToSave;
                objectToSave.GetComponent<BoundsControl>().HandlesActive = true;

                unityAxis.transform.position = lastObjectCurator.transform.position;
                ConstraintSource source = new()
                {
                    sourceTransform = unityAxis.transform,
                    weight = 1f
                };
                /*unityAxis.GetComponent<ParentConstraint>().AddSource(source);  //la source precedente viene eliminata da editor tramite i bottoni annulla e salva modifiche
                unityAxis.GetComponent<ParentConstraint>().constraintActive = true;*/
                unityAxis.SetActive(true);

                ParentConstraint parentConstraint = objectToSave.AddComponent<ParentConstraint>();
                parentConstraint.AddSource(source);
                parentConstraint.rotationAxis = UnityEngine.Animations.Axis.None;
                parentConstraint.constraintActive = true;

                objectToSave.GetComponent<ObjectManipulator>().HostTransform = unityAxis.transform;

                //Destroy(objectToSave.GetComponent<ParentConstraint>()); da fare dopo quando si salva o annulla
                //objectToSave.GetComponent<ObjectManipulator>().HostTransform = objectToSave.transform;

                foreach (Transform item in listaReperti.transform)
                {
                    if (item.gameObject.name != objectToSave.name)
                    {
                        item.GetComponent<ObjectManipulator>().enabled = false;
                        item.GetComponent<BoundsControl>().enabled = false;
                    }
                }
            }

            //quando si finisce di manipolare si riattiva il constraint del reperto e si distrugge quello degli assi, serve nel caso in cui la manipolazione è stata fatta da boundsControl
            /*if (unityAxis.TryGetComponent<ParentConstraint>(out var axisConstraint))
            {
                Destroy(axisConstraint);
            }*/

            //objectToSave.GetComponent<ParentConstraint>().constraintActive = true;
        }
    }

    public void SaveTransformObject()
    {
        WorldLockingManager.GetInstance().Save();
        Debug.Log("salvataggio mondo");

        lastObjectCurator.transform.GetPositionAndRotation(out var positionTemp, out var rotationTemp);
        string objectPosition = positionTemp.x + "_" + positionTemp.y + "_" + positionTemp.z;
        string objectRotation = rotationTemp.x + "_" + rotationTemp.y + "_" + rotationTemp.z + "_" + rotationTemp.w;
        string objectTransform = objectPosition + "/" + objectRotation;
        PlayerPrefs.SetString(lastObjectCurator.name, objectTransform);
        Debug.Log("stringa salvata: " + objectTransform);

        lastObjectCurator.GetComponent<BoundsControl>().HandlesActive = false;
        lastObjectCurator.GetComponent<ObjectManipulator>().HostTransform = lastObjectCurator.transform;

        if (lastObjectCurator.TryGetComponent<ParentConstraint>(out var parentConstraint))
            Destroy(parentConstraint);

        curatorPanel.GetComponentInChildren<Follow>().IgnoreDistanceClamp = false;

        foreach (Transform item in listaReperti.transform)
        {
            item.GetComponent<ObjectManipulator>().enabled = true;
            item.GetComponent<BoundsControl>().enabled = true;
        }
    }

    //riposiziona il reperto all'ultima posizione salvata (sia all'avvio dell'applicazione che quando si finisce di interagire con il reperto
    public void SetInitialTransform(GameObject reperto)
    {
        string transformReperto = PlayerPrefs.GetString(reperto.name);
        if (!string.IsNullOrEmpty(transformReperto))
        {
            Debug.Log("settaggio iniziale reperto " + reperto.name);
            string[] transform = transformReperto.Split('/');

            string[] position = transform[0].Split('_');
            string[] rotation = transform[1].Split('_');

            reperto.transform.position = new Vector3(float.Parse(position[0]), float.Parse(position[1]), float.Parse(position[2]));
            reperto.transform.rotation = new Quaternion(float.Parse(rotation[0]), float.Parse(rotation[1]), float.Parse(rotation[2]), float.Parse(rotation[3]));

            //reset scala
            reperto.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Reperto"))
        {
            Debug.Log("Trigger " + other.gameObject.name);
            if (!curatorMode)
            {
                if (!objectClicked)
                {
                    if (!string.IsNullOrEmpty(lastTriggerEnteredName))
                    {
                        //se si era già all'interno di un trigger si disattiva la manipolazione di tutti i reperti ed in seguito si attiva per l'ultimo a cui ci si è avvicinati
                        Debug.Log("Disattivo le altre manipolazioni");
                        foreach (Transform item in listaReperti.transform)
                        {
                            item.GetComponentInChildren<ObjectManipulator>().AllowedManipulations = MixedReality.Toolkit.TransformFlags.None;
                            item.GetComponentInChildren<BoundsControl>().ToggleHandlesOnClick = false;
                        }

                    }
                    //insideTrigger = true;
                    lastTriggerEnteredName = other.gameObject.name;
                    other.transform.GetComponentInParent<ObjectManipulator>().AllowedManipulations = MixedReality.Toolkit.TransformFlags.Move
                        | MixedReality.Toolkit.TransformFlags.Rotate | MixedReality.Toolkit.TransformFlags.Scale;
                    other.transform.GetComponentInParent<BoundsControl>().ToggleHandlesOnClick = true;

                    ObjectClicked(other.gameObject);
                }
            }
            else
            {
                //insideTrigger = true;
                lastTriggerEnteredName = other.gameObject.name;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        /*if (!curatorMode && other.gameObject.tag == "Reperto" && lastClickedObjectModel != null) 
        {
            // questo sotto funziona ma fa la stessa cosa della funzione (nel caso aggiungere renderer.enable = false)

            /*GameObject emptyReperto = other.gameObject.transform.parent.gameObject;
            emptyReperto.GetComponent<BoundsControl>().HandlesActive = false;
            objectClicked = false;
            resetPositionButton.SetActive(false);
            SetInitialTransform(emptyReperto);

            if (other.gameObject.name == lastClickedObjectModel.name)
                ClosePanelAndResetPosition();
        }*/

        //if (!curatorMode)
        //{
        if (other.gameObject.CompareTag("Reperto") && !objectClicked && other.name == lastTriggerEnteredName)
        {
            Debug.Log("Uscito dal trigger del reperto");
            //insideTrigger = false;
            lastTriggerEnteredName = "";
            if (!curatorMode)
            {
                other.transform.GetComponentInParent<ObjectManipulator>().AllowedManipulations = MixedReality.Toolkit.TransformFlags.None;
                other.transform.GetComponentInParent<BoundsControl>().ToggleHandlesOnClick = false;
            }

        }

        if (other.gameObject.CompareTag("Trigger Exit"))
        {
            Debug.Log("Uscito dal trigger exit");
            ClosePanelAndResetPosition();
            other.gameObject.SetActive(false);
            //insideTrigger = false;
            lastTriggerEnteredName = "";
            lastClickedObjectModel.transform.parent.GetComponent<ObjectManipulator>().AllowedManipulations = MixedReality.Toolkit.TransformFlags.None;
            lastClickedObjectModel.transform.GetComponentInParent<BoundsControl>().ToggleHandlesOnClick = false;
        }
        //}
    }

    //funzione chiamata quando ci si avvicina o si interagisce con il reperto (lo rende visibile e apre il pannello informazioni)
    public void ObjectClicked(GameObject repertoModel)
    {
        if (!curatorMode && !objectClicked)
        {
            if (repertoModel.name == lastTriggerEnteredName)
            {
                Debug.Log("reperto cliccato");
                objectClicked = true;
                lastClickedObjectModel = repertoModel;
                if (lastHoveredObjectMaterial != null)
                {
                    repertoModel.GetComponent<Renderer>().material = lastHoveredObjectMaterial;
                }

                repertoModel.GetComponent<Renderer>().enabled = true;
                objectPanelEmpty.transform.position = this.transform.position + 0.5f * this.transform.forward + 0.2f * this.transform.right;
                objectPanelEmpty.transform.forward = this.transform.forward;
                objectPanelEmpty.SetActive(true);

                //impedisce di interagire con gli altri reperti quando ce ne è già uno selezionato
                foreach (Transform item in listaReperti.transform)
                {
                    if (item.gameObject.name != repertoModel.transform.parent.gameObject.name)
                    {
                        item.GetComponent<ObjectManipulator>().enabled = false;
                        item.GetComponent<BoundsControl>().enabled = false;
                    }
                }

                exitTriggerReperto.transform.position = repertoModel.transform.position;
                exitTriggerReperto.SetActive(true);
                if (messagePanel.activeSelf)
                { messagePanel.SetActive(false); }

                audioDescription = repertoModel.GetComponentInParent<AudioSource>();
                StartAudioDescription();
            }
            else
            {
                Debug.Log("Sei troppo lontano dall'oggetto, per favore avvicinati");
                messagePanel.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Sei troppo lontano dall'oggetto, per favore avvicinati";
                messagePanel.transform.position = this.transform.position + 0.5f * this.transform.forward;
                messagePanel.transform.forward = this.transform.forward;
                messagePanel.SetActive(true);
            }

        }

        if(curatorMode && curatorPanel.activeSelf)
        {
            lastObjectCurator.GetComponent<ParentConstraint>().constraintActive = true;
        }
    }

    //gestisce l'hover in e out dei reperti
    public void ObjectHovered(GameObject repertoModel)
    {
        if (!curatorMode)
        {
            if (!objectHovered)
            {
                objectHovered = true;

                if (!objectClicked)
                {
                    lastHoveredObjectMaterial = repertoModel.GetComponent<Renderer>().material;
                    repertoModel.GetComponent<Renderer>().material = hoverMaterial;
                    repertoModel.GetComponent<Renderer>().enabled = true;
                }
            }
            else
            {
                objectHovered = false;

                if (!objectClicked)
                {
                    repertoModel.GetComponent<Renderer>().enabled = false;
                    repertoModel.GetComponent<Renderer>().material = lastHoveredObjectMaterial;
                    lastHoveredObjectMaterial = null;
                }
            }
        }
    }

    //funzione chiamata quando si clicca sulla x del pannello informazioni o si esce dal trigger: riposiziona il reperto e chiude il pannello
    public void ClosePanelAndResetPosition()
    {
        objectClicked = false;
        GameObject objectEmpty = lastClickedObjectModel.transform.parent.gameObject;
        objectEmpty.GetComponent<BoundsControl>().HandlesActive = false;
        lastClickedObjectModel.GetComponent<Renderer>().enabled = false;
        SetInitialTransform(objectEmpty);
        objectPanelEmpty.SetActive(false);
        lastHoveredObjectMaterial = null;
        exitTriggerReperto.SetActive(false);

        if (audioDescription != null)
        {
            if (audioDescription.isPlaying)
            { audioDescription.Stop(); }
            audioDescription = null;
        }

        //permette nuovamente di interagire con gli altri reperti
        foreach (Transform item in listaReperti.transform)
        {
            if (item.gameObject.name != lastClickedObjectModel.transform.parent.gameObject.name)
            {
                item.GetComponent<ObjectManipulator>().enabled = true;
                item.GetComponent<BoundsControl>().enabled = true;
            }
        }
    }

    //funzione chiamata quando si clicca sul bottone riposiziona reperto (non chiude il pannello informazioni) o su elimina modifiche di curatorPanel
    public void ResetPosition()
    {
        if (!curatorMode) 
        {
            GameObject objectEmpty = lastClickedObjectModel.transform.parent.gameObject;
            SetInitialTransform(objectEmpty);
        }
        else
        {
            if (lastObjectCurator.TryGetComponent<ParentConstraint>(out var parentConstraint))
                Destroy(parentConstraint);
            
            SetInitialTransform(lastObjectCurator);
            lastObjectCurator.GetComponent<BoundsControl>().HandlesActive = false;
            lastObjectCurator.GetComponent<ObjectManipulator>().HostTransform = lastObjectCurator.transform;

            curatorPanel.GetComponentInChildren<Follow>().IgnoreDistanceClamp = false;

            foreach (Transform item in listaReperti.transform)
            {
                item.GetComponent<ObjectManipulator>().enabled = true;
                item.GetComponent<BoundsControl>().enabled = true;
            }
        }
    }

    //permette di attivare e disattivare la modalità curatore
    public void SetApplicationMode(bool value)
    {
        if (curatorPanel.activeSelf)
        {
            curatorPanel.SetActive(false);
            unityAxis.SetActive(false);
            ResetPosition();
        }

        curatorMode = value;
        Debug.Log("Modalità curatore attiva: " + curatorMode);

        foreach (Transform reperto in listaReperti.transform)
        {
            if (curatorMode)
            {
                if (objectClicked)
                    ClosePanelAndResetPosition();

                //se è attiva la modalità curatore i reperti possono essere solo spostati e ruotati
                reperto.GetComponent<ObjectManipulator>().AllowedManipulations = MixedReality.Toolkit.TransformFlags.Move;
                reperto.GetComponent<BoundsControl>().EnabledHandles = HandleType.Rotation;
                reperto.GetComponent<BoundsControl>().ToggleHandlesOnClick = true;

                //in modalità curatore posso vedere tutti gli oggetti
                reperto.GetComponentInChildren<Renderer>().enabled = true;
            }
            else
            {
                //altrimenti i reperti possono essere anche scalati
                //reperto.GetComponent<ObjectManipulator>().AllowedManipulations = MixedReality.Toolkit.TransformFlags.Move | MixedReality.Toolkit.TransformFlags.Rotate
                //  | MixedReality.Toolkit.TransformFlags.Scale;
                reperto.GetComponent<BoundsControl>().EnabledHandles = HandleType.Rotation | HandleType.Scale;
                reperto.GetComponent<BoundsControl>().HandlesActive = false;

                //in modalità visita inizialmente gli oggetti non sono renderizzati
                reperto.GetComponentInChildren<Renderer>().enabled = false;

                if (reperto.GetChild(0).name != lastTriggerEnteredName)
                {
                    reperto.GetComponent<ObjectManipulator>().AllowedManipulations = MixedReality.Toolkit.TransformFlags.None;
                    reperto.GetComponent<BoundsControl>().ToggleHandlesOnClick = false;
                }
                else
                {
                    reperto.GetComponent<ObjectManipulator>().AllowedManipulations = MixedReality.Toolkit.TransformFlags.Move | MixedReality.Toolkit.TransformFlags.Rotate
                        | MixedReality.Toolkit.TransformFlags.Scale;
                    reperto.GetComponent<BoundsControl>().ToggleHandlesOnClick = true;
                }
            }
        }
        /*}
        else
        {
            /*messagePanel.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Chiudi il pannello del reperto prima di cambiare modalità";
            messagePanel.transform.position = this.transform.position + 0.5f * this.transform.forward;
            messagePanel.transform.forward = this.transform.forward;
            messagePanel.SetActive(true);
            toggleButton.GetComponent<PressableButton>().ForceSetToggled(!value, false);


        }*/

    }

    //cambia la texture della mummia di gatto
    public void ChangeTexture(int textureIndex)
    {
        if (lastClickedObjectModel.name == "Mummia di Gatto")
        {
            lastClickedObjectModel.GetComponent<Renderer>().material = materialsMummia[textureIndex];
        }
    }

    public void StartAudioDescription()
    {
        if (audioDescription != null)
        {
            if (!audioDescription.isPlaying)
            {
                audioDescription.Play();
                audioguidaButton.GetComponent<TextMeshProUGUI>().text = "Disattiva audioguida";
            }
            else
            {
                audioDescription.Stop();
                audioguidaButton.GetComponent<TextMeshProUGUI>().text = "Attiva audioguida";
            }
        }


        //if (lastClickedObjectModel.name == "Mummia di Gatto")
        //  mummiaAudioDescription.Play();
    }

  
    public void PinPanel(GameObject panel)
    {
        Follow follow = panel.GetComponent<Follow>();
        follow.IgnoreDistanceClamp = !follow.IgnoreDistanceClamp;
    }

    public void ButtonTest()
    {
        Debug.Log("Prova bottone");
    }
}

