using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableLoadingBar : MonoBehaviour,
    IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Slider slider;
    public TextMeshProUGUI percentageText;
    public float autoFillSpeed = 0.2f;

    public TrafficLight trafficLight;

    private bool isDragging = false;
    private float maxAutoFill = 0.98f;
    private bool hasReached100 = false;

    void Start()
    {
        // Hide the loading bar until a checkpoint activates it
        gameObject.SetActive(false);

        slider.value = 0f;
        hasReached100 = false;

        if (trafficLight != null)
            trafficLight.SetRed();
    }

    void Update()
    {
        if (!gameObject.activeSelf)
            return;

        if (!isDragging && slider.value < maxAutoFill)
        {
            slider.value += autoFillSpeed * Time.deltaTime;
        }

        percentageText.text = Mathf.RoundToInt(slider.value * 100f) + "%";

        if (!hasReached100 && slider.value >= 1f)
        {
            hasReached100 = true;

            if (trafficLight != null)
                trafficLight.SetGreen();
        }
    }

    // Called by CheckpointActivator
    public void ActivateBar()
    {
        gameObject.SetActive(true);     // ENABLE loading bar

        slider.value = 0f;
        hasReached100 = false;

        if (trafficLight != null)
            trafficLight.SetRed();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        UpdateSlider(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        if (slider.value > maxAutoFill && slider.value < 1f)
            slider.value = 1f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateSlider(eventData);
    }

    private void UpdateSlider(PointerEventData eventData)
    {
        RectTransform rt = slider.fillRect.parent.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            float pct = Mathf.InverseLerp(rt.rect.xMin, rt.rect.xMax, localPoint.x);
            slider.value = pct;
        }
    }
}
