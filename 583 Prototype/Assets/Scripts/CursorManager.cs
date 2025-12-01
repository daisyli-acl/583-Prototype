using UnityEngine;
using UnityEngine.SceneManagement; 

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


    [Header("Settings")]
    public string[] activeScenes = { "Level1", "Level2" };

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


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isAllowed = false;


        foreach (string allowedName in activeScenes)
        {
            if (scene.name == allowedName)
            {
                isAllowed = true;
                break;
            }
        }


        if (!isAllowed)
        {

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            Debug.Log($"[CursorManager] Scene '{scene.name}' not in active list. Destroying CursorManager.");
            Destroy(gameObject);
        }
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