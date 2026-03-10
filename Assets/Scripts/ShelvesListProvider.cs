using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelvesListProvider : MonoBehaviour
{
    [SerializeField] private PressableButton buttonPrefabShelves;
    [SerializeField] private Transform contentTransform;
    public List<PressableButton> buttons = new List<PressableButton>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetItemData(GameObject itemData, int index)
    {
        foreach (GameObject item in itemData.transform)
        {
            GameObject obj = Instantiate(buttonPrefabShelves, contentTransform).gameObject;
            obj.GetComponent<ShelvesListItem>().SetData(item.name);
        }
    }

    public void SetItemData()
    {
        foreach (var item in buttons)
        {
            PressableButton obj;
            obj = Instantiate(buttonPrefabShelves, contentTransform);
            //obj.GetComponent<ShelvesListItem>().SetData(buttons[index].name);
        }
    }
}
