using MixedReality.Toolkit.UX.Experimental;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForwardDragToScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ScrollRect scrollRect;

    private void Awake()
    {
        GameObject ob = GetComponentInParent<ScrollRect>().gameObject;
        scrollRect = ob.GetComponent<ScrollRect>();
        Debug.Log("Inizializzazione: " + scrollRect);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        scrollRect.OnBeginDrag(eventData);
        Debug.Log("Forward");
    }

    public void OnDrag(PointerEventData eventData)
    {
        scrollRect.OnDrag(eventData);
        Debug.Log("Forward");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
        Debug.Log("Forward");
    }
}

/*public class ForwardScrollToParent : MonoBehaviour, IMixedRealityScrollHandler
{
    private IMixedRealityScrollHandler parentScrollHandler;

    void Start()
    {
        parentScrollHandler = GetComponentInParent<IMixedRealityScrollHandler>();
    }

    public void OnScroll(ScrollEventData eventData)
    {
        parentScrollHandler?.OnScroll(eventData);
    }
}*/
