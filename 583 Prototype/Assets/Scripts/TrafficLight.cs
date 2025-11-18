using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [Header("Light Sprites")]
    public GameObject redLight;
    public GameObject greenLight;

    [Header("Optional: Set a checkpoint when turning green")]
    public Checkpoint checkpointToClear;

    // Start with RED on
    void Start()
    {
        SetRed();
    }

    public void SetRed()
    {
        redLight.SetActive(true);
        greenLight.SetActive(false);
    }

    public void SetGreen()
    {
        redLight.SetActive(false);
        greenLight.SetActive(true);

        // Mark checkpoint as cleared if attached
        if (checkpointToClear != null)
            checkpointToClear.isCleared = true;
    }
}
