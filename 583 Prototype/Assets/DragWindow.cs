using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform dragRectTransform;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        // If not set in the Inspector, use the parent RectTransform as the window
        if (dragRectTransform == null)
        {
            dragRectTransform = transform.parent.GetComponent<RectTransform>();
        }

        // If no canvas is assigned, search up the hierarchy for one
        if (canvas == null)
        {
            Transform testCanvasTransform = transform.parent;

            while (testCanvasTransform != null)
            {
                canvas = testCanvasTransform.GetComponent<Canvas>();
                if (canvas != null)
                {
                    break;
                }

                testCanvasTransform = testCanvasTransform.parent;
            }
        }
    }

    // Drag the window around
    public void OnDrag(PointerEventData eventData)
    {
        if (dragRectTransform == null || canvas == null) return;

        dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // When clicked, bring this window to the top
    public void OnPointerDown(PointerEventData eventData)
    {
        if (dragRectTransform == null) return;

        dragRectTransform.SetAsLastSibling();
    }
}
