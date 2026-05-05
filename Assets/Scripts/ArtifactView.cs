using System.Collections;
using System.Collections.Generic;
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

        // 🔥 qui puoi anche rinominare il GameObject
        gameObject.name = artifact.name;
    }
}
