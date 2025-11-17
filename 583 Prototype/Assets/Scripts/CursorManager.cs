using UnityEngine;

public enum CursorType
{
    None,
    Normal,
    HoverInteract,
    Holding,
    HoverDrop
}

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    public Texture2D normalCursor;
    public Texture2D hoverInteractCursor;
    public Texture2D holdingCursor;
    public Texture2D hoverDropCursor;

    public Vector2 hotSpot = Vector2.zero;

    private CursorType currentType = CursorType.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SetNormal();
    }

    private void SetCursor(Texture2D texture, CursorType type)
    {
        if (currentType == type) return;

        currentType = type;
        Cursor.SetCursor(texture, hotSpot, CursorMode.Auto);
    }

    public void SetNormal() => SetCursor(normalCursor, CursorType.Normal);
    public void SetHoverInteract() => SetCursor(hoverInteractCursor, CursorType.HoverInteract);
    public void SetHolding() => SetCursor(holdingCursor, CursorType.Holding);
    public void SetHoverDrop() => SetCursor(hoverDropCursor, CursorType.HoverDrop);
}
