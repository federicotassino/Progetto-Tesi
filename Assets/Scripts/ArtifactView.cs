using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class ArtifactView : MonoBehaviour
{
    public Artifact data;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(Artifact artifact)
    {
        data = artifact;

        gameObject.name = artifact.name;

        //PlayerPrefs.SetInt("ArtifactID_" + artifact.id, artifact.shelvingUnit);
        //PlayerPrefs.SetInt("ArtifactID_" + artifact.id + "_Last", artifact.shelvingUnit);
    }
}
