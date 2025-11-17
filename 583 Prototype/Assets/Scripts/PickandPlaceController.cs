using UnityEngine;

public class PickAndPlaceController : MonoBehaviour
{
    public LayerMask interactableLayer;
    public LayerMask dropLayer;

    private PickableItem heldItem = null;

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        if (heldItem == null)
            HandleHoverAndPick(mousePos2D, mouseWorldPos);
        else
            HandleHoverAndDrop(mousePos2D, mouseWorldPos);
    }

    private void HandleHoverAndPick(Vector2 mousePos2D, Vector3 mouseWorldPos)
    {
        CursorManager.Instance.SetNormal();

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0f, interactableLayer);
        if (hit.collider != null)
        {
            PickableItem item = hit.collider.GetComponent<PickableItem>();
            if (item != null)
            {
                CursorManager.Instance.SetHoverInteract();

                if (Input.GetMouseButtonDown(0))
                {
                    heldItem = item;
                    heldItem.OnPickUp(mouseWorldPos);
                    CursorManager.Instance.SetHolding();
                }
            }
        }
    }

    private void HandleHoverAndDrop(Vector2 mousePos2D, Vector3 mouseWorldPos)
    {
        heldItem.FollowMouse(mouseWorldPos);

        // First, check if we are over a valid drop slot.
        Collider2D col = Physics2D.OverlapPoint(mousePos2D, dropLayer);
        if (col != null)
        {
            DropSlot slot = col.GetComponent<DropSlot>();
            if (slot != null && slot.CanDrop(heldItem))
            {
                CursorManager.Instance.SetHoverDrop();

                if (Input.GetMouseButtonDown(0))
                {
                    slot.PlaceItem(heldItem);
                    heldItem = null;
                    CursorManager.Instance.SetNormal();
                }

                return;
            }
        }

        // If not over a valid slot, stay in holding state.
        CursorManager.Instance.SetHolding();
    }
}
