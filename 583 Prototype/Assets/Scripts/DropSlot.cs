using UnityEngine;

public class DropSlot : MonoBehaviour
{
    [Header("Which item can be placed here")]
    public string acceptedItemId;
    public Transform placePoint;

    [Header("Checkpoint linked to this obstacle")]
    public Checkpoint checkpointToClear;

    public bool CanDrop(PickableItem item)
    {
        return item != null && item.itemId == acceptedItemId;
    }

    public void PlaceItem(PickableItem item)
    {
        if (item == null) return;

        item.OnPlace(placePoint);

        // Mark checkpoint as cleared so camera can continue
        if (checkpointToClear != null)
            checkpointToClear.isCleared = true;
    }
}
