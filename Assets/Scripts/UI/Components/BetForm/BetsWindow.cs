using OpaGames.Forms;
using OpaGames.UIComponents;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Components
{
    public class BetsWindow : FormAddOn
    {
        [SerializeField] private Button windowButton;
        [SerializeField] private ValueInputField inputField;
        [SerializeField] private GameObject windowContainer;
        [SerializeField] private List<FormValueSetButton> valueChangeButtons;
        private FormValueSetButton _currentBetButton;

        private FormValueSetButton CurrentBetButtonBetButton
        {
            get
            {
                if (_currentBetButton == null)
                    _currentBetButton = valueChangeButtons[0];

                return _currentBetButton;
            }
            set => _currentBetButton = value;
        }

        private void OnEnable()
        {
            windowButton.onClick.AddListener(ShowBets);
            foreach (var betButton in valueChangeButtons)
            {
                betButton.changeTarget = inputField;
                var button = betButton.GetComponent<Button>();
                button.onClick.AddListener(() => DisableClickedButton(betButton));
            }
        }

        private void OnDisable()
        {
            windowButton.onClick.RemoveListener(ShowBets);
            foreach (var betButton in valueChangeButtons)
            {
                var button = betButton.GetComponent<Button>();
                button.onClick.RemoveListener(() => DisableClickedButton(betButton));
            }
        }

        private void DisableClickedButton(FormValueSetButton button)
        {
            CurrentBetButtonBetButton.SetInteractable(true);
            CurrentBetButtonBetButton = button;
            CurrentBetButtonBetButton.SetInteractable(false);
            CloseWindow();
        }

        public override void SetInteractable(bool value)
        {
            if (!value)
            {
                CloseWindow();
            }

            windowButton.interactable = value;
        }

        private void ShowBets()
        {
            CheckCurrentBet();
            windowContainer.SetActive(!windowContainer.activeSelf);
        }

        private void CheckCurrentBet()
        {
            for (int i = 0; i < valueChangeButtons.Count; i++)
            {
                valueChangeButtons[i].SetInteractable(true);
                if (Math.Abs(valueChangeButtons[i].valueChange - inputField.Value) < Mathf.Epsilon)
                {
                    DisableClickedButton(valueChangeButtons[i]);
                }
            }
        }

        private void CloseWindow()
        {
            windowContainer.SetActive(false);
        }


        public void ApplyLimits(List<float> value)
        {
            for (int i = 0; i < valueChangeButtons.Count; i++)
            {
                valueChangeButtons[i].valueChange = value[i];
                valueChangeButtons[i].ChangeButton.GetComponentInChildren<TMP_Text>().SetText(value[i].ToString("N2"));
            }
            Resources.UnloadUnusedAssets();
        }
    }
}