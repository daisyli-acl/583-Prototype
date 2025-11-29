using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleObjectOnClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject targetObject;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (targetObject != null)
            targetObject.SetActive(!targetObject.activeSelf);
    }
}
