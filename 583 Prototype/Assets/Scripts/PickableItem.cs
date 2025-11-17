using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public string itemId;
    [HideInInspector] public bool isHeld = false;

    private Vector3 offset;

    public void OnPickUp(Vector3 mouseWorldPos)
    {
        isHeld = true;
        offset = transform.position - mouseWorldPos;
    }

    public void FollowMouse(Vector3 mouseWorldPos)
    {
        if (isHeld)
            transform.position = mouseWorldPos + offset;
    }

    public void OnPlace(Transform target)
    {
        isHeld = false;
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
