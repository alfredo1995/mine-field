using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    public class NumberRoundsButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text value;
        [field: SerializeField] private bool IsSelected { get; set; }

        public TMP_Text Value
        {
            get
            {
                if (button == null)
                    value = GetComponentInChildren<TMP_Text>();
                return value;
            }
        }

        public Button Button
        {
            get
            {
                if (button == null) button = GetComponent<Button>();
                return button;
            }
        }

        public void SetSelected(bool isSelected)
        {
            IsSelected = isSelected;
            button.interactable = !isSelected;
        }
    }
}