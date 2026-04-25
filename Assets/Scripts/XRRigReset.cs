using MixedReality.Toolkit;
using MixedReality.Toolkit.Subsystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRRigReset : MonoBehaviour
{
    [SerializeField]
    private GameObject xrRig;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ResetXR());
    }

    IEnumerator ResetXR()
    {
        yield return new WaitForSeconds(1f);
        xrRig.SetActive(false);
        yield return new WaitForSeconds(1f);
        xrRig.SetActive(true);
    }
}