using UnityEngine;
using UnityEngine.EventSystems;

public class UIObstacleClick : MonoBehaviour, IPointerClickHandler
{
    [Header("Hide this obstacle when clicked")]
    public bool hideOnClick = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (hideOnClick)
        {
            gameObject.SetActive(false);
        }

        Debug.Log("[UIObstacleClick] Obstacle clicked and hidden: " + gameObject.name);
    }
}
