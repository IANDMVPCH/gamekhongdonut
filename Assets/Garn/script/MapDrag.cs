using UnityEngine;
using UnityEngine.EventSystems;

public class MapDrag : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("Map")]
    public RectTransform mapContent;

    [Header("Drag Settings")]
    public float dragSpeed = 1f;

    [Header("Movement Limits")]
    public float minX = -1000f;
    public float maxX = 1000f;
    public float minY = -700f;
    public float maxY = 700f;

    private Vector2 lastMousePosition;

    public void OnPointerDown(PointerEventData eventData)
    {
        lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentMousePosition = eventData.position;

        Vector2 difference = currentMousePosition - lastMousePosition;

        mapContent.anchoredPosition += difference * dragSpeed;

        Vector2 position = mapContent.anchoredPosition;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        mapContent.anchoredPosition = position;

        lastMousePosition = currentMousePosition;
    }
}