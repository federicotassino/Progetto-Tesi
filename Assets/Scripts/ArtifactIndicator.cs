using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactIndicator : MonoBehaviour
{
    private Vector3 targetPosition;
    public float speed;
    public float amplitude;

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        float y = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = targetPosition + new Vector3(0f, y, 0f);
    }

    public void SetTargetPosition(Vector3 value)
    {
        targetPosition = value + new Vector3(0f, (amplitude/2f), 0f);
    }
}
