using System.Collections.Generic;
using Pattern;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace PopupSystem
{
    public class Popup : Singleton<Popup>
    {
        private PopupUI _popupUI;

        private readonly Queue<PopupData> _popups = new();
        [HideInInspector] public bool isLoaded;

        private PopupUI PopupUI
        {
            get
            {
                if (!_popupUI)
                {
                    GameObject instance = Instantiate(Resources.Load<GameObject>("PopupUI"));
                    instance.name = "[ POPUP UI ]";
                    _popupUI = instance.GetComponent<PopupUI>();
                    _popupUI.onClose.AddListener(Hide);
                }

                return _popupUI;
            }
        }

        private void Prepare()
        {
            if (!isLoaded)
            {
                isLoaded = true;
                Time.timeScale = 0f;

                CheckForEventSystem();
            }
        }

        private static void CheckForEventSystem()
        {
            // Check if there is an EventSystem in the scene (if not, add one)
            EventSystem es = FindObjectOfType<EventSystem>();
            if (ReferenceEquals(es, null))
            {
                GameObject esGameObject = new GameObject("EventSystem");
                esGameObject.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
                esGameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
                esGameObject.AddComponent<StandaloneInputModule>();
#endif
            }
        }

        private void ShowNextPopup()
        {
            Prepare();
            var tempData = _popups.Dequeue();
            PopupUI.Setup(tempData.title, tempData.description, tempData.buttons[0], tempData.buttons[1]);
        }

        public void Show(string title, string description, PopupButtonData rightButton,
            PopupButtonData leftButton = null)
        {
            var enqueueData = new PopupData
            {
                title = title,
                description = description,
                buttons = new[] { rightButton, leftButton }
            };

            _popups.Enqueue(enqueueData);

            if (!PopupUI.IsVisible) ShowNextPopup();
        }

        public void Show(string title, string description, string rightButtonText = "Ok",
            PopupColor rightButtonColor = PopupColor.Green,
            UnityAction rightButtonAction = null, string leftButtonText = null,
            PopupColor leftButtonColor = PopupColor.Red,
            UnityAction leftButtonAction = null)
        {
            var rightButtonData = new PopupButtonData
            {
                buttonText = rightButtonText,
                buttonColor = rightButtonColor,
                onCloseAction = rightButtonAction
            };

            var leftButtonData = new PopupButtonData
            {
                buttonText = leftButtonText,
                buttonColor = leftButtonColor,
                onCloseAction = leftButtonAction
            };

            var enqueueData = new PopupData
            {
                title = title,
                description = description,
                buttons = new[] { rightButtonData, leftButtonData }
            };

            _popups.Enqueue(enqueueData);

            if (!PopupUI.IsVisible) ShowNextPopup();
        }

        public void Hide()
        {
            if (_popups.Count != 0)
                ShowNextPopup();
        }
    }

    public struct PopupData
    {
        public string title;
        public string description;
        public PopupButtonData[] buttons;
    }

    public class PopupButtonData
    {
        public string buttonText;
        public PopupColor buttonColor;
        public UnityAction onCloseAction;
    }
}