using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PopupSystem
{
    public class PopupUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField]
        private GraphicRaycaster uiCanvasGraphicRaycaster;

        [SerializeField] private CanvasGroup uiCanvasGroup;

        [SerializeField] private GameObject uiHeader;
        [SerializeField] private Text uiTitle;
        [SerializeField] private Text uiDescription;
        [SerializeField] private Image uiBtnImageAction1, uiBtnImageAction2;
        [SerializeField] private Text uiBtnTextAction1, uiBtnTextAction2;
        private Button _rightButton, _leftButton;

        [Header("Popup Colors :")]
        [SerializeField] private Color[] colors;

        [Header("Popup Fade Duration")]
        [SerializeField, Range(.1f, .8f)] private float fadeInDuration = .3f;

        [SerializeField, Range(.1f, .8f)] private float fadeOutDuration = .3f;

        [Space] private UnityAction _onRightAction, _onLeftAction;
        public UnityEvent onClose = new();

        public bool IsVisible { get; private set; }

        private void Awake()
        {
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;
            uiCanvasGraphicRaycaster.enabled = false;

            _rightButton = uiBtnImageAction1.GetComponent<Button>();
            _rightButton.onClick.RemoveAllListeners();
            _rightButton.onClick.AddListener(() =>
            {
                _onRightAction?.Invoke();
                _onRightAction = null;

                Unset();
            });

            _leftButton = uiBtnImageAction2.GetComponent<Button>();
            _leftButton.onClick.RemoveAllListeners();
            _leftButton.onClick.AddListener(() =>
            {
                _onLeftAction?.Invoke();
                _onLeftAction = null;

                Unset();
            });
        }

        public void Setup(string title, string description, PopupButtonData rightButtonData,
            PopupButtonData leftButtonData = null)
        {
            uiHeader.SetActive(!string.IsNullOrEmpty(title.Trim()));
            _leftButton.gameObject.SetActive(leftButtonData != null);

            SetTitle(title);
            SetDescription(description);

            SetRightButton(rightButtonData.buttonText, rightButtonData.buttonColor, rightButtonData.onCloseAction);

            if (leftButtonData != null && !string.IsNullOrEmpty(leftButtonData.buttonText))
                SetLeftButton(leftButtonData.buttonText, leftButtonData.buttonColor, leftButtonData.onCloseAction);

            Dismiss();
            StartCoroutine(FadeIn(fadeInDuration));
        }

        private void Unset()
        {
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;
            uiCanvasGraphicRaycaster.enabled = false;

            StartCoroutine(FadeOut(fadeOutDuration));
        }

        #region Setters

        public void SetTitle(string title) => uiTitle.text = title;
        public void SetDescription(string description) => uiDescription.text = description;

        public void SetRightButton(string txtBtn, PopupColor color, UnityAction action)
        {
            uiBtnTextAction1.text = txtBtn;
            _onRightAction = action;

            //Color
            uiBtnImageAction1.color = colors[(int)color];
        }

        public void SetLeftButton(string txtBtn, PopupColor color, UnityAction action)
        {
            uiBtnTextAction2.text = txtBtn;
            uiBtnImageAction2.color = colors[(int)color];
            _onLeftAction = action;
        }

        #endregion

        #region Coroutines

        private IEnumerator FadeIn(float duration)
        {
            IsVisible = true;
            uiCanvasGraphicRaycaster.enabled = true;
            yield return CanvasUtility.Fade(uiCanvasGroup, 0f, 1f, duration);
            uiCanvasGroup.interactable = true;
        }

        private IEnumerator FadeOut(float duration)
        {
            yield return CanvasUtility.Fade(uiCanvasGroup, 1f, 0f, duration);
            uiCanvasGroup.interactable = false;
            uiCanvasGraphicRaycaster.enabled = false;

            IsVisible = false;
            onClose?.Invoke();
        }

        #endregion

        private void Dismiss()
        {
            StopAllCoroutines();
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;
            uiCanvasGraphicRaycaster.enabled = false;
            Time.timeScale = 1f;
        }
    }
}