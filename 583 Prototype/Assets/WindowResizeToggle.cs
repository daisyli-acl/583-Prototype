using UnityEngine;

public class WindowScaleToggle : MonoBehaviour
{
    [Header("Window Reference")]
    public RectTransform window;

    [Header("Small Window Settings")]
    public float scaleFactor = 0.5f;

    private Vector3 originalScale;
    private Vector2 originalPosition;

    private Vector2 smallPosition;
    private bool hasSmallPosition = false;
    private bool isSmall = false;

    private void Awake()
    {
        if (window == null)
            window = GetComponent<RectTransform>();

        // Save the original (big window) setup
        originalScale = window.localScale;
        originalPosition = window.anchoredPosition;
    }

    public void ToggleSmallWindow()
    {
        if (!isSmall)
        {
            // GOING FROM BIG → SMALL

            // If we have NEVER stored a small position before, use current position
            if (!hasSmallPosition)
            {
                smallPosition = window.anchoredPosition;
                hasSmallPosition = true;
            }

            // Shrink and move to saved small window position
            window.localScale = originalScale * scaleFactor;
            window.anchoredPosition = smallPosition;

            isSmall = true;
        }
        else
        {
            // GOING FROM SMALL → BIG

            // Save the UPDATED small window position before going big
            smallPosition = window.anchoredPosition;
            hasSmallPosition = true;

            // Restore to original big size + position
            window.localScale = originalScale;
            window.anchoredPosition = originalPosition;

            isSmall = false;
        }
    }
}
