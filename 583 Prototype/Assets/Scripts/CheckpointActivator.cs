using UnityEngine;

public class CheckpointActivator : MonoBehaviour
{
    public DraggableLoadingBar loadingBar;
    public Checkpoint checkpoint;

    private bool hasActivated = false;
    private bool disableStarted = false;

    public void Activate()
    {
        if (hasActivated)
            return;

        hasActivated = true;

        // Turn on or activate loading bar
        if (loadingBar != null)
            loadingBar.ActivateBar();

        // If checkpoint already cleared, start disabling immediately
        if (checkpoint != null && checkpoint.isCleared)
            StartDisableTimer();
    }

    void Update()
    {
        // If activated but not yet disabled AND checkpoint becomes cleared
        if (hasActivated && !disableStarted && checkpoint != null && checkpoint.isCleared)
        {
            StartDisableTimer();
        }
    }

    private void StartDisableTimer()
    {
        disableStarted = true;
        StartCoroutine(DisableBarAfterDelay());
    }

    private System.Collections.IEnumerator DisableBarAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        if (loadingBar != null)
            loadingBar.gameObject.SetActive(false);
    }
}
