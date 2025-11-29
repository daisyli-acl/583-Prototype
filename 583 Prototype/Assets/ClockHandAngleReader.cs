using UnityEngine;

public class ClockHandAngleReader : MonoBehaviour
{
    public RectTransform pivot;   // HandPivot
    public float currentAngle;    // Readable angle (0–360)

    void Update()
    {
        // Unity stores rotation as 0–360 on the Z axis
        float rawAngle = pivot.eulerAngles.z;

        // Convert to traditional clock format (0° = pointing up)
        currentAngle = Mathf.Repeat(rawAngle + 90f, 360f);

        // Debug output (optional)
        // Debug.Log("Clock hand angle: " + currentAngle);
    }
}
