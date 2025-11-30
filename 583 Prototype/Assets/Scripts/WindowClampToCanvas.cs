using UnityEngine;

public class WindowClampToCanvas : MonoBehaviour
{
    [Header("Window to clamp")]
    public RectTransform windowRect;

    [Header("Bounds (usually the root canvas rect)")]
    public RectTransform boundsRect;

    [Header("How many pixels of the window must stay visible inside the screen")]
    public float visibleMargin = 40f;

    private void Awake()
    {
        if (windowRect == null)
            windowRect = GetComponent<RectTransform>();

        if (boundsRect == null)
        {
            // Try to find the parent canvas as bounds
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                boundsRect = canvas.GetComponent<RectTransform>();
            }
        }
    }

    private void LateUpdate()
    {
        if (windowRect == null || boundsRect == null)
            return;

        // This assumes the window pivot is roughly at (0.5, 0.5)
        // and boundsRect is the full screen / canvas area.
        Vector2 parentSize = boundsRect.rect.size;

        // Take scaling into account when computing window size
        Vector2 rawSize = windowRect.rect.size;
        Vector3 scale = windowRect.localScale;

        float windowWidth = rawSize.x * Mathf.Abs(scale.x);
        float windowHeight = rawSize.y * Mathf.Abs(scale.y);

        float halfParentW = parentSize.x * 0.5f;
        float halfParentH = parentSize.y * 0.5f;

        float halfWinW = windowWidth * 0.5f;
        float halfWinH = windowHeight * 0.5f;

        Vector2 pos = windowRect.anchoredPosition;

        // We want at least "visibleMargin" pixels of the window to remain inside bounds.
        // So we clamp the center position so that:
        // - left edge <= right bound - visibleMargin
        // - right edge >= left bound + visibleMargin
        float minX = -halfParentW + visibleMargin - halfWinW;
        float maxX = halfParentW - visibleMargin + halfWinW;

        float minY = -halfParentH + visibleMargin - halfWinH;
        float maxY = halfParentH - visibleMargin + halfWinH;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        windowRect.anchoredPosition = pos;
    }
}
