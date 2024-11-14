using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoopList : MonoBehaviour
{
    LoopScrollRect rect;

    public void OnScroll()
    {
        Debug.Log(1);
    }

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponentInParent<LoopScrollRect>();
        rect.onScroll += OnScroll;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
