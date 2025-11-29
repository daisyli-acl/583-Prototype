using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Attach this to the object you want to catch clicks.
/// </summary>
#region License
/*
View the full license here: https://creativecommons.org/licenses/by/4.0/

In summary, you may do as you wish with it, so long as you credit me the original author. 
Using my bussiness tag "Digineaux" or preferably my website URL "Digineaux.com" in the credits of a finished software/game or 
as a script comment or similar in a modified script you are reselling is sufficent.

In regards to tracking changes made, I am not strict. A summary as short as a sentence or a changelog is fine.
*/
#endregion

namespace Digineaux
{
    public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Tooltip("What transform you want to move when dragging button. Defaults to this transform if null")]
        public Transform target;

        [Tooltip("When this is pressed the draggable will be released and returned to the position it started. Defaults to escape")]
        public KeyCode resetButton = KeyCode.Escape;

        [Tooltip("If true then it will release the target when the mouse button is released. If false then it will release when this object is clicked again")]
        public bool holdKey;

        bool dragging;
        Vector3 offset, originalPos;

        private void OnEnable()
        {
            if (target == null) { target = transform; }
        }
        private void Update()
        {
            if (dragging == false) { return; }

            //reset and release object when reset key clicked
            if (Input.GetKeyDown(resetButton))
            {
                target.position = originalPos;
                dragging = false;
                return;
            }

            //move target to mouse
            Vector3 newPos = GetCanvasRelativePos(Input.mousePosition);
            target.position = newPos + offset;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (holdKey == false && dragging == true) { dragging = false; return; }
            dragging = true;
            originalPos = target.position;
            offset = target.position - GetCanvasRelativePos(Input.mousePosition);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (holdKey == false) { return; }
            dragging = false;
        }


        void ResetPos()
        {
            target.position = originalPos;
            dragging = false;
        }

        Vector3 GetCanvasRelativePos(Vector3 basePos)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                Camera cam = canvas.worldCamera;
                Plane plane = new Plane(cam.transform.forward, target.position);
                Ray ray = cam.ScreenPointToRay(basePos);
                if (plane.Raycast(ray, out float enter))
                {
                    Vector3 worldPos = ray.GetPoint(enter);
                    return worldPos;
                }
            }
            return basePos;
        }
    }
}
