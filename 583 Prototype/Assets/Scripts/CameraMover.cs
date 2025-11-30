using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMover2D : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Checkpoint[] checkpoints;

    [Header("Level transition")]
    public bool loadNextByBuildIndex = true; // true: load next build index, false: use nextSceneName
    public string nextSceneName;             // used when loadNextByBuildIndex == false

    private int currentIndex = 0;
    private bool hasLoadedNextScene = false;

    void Update()
    {
        if (hasLoadedNextScene)
            return;

        // 所有 checkpoint 都走完了
        if (currentIndex >= checkpoints.Length)
        {
            Debug.Log($"[CameraMover2D] All checkpoints passed. currentIndex = {currentIndex}");
            HandleLevelComplete();
            return;
        }

        Checkpoint cp = checkpoints[currentIndex];
        if (cp == null)
        {
            Debug.LogWarning($"[CameraMover2D] Checkpoint at index {currentIndex} is null.");
            return;
        }

        // Move camera horizontally to checkpoint.x
        Vector3 target = new Vector3(
            cp.transform.position.x,
            transform.position.y,
            transform.position.z
        );

        if (transform.position.x != target.x)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );
            return;
        }

        // Activate checkpoint logic (loading bar, etc.)
        CheckpointActivator activator = cp.GetComponent<CheckpointActivator>();
        if (activator != null && !cp.isCleared)
        {
            activator.Activate();
        }

        // Wait until this checkpoint is cleared
        if (!cp.isCleared)
            return;

        Debug.Log($"[CameraMover2D] Checkpoint {currentIndex} cleared, move to next.");
        // Go to next checkpoint
        currentIndex++;
    }

    private void HandleLevelComplete()
    {
        if (hasLoadedNextScene)
            return;

        hasLoadedNextScene = true;

        if (loadNextByBuildIndex)
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = buildIndex + 1;
            int totalScenes = SceneManager.sceneCountInBuildSettings;

            if (nextIndex >= totalScenes)
            {
                Debug.LogError($"[CameraMover2D] Cannot load next scene by build index. " +
                               $"Current index = {buildIndex}, nextIndex = {nextIndex}, " +
                               $"sceneCountInBuildSettings = {totalScenes}. " +
                               $"Check Build Settings.");
                return;
            }

            Debug.Log($"[CameraMover2D] Loading next scene by build index: {nextIndex}");
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                Debug.Log($"[CameraMover2D] Loading next scene by name: {nextSceneName}");
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("[CameraMover2D] nextSceneName is empty and loadNextByBuildIndex is false. No scene loaded.");
            }
        }
    }
}
