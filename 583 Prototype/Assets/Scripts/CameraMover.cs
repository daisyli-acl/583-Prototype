using UnityEngine;

public class CameraMover2D : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Checkpoint[] checkpoints;

    private int currentIndex = 0;

    void Update()
    {
        if (currentIndex >= checkpoints.Length)
            return; // No more checkpoints

        // Target position (X only, keep camera Y/Z)
        Vector3 target = new Vector3(
            checkpoints[currentIndex].transform.position.x,
            transform.position.y,
            transform.position.z
        );

        // Move horizontally toward the checkpoint
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        // Check if we reached the checkpoint
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            currentIndex++;
        }
    }
}
