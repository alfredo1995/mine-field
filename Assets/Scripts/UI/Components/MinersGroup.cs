using System;
using System.Collections.Generic;
using System.Globalization;
using Managers;
using Pattern;
using TMPro;
using UnityEngine;
using Utils.Extensions;

namespace UI.Components
{
    public class MinersGroup : MonoBehaviour, IViewState
    {
        [SerializeField] TMP_Dropdown dropdown;
        [SerializeField] TMP_Text nexPayoutMultiplierText;
        [SerializeField] private TMP_Text captionDuplicate;
        private float _maxPayout;

        public Action onMinesAmountChanged;

        private TMP_Dropdown Dropdown
        {
            get
            {
                if (dropdown == null)
                    dropdown = GetComponentInChildren<TMP_Dropdown>();
                return dropdown;
            }
        }

        public void OnEnable()
        {
            Dropdown.onValueChanged.AddListener(_ => onMinesAmountChanged?.Invoke());
            Dropdown.onValueChanged.Invoke(Dropdown.value);
        }

        public void OnDisable()
        {
            Dropdown.onValueChanged.RemoveAllListeners();
        }

        public void SetInteractable(bool value)
        {
            Dropdown.interactable = value;
        }

        public void SetOptions(int minMines, int count, string text = "Mines: ")
        {
            Dropdown.options = new List<TMP_Dropdown.OptionData>();

            for (int i = minMines - 1; i < count; i++)
            {
                var option = $"{text}{i + 1}";
                Dropdown.options.Add(new TMP_Dropdown.OptionData(option));
            }

            Dropdown.value = 2;
            Dropdown.RefreshShownValue();
        }

        public int GetMinesAmount()
        {
            return Dropdown.value + 1;
        }

        public void SetMaxPayout(float payout) => _maxPayout = payout;

        public void SetNextPayoutMultiplier(float multiplier, float betAmount)
        {
            Dropdown.captionText.text = $"{GameManager.Locale["mines_dropdown"]} {Dropdown.options[Dropdown.value].text}";
            captionDuplicate.text = Dropdown.captionText.text;
            var payout = (decimal) Mathf.Min(_maxPayout, multiplier * betAmount);
            nexPayoutMultiplierText.text = $"{GameManager.Locale["next_bet"]}: {payout.TruncateDecimal(2).ToString("N2", CultureInfo.CurrentCulture)}";
        }

        public void Lock()
        {
            SetInteractable(false);
        }

        public void Unlock()
        {
            SetInteractable(true);
        }
    }
}