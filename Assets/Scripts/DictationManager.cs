using MixedReality.Toolkit;
using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DictationManager : MonoBehaviour
{
    private DictationSubsystem dictationSubsystem;
    private bool dictating = false;

    [SerializeField] private MRTKTMPInputField inputField;
    [SerializeField] private GameObject micButtonOff;
    [SerializeField] private GameObject micButtonOn;


    // Start is called before the first frame update
    void Start()
    {
        micButtonOn.SetActive(false);

        dictationSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<DictationSubsystem>();
        if (dictationSubsystem != null)
        {
            dictationSubsystem.Recognized += OnDictationResult;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDictationResult(DictationResultEventArgs args)
    {
        Debug.Log("Testo dettato: " + args.Result.ToString());
        if (inputField.text != "")
            inputField.text += " ";

        inputField.text += args.Result.ToString();
        
    }

    public void StartDictation()
    {
        Debug.Log("Inzio dettatura");
        dictationSubsystem.StartDictation();
        dictating = true;
        StartCoroutine(Dictating());
    }

    public void StopDictation()
    {
        if (dictating)
        {
            Debug.Log("Fine dettatura");
            dictationSubsystem.StopDictation();
            micButtonOn.SetActive(false);
            micButtonOff.SetActive(true);
            StopCoroutine(Dictating()); //altrimenti se viene effettuata una nuova dettatura la coroutine della prima la interrompe prima del dovuto
            dictating = false;
        }
    }

    IEnumerator Dictating()
    {
        yield return new WaitForSeconds(10f);
        StopDictation();
    }
}
