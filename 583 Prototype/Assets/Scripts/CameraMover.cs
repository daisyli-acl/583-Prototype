using UnityEngine;

public class CameraMover2D : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Checkpoint[] checkpoints;

    private int currentIndex = 0;

    void Update()
    {
        if (currentIndex >= checkpoints.Length)
            return;

        Checkpoint cp = checkpoints[currentIndex];

        // Target X position only
        Vector3 target = new Vector3(
            cp.transform.position.x,
            transform.position.y,
            transform.position.z
        );

        // If we have NOT reached the checkpoint yet → KEEP MOVING
        if (transform.position.x != target.x)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            return; // still moving, no need to check clearing
        }

        // We reached the checkpoint

        // Trigger loading bar activation (ONLY once when arriving)
        CheckpointActivator activator = cp.GetComponent<CheckpointActivator>();
        if (activator != null && !cp.isCleared)
        {
            activator.Activate();
        }

        // WAIT here until checkpoint is cleared
        if (!cp.isCleared)
            return;

        // Continue to next checkpoint
        currentIndex++;
    }
}
