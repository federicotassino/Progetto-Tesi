using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShelvesListItem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetData(string text)
    {
        this.gameObject.name = text;
        this.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
}
