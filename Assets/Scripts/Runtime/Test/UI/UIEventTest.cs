using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEventTest : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    private void Awake()
    {
        Debug.Log(GetComponent<Button>().name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Pointer Click {name}");
        // RaycastThrough(eventData, ExecuteEvents.pointerClickHandler);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Pointer Enter {name}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"Pointer Exit {name}");
    }

    private void RaycastThrough<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventFunction) where T : IEventSystemHandler
    {
        PointerEventData pointerEventData = (PointerEventData)eventData;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        GameObject curObj = pointerEventData.pointerCurrentRaycast.gameObject ?? pointerEventData.pointerDrag;
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        foreach (var item in raycastResults)
        {
            GameObject nextObj = item.gameObject;
            if (nextObj != null && nextObj != curObj)
            {
                GameObject excuteObj = ExecuteEvents.GetEventHandler<T>(nextObj);
                if (excuteObj != curObj)
                {
                    ExecuteEvents.Execute(excuteObj, pointerEventData, eventFunction);
                    return;
                }
            }
        }
    }
}
