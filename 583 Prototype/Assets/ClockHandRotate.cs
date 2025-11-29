using UnityEngine;
using UnityEngine.EventSystems;

public class ClockHandRotate : MonoBehaviour, IDragHandler
{
    public RectTransform pivot; // assign HandPivot to itself

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 screenPivot = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, pivot.position);
        Vector2 dir = eventData.position - screenPivot;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle -= 90f; // make upward = 0 degrees

        pivot.rotation = Quaternion.Euler(0, 0, angle);
    }
}
