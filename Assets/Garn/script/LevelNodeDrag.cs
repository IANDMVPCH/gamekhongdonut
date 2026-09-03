using UnityEngine;
using UnityEngine.EventSystems;

public class LevelNodeDrag : MonoBehaviour, 
    IPointerDownHandler, 
    IDragHandler
{
    public RectTransform mapContent;

    private Vector2 lastPosition;

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentPosition = eventData.position;
        Vector2 difference = currentPosition - lastPosition;

        mapContent.anchoredPosition += difference;

        lastPosition = currentPosition;
    }
}