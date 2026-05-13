using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageContainerView : MonoBehaviour
{
    public StorageContainer data;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetData(StorageContainer shelf)
    {
        data = shelf;

        // 🔥 qui puoi anche rinominare il GameObject
        gameObject.name = shelf.name;
    }
}
