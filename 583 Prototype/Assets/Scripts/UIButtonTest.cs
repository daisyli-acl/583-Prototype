using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonTest : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("UI Click Works");
    }
}
