using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageContainer : MonoBehaviour
{
    //[PrimaryKey, AutoIncrement]
    private int Id { get; set; }
    private string Name { get; set; }
    private bool isShelf = false;


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
