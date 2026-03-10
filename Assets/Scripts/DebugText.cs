using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MixedReality.Toolkit.SpatialManipulation;

public class DebugText : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textMeshPro = default;

    public GameObject debugPanel;
    // Start is called before the first frame update
    void Start()
    {
        textMeshPro.text = "test test";
        Application.logMessageReceived += HandleLog;
        debugPanel.GetComponent<Follow>().enabled = false;
        debugPanel.GetComponent<SolverHandler>().enabled = false;
    }


    void HandleLog(string logString, string stackTrace, LogType type)
    {
        textMeshPro.text += logString + "\n";

    }

    public void ResetText()
    {
        textMeshPro.text = "test test\n";
    }

}
