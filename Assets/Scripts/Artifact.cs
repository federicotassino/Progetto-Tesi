using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting.Dependencies.Sqlite; //poi con SQLite4Unity3d ???
using UnityEngine;

public class Artifact : MonoBehaviour
{
    //[PrimaryKey, AutoIncrement]
    private int Id { get; set; }
    private string Name { get; set; }
    private string TextDescription { get; set; }
    public string ShelfID; //poi deve essere un int con ID dello shelf


    public override string ToString()
    {
        return string.Format("[Artifact: Id={0}, Name={1},  TextDescription={2},  ShelvingID={3}]", Id, Name, TextDescription, ShelfID);
    }

    public string GetID()
        { return ShelfID; }

    public void SetShelfID(string id)
        { ShelfID = id; }
}
