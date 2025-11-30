using UnityEngine;

public class WindowNode : MonoBehaviour
{
    [Header("Window & content area")]
    public RectTransform windowRect;   // whole window
    public RectTransform contentRect;  // inner area for player movement

    [Header("Order & linking")]
    public int sequenceIndex;          // e.g. 0,1,2,...
    public WindowNode nextWindow;      // logical next window in the chain
    public bool isFinalWindow = false; // mark the last window

    [Header("Blocking obstacles in this window (optional, UI RectTransforms)")]
    public RectTransform[] blockingObstacles;

    [Header("Connection rules")]
    [Tooltip("Minimum vertical overlap height (in screen pixels) to count as connected.")]
    public float minVerticalOverlap = 40f;

    [Tooltip("Maximum horizontal depth (in pixels) that next window can overlap into this window.")]
    public float maxHorizontalOverlap = 80f;

    [Header("Small window detection")]
    [Tooltip("If current scale.x <= originalScale.x * this, we treat it as SMALL mode.")]
    public float smallScaleThreshold = 0.9f;

    private Vector3 originalScale;
    private bool originalRecorded = false;

    private void Awake()
    {
        if (windowRect == null)
            windowRect = GetComponent<RectTransform>();

        if (contentRect == null)
        {
            Transform t = transform.Find("Content");
            if (t != null)
                contentRect = t.GetComponent<RectTransform>();
            else
                contentRect = windowRect;
        }

        if (windowRect != null)
        {
            originalScale = windowRect.localScale;
            originalRecorded = true;
        }
    }

    public Rect GetWindowScreenRect()
    {
        Vector3[] corners = new Vector3[4];
        windowRect.GetWorldCorners(corners);
        float xMin = corners[0].x;
        float yMin = corners[0].y;
        float xMax = corners[2].x;
        float yMax = corners[2].y;
        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }

    public bool IsSmallNow()
    {
        if (!originalRecorded || windowRect == null)
            return false;

        float currentX = windowRect.localScale.x;
        float originalX = originalScale.x;

        if (originalX <= 0f)
            return false;

        return currentX <= originalX * smallScaleThreshold;
    }

    /// <summary>
    /// Only returns true when:
    /// 1) both this window and nextWindow are in SMALL state
    /// 2) they overlap vertically enough
    /// 3) this window's RIGHT side overlaps with nextWindow's LEFT side a little bit
    /// </summary>
    public bool IsCorrectlyConnectedToNext()
    {
        if (nextWindow == null || windowRect == null)
            return false;

        if (!IsSmallNow() || !nextWindow.IsSmallNow())
            return false;

        Rect a = GetWindowScreenRect();             // current window
        Rect b = nextWindow.GetWindowScreenRect();  // next window

        float overlapHeight = Mathf.Min(a.yMax, b.yMax) - Mathf.Max(a.yMin, b.yMin);
        if (overlapHeight < minVerticalOverlap)
            return false;

        float diff = b.xMin - a.xMax;

        if (diff > 0f)
            return false;

        if (Mathf.Abs(diff) > maxHorizontalOverlap)
            return false;

        return true;
    }
}
