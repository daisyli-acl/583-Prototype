using UnityEngine;
using UnityEngine.EventSystems;

public class ClockHandPressRotate : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform pivot;       // Assign the HandPivot
    public float rotationSpeed = 90f; // degrees per second

    private bool rotating = false;

    private void Awake()
    {
        if (pivot == null)
            pivot = transform as RectTransform;
    }

    private void Update()
    {
        if (rotating)
        {
            float angle = rotationSpeed * Time.deltaTime;
            pivot.Rotate(0, 0, -angle);  // negative = clockwise
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rotating = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rotating = false;
    }
}
