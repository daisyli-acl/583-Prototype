using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClockHandButtonHoldRotateSteps : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Setup")]
    public RectTransform pivot;               // The clock hand pivot (HandPivot)
    public float rotationSpeed = 90f;         // Degrees per second (positive = clockwise)

    [Header("Checkpoint Requirement (optional)")]
    public Checkpoint requiredCheckpoint;     // Must be cleared to allow rotation

    [Header("Checkpoint On Complete (optional)")]
    public Checkpoint completionCheckpoint;   // Will be set when 360° is reached

    [Header("Step Settings")]
    public int stepCount = 5;                 // 360° / 5 = 72° per step
    public List<GameObject> stepObjects;      // Objects to enable, in order

    [Header("Debug Info (Read Only)")]
    public float totalRotatedDegrees;         // How much it has rotated in total
    public int currentStepIndex = 0;          // 0..stepCount

    private bool rotating = false;
    private float stepSize;                   // Degrees per step
    private bool completionTriggered = false; // Only fire completion once

    private void Awake()
    {
        if (pivot == null)
            pivot = transform as RectTransform;

        stepSize = 360f / Mathf.Max(1, stepCount);
    }

    private void Start()
    {
        // Optional: make sure all step objects start disabled
        if (stepObjects != null)
        {
            foreach (var obj in stepObjects)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (!rotating)
            return;

        // Gate: require checkpoint to be cleared (if assigned)
        if (requiredCheckpoint != null && !requiredCheckpoint.isCleared)
            return;

        // Stop if already fully rotated
        if (totalRotatedDegrees >= 360f)
        {
            rotating = false;

            // Trigger completion checkpoint once
            if (!completionTriggered && completionCheckpoint != null)
            {
                completionCheckpoint.isCleared = true;
                completionTriggered = true;
            }

            return;
        }

        // How much we want to rotate *this frame*
        float deltaAngle = rotationSpeed * Time.deltaTime;

        // Clamp so we never go past exactly 360°
        float remaining = 360f - totalRotatedDegrees;
        float deltaThisFrame = Mathf.Min(Mathf.Abs(deltaAngle), remaining);
        deltaThisFrame *= Mathf.Sign(deltaAngle);

        // Apply rotation (negative Z = clockwise)
        pivot.Rotate(0f, 0f, -deltaThisFrame);
        totalRotatedDegrees += Mathf.Abs(deltaThisFrame);

        // Step logic
        int newStepIndex = Mathf.FloorToInt(totalRotatedDegrees / stepSize);

        if (newStepIndex > currentStepIndex)
        {
            for (int i = currentStepIndex; i < newStepIndex; i++)
            {
                int objIndex = i;
                if (stepObjects != null &&
                    objIndex >= 0 && objIndex < stepObjects.Count &&
                    stepObjects[objIndex] != null)
                {
                    stepObjects[objIndex].SetActive(true);
                }
            }

            currentStepIndex = newStepIndex;
        }

        // If we just reached exactly 360 this frame, trigger completion
        if (totalRotatedDegrees >= 360f && !completionTriggered)
        {
            if (completionCheckpoint != null)
                completionCheckpoint.isCleared = true;

            completionTriggered = true;
            rotating = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (totalRotatedDegrees >= 360f)
            return; // already finished

        rotating = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rotating = false;
    }

    // Optional reset if you ever need to reuse the puzzle
    public void ResetRotationAndSteps()
    {
        totalRotatedDegrees = 0f;
        currentStepIndex = 0;
        completionTriggered = false;

        if (pivot != null)
            pivot.localRotation = Quaternion.identity;

        if (stepObjects != null)
        {
            foreach (var obj in stepObjects)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        rotating = false;
    }
}
