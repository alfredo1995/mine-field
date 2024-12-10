using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerOutside : MonoBehaviour
{
    public Canvas canvas;
    private GraphicRaycaster _graphicRaycaster;
    public GameObject ignoreButton;
    public RectTransform focusObject;

    public bool useIgnoreButton = true;
    
    [SerializeField] public UnityEvent onBlurAllEvents;

    private void Awake()
    {
        _graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
    }

    private void OnGUI()
    {
        if(!focusObject.gameObject.activeSelf) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            
            var results = new List<RaycastResult>();
            _graphicRaycaster.Raycast(eventData, results);

            var clickedOnPopup = false;
            
            foreach (var result in results)
            {
                if (result.gameObject == focusObject.gameObject || (result.gameObject == ignoreButton && useIgnoreButton))
                {
                    clickedOnPopup = true;
                    break;
                }
            }

            if (!clickedOnPopup) onBlurAllEvents?.Invoke();
        }
    }
}
