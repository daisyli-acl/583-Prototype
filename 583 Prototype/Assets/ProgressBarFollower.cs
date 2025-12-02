using UnityEngine;
using System.Collections.Generic;

public class ProgressBarFollower : MonoBehaviour
{
    [Header("Checkpoints (in order)")]
    public List<Checkpoint> checkpoints;         // Drag checkpoint objects in order

    [Header("UI Elements")]
    public RectTransform manIcon;               // The tiny man
    public RectTransform barStart;               // Left edge of bar
    public RectTransform barEnd;                 // Right edge of bar

    [Header("Animation Settings")]
    public float moveDuration = 0.5f;            // Time for smooth slide
    public AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Debug / Read Only")]
    public int clearedCount = 0;

    private bool isMoving = false;

    private void Update()
    {
        if (checkpoints == null || checkpoints.Count == 0)
            return;

        // count how many checkpoints are cleared
        int newClearedCount = 0;
        for (int i = 0; i < checkpoints.Count; i++)
        {
            if (checkpoints[i] != null && checkpoints[i].isCleared)
                newClearedCount++;
            else
                break;
        }

        // detect when a NEW checkpoint gets cleared
        if (newClearedCount > clearedCount)
        {
            int stepsToAdvance = newClearedCount - clearedCount;

            // Move man forward for each newly cleared checkpoint
            for (int i = 0; i < stepsToAdvance; i++)
            {
                MoveManToStep(clearedCount + i + 1);
            }

            clearedCount = newClearedCount;
        }
    }

    private void MoveManToStep(int stepIndex)
    {
        if (stepIndex < 0) stepIndex = 0;
        if (stepIndex > checkpoints.Count) stepIndex = checkpoints.Count;

        float t = (float)stepIndex / checkpoints.Count;

        Vector2 startPos = barStart.anchoredPosition;
        Vector2 endPos = barEnd.anchoredPosition;
        Vector2 targetPos = Vector2.Lerp(startPos, endPos, t);

        // Start smooth movement
        StartCoroutine(SmoothMoveRoutine(targetPos));
    }

    private System.Collections.IEnumerator SmoothMoveRoutine(Vector2 targetPos)
    {
        if (isMoving) yield break;      // prevent overlapping motions
        isMoving = true;

        Vector2 initialPos = manIcon.anchoredPosition;
        float time = 0f;

        while (time < moveDuration)
        {
            float t = time / moveDuration;
            t = easing.Evaluate(t);

            manIcon.anchoredPosition = Vector2.Lerp(initialPos, targetPos, t);

            time += Time.deltaTime;
            yield return null;
        }

        manIcon.anchoredPosition = targetPos;

        isMoving = false;
    }
}
