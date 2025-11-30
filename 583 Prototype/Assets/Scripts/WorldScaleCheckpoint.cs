using UnityEngine;

public class WorldScaleCheckpoint : MonoBehaviour
{
    [Header("Target world object to scale")]
    public Transform targetObject;

    [Header("Scale settings")]
    public float minScale = 1f;        // horizontal scale X min
    public float maxScale = 2f;        // horizontal scale X max
    public float requiredScale = 1.8f; // horizontal scale X required to clear
    public float dragSensitivity = 0.01f; // scale per pixel of drag

    [Header("Optional animation to stop when done")]
    public Animator targetAnimator;

    [Header("Checkpoint to clear when finished")]
    public Checkpoint checkpointToClear;

    [Header("Renderer used to compute left edge (auto found if null)")]
    public Renderer boundsRenderer;

    private bool isDragging = false;
    private bool isCompleted = false;
    private float currentScaleX;
    private Vector3 lastMousePosition;

    private float originalScaleY;
    private float originalScaleZ;
    private float leftEdgeWorldX;

    private void Start()
    {
        if (targetObject == null)
            targetObject = transform;

        if (boundsRenderer == null)
            boundsRenderer = targetObject.GetComponentInChildren<Renderer>();

        Vector3 s = targetObject.localScale;
        currentScaleX = s.x;
        originalScaleY = s.y;
        originalScaleZ = s.z;

        currentScaleX = Mathf.Clamp(currentScaleX, minScale, maxScale);
        targetObject.localScale = new Vector3(currentScaleX, originalScaleY, originalScaleZ);

        if (boundsRenderer != null)
        {
            leftEdgeWorldX = boundsRenderer.bounds.min.x;
        }
        else
        {
            leftEdgeWorldX = targetObject.position.x;
        }
    }

    private void OnMouseDown()
    {
        if (isCompleted) return;

        isDragging = true;
        lastMousePosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void OnMouseDrag()
    {
        if (!isDragging || isCompleted || targetObject == null)
            return;

        Vector3 mousePos = Input.mousePosition;
        float deltaX = mousePos.x - lastMousePosition.x;

        currentScaleX += deltaX * dragSensitivity;
        currentScaleX = Mathf.Clamp(currentScaleX, minScale, maxScale);

        // Only scale horizontally, keep vertical scale unchanged
        targetObject.localScale = new Vector3(currentScaleX, originalScaleY, originalScaleZ);

        // Re-anchor left edge to its original world X
        if (boundsRenderer != null)
        {
            Bounds b = boundsRenderer.bounds;
            float currentLeft = b.min.x;
            float offset = leftEdgeWorldX - currentLeft;
            targetObject.position += new Vector3(offset, 0f, 0f);
        }

        lastMousePosition = mousePos;

        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (isCompleted)
            return;

        if (currentScaleX >= requiredScale)
        {
            isCompleted = true;

            if (targetAnimator != null)
                targetAnimator.enabled = false;

            if (checkpointToClear != null)
                checkpointToClear.isCleared = true;
        }
    }
}
