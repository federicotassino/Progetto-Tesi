using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArtifactIndicator : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool lookingForShelf = false;
    [SerializeField]
    private GameObject player;
    public float speed;
    public float amplitude;

    private void Start()
    {
        //transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void FixedUpdate()
    {
        if (!lookingForShelf)
        {
            float y = Mathf.Sin(Time.time * speed) * amplitude;
            transform.position = targetPosition + new Vector3(0f, y, 0f);
        }
        else
        {
            float offset = Mathf.Sin(Time.time * speed) * amplitude;
            Vector3 v = player.transform.position - targetPosition;
            Vector3 dir = Vector3.Scale(v, new Vector3(1, 0, 1));
            Quaternion rot = Quaternion.LookRotation(dir);
            
            transform.rotation = rot * Quaternion.Euler(90f, 0f, 0f);
            transform.position = targetPosition + dir.normalized * offset;
        }
    }

    public void SetTargetPosition(Transform shelfTransform)
    {
        if (shelfTransform.gameObject.TryGetComponent<StorageContainerView>(out var st))
            lookingForShelf = st.data.GetIsShelf();

        Vector3 value = shelfTransform.position;
        if (!lookingForShelf)
            targetPosition = value + new Vector3(0f, (amplitude / 2f), 0f);
        else
            targetPosition = value;
    }
}
