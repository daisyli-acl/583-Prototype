using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WindowAutoPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 200f;      // pixels per second in UI space
    public float edgePadding = 10f;     // padding inside content area
    public float obstaclePadding = 5f;  // extra distance before hitting obstacle

    [Header("Animation")]
    public Sprite[] walkFrames;         // 4 walking frames
    public float frameRate = 8f;        // frames per second


    // public float moveThreshold = 0.1f; 

    [Header("Current window")]
    public WindowNode currentWindow;

    [Header("Ending")]
    [Tooltip("End zone inside the FINAL window's Content. Player entering this zone triggers the ending.")]
    public RectTransform finalEndZone;

    public float endZoomDuration = 1.5f;

    private RectTransform playerRect;
    private Image image;
    private bool endingStarted = false;

    private int currentFrame = 0;
    private float animTimer = 0f;
    private Vector2 lastPosition;

    private void Awake()
    {
        playerRect = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        if (walkFrames != null && walkFrames.Length > 0 && image != null)
        {
            image.sprite = walkFrames[0];
        }

        if (playerRect != null)
        {
            lastPosition = playerRect.anchoredPosition;
        }
    }

    private void Start()
    {
        if (currentWindow == null)
        {
            WindowNode wn = GetComponentInParent<WindowNode>();
            if (wn != null)
                currentWindow = wn;
        }
    }

    private void Update()
    {
        if (currentWindow == null || endingStarted)
            return;

        AutoMoveInsideWindow();
        UpdateWalkAnimation();
        TryTriggerEnding();
    }

    private void AutoMoveInsideWindow()
    {
        RectTransform content = currentWindow.contentRect;
        if (content == null || playerRect == null)
            return;

        Rect r = content.rect;
        float halfW = r.width * 0.5f;
        float halfH = r.height * 0.5f;

        Vector2 pos = playerRect.anchoredPosition;
        float deltaX = moveSpeed * Time.deltaTime;
        float targetX = pos.x + deltaX;


        targetX = ApplyObstacleBlocking(pos.x, targetX);

        WindowNode next = currentWindow.nextWindow;
        bool canConnectToNext = currentWindow.IsCorrectlyConnectedToNext();


        if (next != null && canConnectToNext && targetX > halfW - edgePadding)
        {
            EnterNextWindow(next);
            return;
        }

        float minX = -halfW + edgePadding;
        float maxX = halfW - edgePadding;
        float minY = -halfH + edgePadding;
        float maxY = halfH - edgePadding;

        pos.x = Mathf.Clamp(targetX, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        playerRect.anchoredPosition = pos;
    }

    private float ApplyObstacleBlocking(float currentX, float targetX)
    {
        if (currentWindow.blockingObstacles == null || currentWindow.blockingObstacles.Length == 0)
            return targetX;

        if (playerRect == null)
            return targetX;

        float deltaX = targetX - currentX;
        if (deltaX <= 0f)
            return targetX;

        Vector2 pSize = playerRect.rect.size;
        Vector3 pScale = playerRect.localScale;
        float pHalfW = pSize.x * Mathf.Abs(pScale.x) * 0.5f;

        foreach (RectTransform obst in currentWindow.blockingObstacles)
        {
            if (obst == null || !obst.gameObject.activeInHierarchy)
                continue;

            Vector2 oSize = obst.rect.size;
            Vector3 oScale = obst.localScale;
            float oHalfW = oSize.x * Mathf.Abs(oScale.x) * 0.5f;

            float obstCenterX = obst.anchoredPosition.x;
            float obstLeft = obstCenterX - oHalfW;

            float playerRightNow = currentX + pHalfW;
            float playerRightNext = targetX + pHalfW;

            if (playerRightNow <= obstLeft && playerRightNext > obstLeft)
            {
                float stopX = obstLeft - pHalfW - obstaclePadding;
                targetX = Mathf.Min(targetX, stopX);
            }
        }

        return targetX;
    }

    private void EnterNextWindow(WindowNode next)
    {
        if (next == null || next.contentRect == null || playerRect == null)
            return;

        Rect rCur = currentWindow.contentRect.rect;
        float curHalfH = rCur.height * 0.5f;

        Vector2 pos = playerRect.anchoredPosition;
        float tY = (pos.y - (-curHalfH)) / rCur.height;
        tY = Mathf.Clamp01(tY);

        RectTransform nextContent = next.contentRect;
        playerRect.SetParent(nextContent, worldPositionStays: false);

        Rect rNext = nextContent.rect;
        float nextHalfW = rNext.width * 0.5f;
        float nextHalfH = rNext.height * 0.5f;

        float newY = Mathf.Lerp(-nextHalfH + edgePadding, nextHalfH - edgePadding, tY);
        float newX = -nextHalfW + edgePadding;

        playerRect.anchoredPosition = new Vector2(newX, newY);

        currentWindow = next;
        lastPosition = playerRect.anchoredPosition;
    }

    private void UpdateWalkAnimation()
    {
        if (image == null || walkFrames == null || walkFrames.Length == 0 || playerRect == null)
            return;

        Vector2 currentPos = playerRect.anchoredPosition;
        float distance = Vector2.Distance(currentPos, lastPosition);


        bool isMoving = false;
        if (Time.deltaTime > 0f)
        {
            float currentSpeed = distance / Time.deltaTime;
            isMoving = currentSpeed > 1f;
        }
        else
        {
            isMoving = distance > 0.001f;
        }


        if (!isMoving)
        {
            currentFrame = 0;
            image.sprite = walkFrames[0];
            animTimer = 0f;
            lastPosition = currentPos;
            return;
        }

        animTimer += Time.deltaTime;
        float frameTime = 1f / frameRate;

        if (animTimer >= frameTime)
        {
            animTimer -= frameTime;
            currentFrame++;
            if (currentFrame >= walkFrames.Length)
                currentFrame = 0;

            image.sprite = walkFrames[currentFrame];
        }

        lastPosition = currentPos;
    }

    private void TryTriggerEnding()
    {
        if (!currentWindow.isFinalWindow || endingStarted)
            return;

        if (finalEndZone == null || playerRect == null)
            return;

        if (IsPlayerInsideZone(finalEndZone))
        {
            StartCoroutine(PlayEndZoom());
        }
    }

    private bool IsPlayerInsideZone(RectTransform zoneRect)
    {
        Vector3[] pCorners = new Vector3[4];
        Vector3[] zCorners = new Vector3[4];

        playerRect.GetWorldCorners(pCorners);
        zoneRect.GetWorldCorners(zCorners);

        Rect pRect = Rect.MinMaxRect(
            pCorners[0].x, pCorners[0].y,
            pCorners[2].x, pCorners[2].y
        );
        Rect zRect = Rect.MinMaxRect(
            zCorners[0].x, zCorners[0].y,
            zCorners[2].x, zCorners[2].y
        );

        return pRect.Overlaps(zRect);
    }

    private IEnumerator PlayEndZoom()
    {
        endingStarted = true;

        RectTransform wRect = currentWindow.windowRect;
        if (wRect == null)
            yield break;

        Vector3 startScale = wRect.localScale;
        Vector2 startPos = wRect.anchoredPosition;

        Vector3 endScale = Vector3.one;
        Vector2 endPos = Vector2.zero;

        float t = 0f;
        while (t < endZoomDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / endZoomDuration);

            wRect.localScale = Vector3.Lerp(startScale, endScale, k);
            wRect.anchoredPosition = Vector2.Lerp(startPos, endPos, k);

            yield return null;
        }

        wRect.localScale = endScale;
        wRect.anchoredPosition = endPos;

        Debug.Log("Ending reached: here you can show final UI or load ending scene.");
    }
}