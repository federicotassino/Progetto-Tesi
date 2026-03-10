using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PositionSavior : MonoBehaviour
{

    //private bool positionSaved = false;
    //[SerializeField]
    //private bool savePositionOnFile;

    public GameObject reperti;
    public GameObject plate;

    // Start is called before the first frame update
    void Start()
    {
        reperti.SetActive(false);
        plate.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (savePositionOnFile)
        {
            Debug.Log("Stanza salvata su file");
            savePositionOnFile = false;

            string path = "Assets/Resources/Posizioni oggetti.txt";

            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(transform.position);
            writer.Close();
        }*/
       
    }

    /*public void SaveRoomOnFile()
    {
        Debug.Log("Stanza script");
        if (!positionSaved)
        {
            Debug.Log("Stanza salvata su file");
            positionSaved = true;
        }
    }*/

    public void OpenPanel()
    {
        reperti.SetActive(true);
        foreach (Transform item in reperti.transform)
        {
            item.position = new Vector3(item.position.x, reperti.transform.position.y, item.position.z);
        }
        //reperti.transform.rotation = new Quaternion(-90f, reperti.transform.rotation.y, reperti.transform.rotation.z, 1);
        reperti.transform.Rotate(-90f, 0f, 0f, Space.World);
        plate.SetActive(true);
        plate.transform.position = reperti.transform.position;
    }
}
