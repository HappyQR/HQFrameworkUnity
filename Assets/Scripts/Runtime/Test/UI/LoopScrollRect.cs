using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoopScrollRect : ScrollRect
{
    public event Action onScroll;

    public event Action onBeginScroll;

    public event Action onEndScroll;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnScroll(PointerEventData data)
    {
        base.OnScroll(data);
        onScroll?.Invoke();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
    }
}
