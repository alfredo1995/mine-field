using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Button cellButton;
        [SerializeField] private Image background;
        [SerializeField] private Animator ball, overlay, underlay;
        [SerializeField] private Image underlayImage;
        [SerializeField] private GameObject explosion;

        [field: SerializeField] float AnimationTime { get; set; } = 0.2f;

        private bool _isAnimation;


        public Button CellButton
        {
            get
            {
                if (cellButton == null) cellButton = GetComponent<Button>();
                return cellButton;
            }
        }

        public bool IsSelected { get; set; }

        public void ShowCell(bool active)
        {
            background.enabled = active;
        }

        public void ShowOverlay(bool active)
        {
            overlay.gameObject.SetActive(active);
        }

        public void ShowUnderlay(bool active)
        {
            //optional, see if approved
            underlay.gameObject.SetActive(active);
        }

        public void SetUnderlayColor(Color color)
        {
            underlayImage.color = color;
        }

        public void SetBackgroundColor(Color color)
        {
            background.color = color;
        }

        public void SetBallColor(Color color)
        {
            ball.GetComponent<Image>().color = color;
        }

        public void ShowBall(bool active)
        {
            ball.gameObject.SetActive(active);
        }

        public void SetBallAnimation(RuntimeAnimatorController controller)
        {
            ball.runtimeAnimatorController = controller;
        }

        public void SetUnderlayAnimation(RuntimeAnimatorController controller)
        {
            underlay.runtimeAnimatorController = controller;
        }

        public void Explode(bool active)
        {
            explosion.transform.SetParent(active ? transform.parent.parent : transform);
            explosion.transform.localScale = Vector3.one*3;
            explosion.SetActive(active);
        }

        public void SetBackground(Sprite sprite)
        {
            background.sprite = sprite;
            cellButton.image.sprite = sprite;
        }

        private void ShowAnimation(float time, float scale)
        {
            StartCoroutine(ScaleAnimation(time, scale));
        }

        private IEnumerator ScaleAnimation(float duration, float scale)
        {
            _isAnimation = true;

            var cellTransform = cellButton.transform;
            var startScale = cellTransform.localScale.x;

            yield return ScriptAnimations.ScaleAnimation(cellTransform, startScale, scale, duration);
            yield return ScriptAnimations.ScaleAnimation(cellTransform, scale, startScale, duration);

            _isAnimation = false;
        }

        public void SetInteractable(bool value)
        {
            cellButton.image.raycastTarget = value;
            cellButton.interactable = value;
        }

        private void Click()
        {
            IsSelected = !IsSelected;

            if (_isAnimation)
                return;

            ShowAnimation(AnimationTime, 1.3f);
        }

        #region OnEnable/OnDisable

        private void OnEnable()
        {
            cellButton.onClick.AddListener(Click);
        }

        private void OnDisable()
        {
            if (cellButton == null) return;
            cellButton.onClick.RemoveAllListeners();
        }

        #endregion
    }
}
