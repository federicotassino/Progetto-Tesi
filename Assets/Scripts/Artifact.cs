using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting.Dependencies.Sqlite; //poi con SQLite4Unity3d ???
using UnityEngine;

[System.Serializable]
public class Artifact
{
    //[PrimaryKey, AutoIncrement]
    public int id;
    public string name;
    public string textDescription;
    public int shelvingUnit = -1; //poi deve essere un int con ID dello shelf
    public int lastShelvingUnit = -1;

    public override string ToString()
    {
        return string.Format("[Artifact: Id={0}, Name={1},  TextDescription={2},  ShelvingUnit={3}, LastShelvingUnit={4}]", 
            id, name, textDescription, shelvingUnit.ToString(), lastShelvingUnit.ToString());
    }

    public int GetShelfID()
        { return shelvingUnit; }

    public void SetShelfID(int id)
    { 
        shelvingUnit = id;
        lastShelvingUnit = id;
    }

    public void SetName(string nome)
    { this.name = nome; }
}
