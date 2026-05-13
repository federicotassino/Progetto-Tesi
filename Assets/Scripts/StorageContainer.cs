using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageContainer
{
    //[PrimaryKey, AutoIncrement]
    public int id;
    public string name;
    public string worldTransform;
    public int parentShelfId;
    public bool isShelf = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIsShelf(bool value)
    {
        isShelf = value;
    }

    public bool GetIsShelf()
        { return isShelf; }
}
