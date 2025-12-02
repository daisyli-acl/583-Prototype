using UnityEngine;
using System.Collections.Generic;

public class SmoothProgressBarByCheckpoints : MonoBehaviour
{
    [Header("Checkpoints (in order)")]
    public List<Checkpoint> checkpoints;         // Drag checkpoints in order

    [Header("UI Elements")]
    public RectTransform manIcon;               // Tiny man
    public RectTransform barStart;              // Left end
    public RectTransform barEnd;                // Right end

    [Header("Animation Settings")]
    public float moveDuration = 0.5f;
    public AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Manual Skip Settings")]
    [Tooltip("After this many checkpoints are cleared, skip becomes available.")]
    public int unlockSkipAfterCleared = 1;

    private int clearedCount = 0;
    private bool isMoving = false;

    private bool skipUsed = false;   // ← IMPORTANT: skip can only be used once

    private void Update()
    {
        if (checkpoints == null || checkpoints.Count == 0)
            return;

        // Count cleared checkpoints in sequence
        int newClearedCount = 0;
        for (int i = 0; i < checkpoints.Count; i++)
        {
            if (checkpoints[i] != null && checkpoints[i].isCleared)
                newClearedCount++;
            else
                break;
        }

        // Automatically move for newly cleared checkpoints
        if (newClearedCount > clearedCount)
        {
            MoveManToStep(newClearedCount);
            clearedCount = newClearedCount;
        }
    }

    // -------- PLAYER MANUAL SKIP (ONE TIME ONLY) --------
    public void PlayerSkipOneStep()
    {
        // Must have reached unlock checkpoint
        if (clearedCount < unlockSkipAfterCleared)
            return;

        // Can only use skip once
        if (skipUsed)
            return;

        // Cannot skip past end
        if (clearedCount >= checkpoints.Count)
            return;

        skipUsed = true; // lock future use

        // Mark the next checkpoint cleared
        Checkpoint next = checkpoints[clearedCount];
        if (next != null)
            next.isCleared = true;

        // Move man to new step
        int newCleared = clearedCount + 1;
        MoveManToStep(newCleared);
        clearedCount = newCleared;
    }

    // -------- movement helpers --------

    private void MoveManToStep(int stepIndex)
    {
        stepIndex = Mathf.Clamp(stepIndex, 0, checkpoints.Count);

        float t = (float)stepIndex / checkpoints.Count;

        Vector2 startPos = barStart.anchoredPosition;
        Vector2 endPos = barEnd.anchoredPosition;
        Vector2 targetPos = Vector2.Lerp(startPos, endPos, t);

        StartCoroutine(SmoothMoveRoutine(targetPos));
    }

    private System.Collections.IEnumerator SmoothMoveRoutine(Vector2 targetPos)
    {
        if (isMoving) yield break;
        isMoving = true;

        Vector2 initialPos = manIcon.anchoredPosition;
        float time = 0f;

        while (time < moveDuration)
        {
            float t = easing.Evaluate(time / moveDuration);
            manIcon.anchoredPosition = Vector2.Lerp(initialPos, targetPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        manIcon.anchoredPosition = targetPos;
        isMoving = false;
    }
}
