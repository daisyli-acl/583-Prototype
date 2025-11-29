using UnityEngine;

public class SubPanelToggle : MonoBehaviour
{
    [SerializeField] private GameObject subPanel;

    private void Awake()
    {
        // Ensure the panel starts hidden
        if (subPanel != null)
            subPanel.SetActive(false);
    }

    public void TogglePanel()
    {
        if (subPanel == null) return;

        // Flip the active state
        bool isActive = subPanel.activeSelf;
        subPanel.SetActive(!isActive);
    }
}
