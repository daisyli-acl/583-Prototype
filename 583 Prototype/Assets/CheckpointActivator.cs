using UnityEngine;

public class CheckpointActivator : MonoBehaviour
{
    public DraggableLoadingBar loadingBar;
    private bool hasActivated = false;

    public void Activate()
    {
        if (hasActivated)
            return;

        hasActivated = true;

        if (loadingBar != null)
            loadingBar.ActivateBar();   // This now ENABLES the bar
    }
}
