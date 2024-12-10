using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Components.Keyboard
{
    //For the keyboard to work properly with ValueInputField both UpdateOnEndEdit and UpdateOnDeselect must be false
    public class KeyBoard : MonoBehaviour
    {
        public static KeyBoard instance;
        
        private RectTransform rectTransform;
        private Vector2 basePosition;
        [SerializeField] private GameObject keyboard;
        [SerializeField] private List<Button> charButtons;
        [SerializeField] private Button submit, backSpace;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private KeyBoardEvent keyBoardEvent;
        private TMP_InputField submitInputField;

        private string _cacheText;
        private bool _wasDigit;
        private bool _isOpen;

        private void Awake()
        {
            if (instance != this)
            {
                if (instance == null)
                {
                    instance = this;
                    Initialize();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        private void OnDisable()
        {
            submit.onClick.RemoveAllListeners();
            backSpace.onClick.RemoveAllListeners();
            keyBoardEvent.OnKeyDown -= HideWithDot;

            foreach (var t in charButtons)
                t.onClick.RemoveAllListeners();
        }

        private void Initialize()
        {
            rectTransform = (RectTransform)keyboard.transform;
            basePosition = rectTransform.anchoredPosition;
            var buttonsCount = charButtons.Count;

            for (var i = 0; i < buttonsCount - 2; i++)
            {
                var text = charButtons[i].GetComponentInChildren<TMP_Text>();
                var buttonText = (i + 1).ToString();
                text.SetText(buttonText);

                var c = text.text[0];
                charButtons[i].onClick.AddListener(() => ReturnedFromButton(c));
            }

            charButtons[buttonsCount - 2].onClick.AddListener(() => ReturnedFromButton('0'));

            if (PlayerPrefs.GetString("lang", "en").Equals("en"))
            {
                charButtons[buttonsCount - 1].onClick.AddListener(() => ReturnedFromButton('.'));
            }
            else
            {
                charButtons[buttonsCount - 1].onClick.AddListener(() => ReturnedFromButton(','));
            }

            submit.onClick.AddListener(Submit);
            backSpace.onClick.AddListener(Backspace);
            
            keyBoardEvent.OnKeyDown += HideWithDot;
        }

        private void SetUp(TMP_InputField field)
        {
            if(field == inputField) return;
            
            submitInputField = field;
        }

        private void ReturnedFromButton(char c)
        {
            if (inputField.text.Length >= inputField.characterLimit) return;
            inputField.text += c;
            inputField.caretPosition = inputField.text.Length;
            _wasDigit = true;
        }

        private void Submit()
        {
            if(!inputField)return;
            SetCacheOnInputField();
            inputField.onEndEdit.Invoke(inputField.text);
            inputField.onSubmit.Invoke(inputField.text);
            submitInputField.onEndEdit.Invoke(inputField.text);
            submitInputField.onSubmit.Invoke(inputField.text);
            CloseKeyboard();
        }

        private void Backspace()
        {
            if (inputField.text.Length <= 0) return;
            var text = inputField.text;
            inputField.text = text.Remove(text.Length - 1);
            CenterInputField();
        }

        public void Show(Vector2? position = null, TMP_InputField targetField = null)
        {
            if (_isOpen) return;

            if (targetField && targetField != inputField) SetUp(targetField);

            _isOpen = true;
            _wasDigit = false;
            _cacheText = submitInputField.text;
            inputField.text = string.Empty;
            keyboard.SetActive(_isOpen);
        }

        private void HideWithDot()
        {
            Submit();
        }

        public void Hide()
        {
            if (!_isOpen) return;
            if (inputField.text.Length > 0)
                _wasDigit = true;
            CloseKeyboard();
            CenterInputField();
        }

        private void SetCacheOnInputField()
        {
            if (!string.IsNullOrEmpty(inputField.text)) return;
            inputField.text = _cacheText;
        }

        private void CenterInputField()
        {
            inputField.textComponent.rectTransform.anchoredPosition = Vector2.zero;
            inputField.textComponent.rectTransform.sizeDelta = Vector2.zero;
        }
        public void CloseKeyboard()
        {
            _isOpen = false;
            keyboard.SetActive(_isOpen);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}