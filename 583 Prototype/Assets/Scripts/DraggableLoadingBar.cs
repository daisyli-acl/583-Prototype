using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableLoadingBar : MonoBehaviour,
    IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Slider slider;
    public TextMeshProUGUI percentageText;   // <--- add this
    public float autoFillSpeed = 0.2f;
    private bool isDragging = false;

    private float maxAutoFill = 0.98f;

    void Update()
    {
        if (!isDragging)
        {
            if (slider.value < maxAutoFill)
                slider.value += autoFillSpeed * Time.deltaTime;
        }

        // Update text every frame
        percentageText.text = Mathf.RoundToInt(slider.value * 100f) + "%";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        UpdateSliderValue(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateSliderValue(eventData);
    }

    private void UpdateSliderValue(PointerEventData eventData)
    {
        RectTransform rt = slider.fillRect.parent.GetComponent<RectTransform>();

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            float pct = Mathf.InverseLerp(rt.rect.xMin, rt.rect.xMax, localPoint.x);
            slider.value = pct;
        }
    }
}
