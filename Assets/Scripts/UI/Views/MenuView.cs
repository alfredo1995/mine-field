using Pattern;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace UI.Views
{
    public class MenuView : MonoBehaviour, IViewState
    {
        [SerializeField] private Button infoButton;
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private MenuButton menuButton;
        [SerializeField] private GameObject infoPopup;

        [Header("Game Limits")]
        [SerializeField] private TMP_Text minBetTxt;
        [SerializeField] private TMP_Text maxBetTxt;
        [SerializeField] private TMP_Text maxBetPayoutTxt;

        public void SetCurrency(decimal balance, string currency)
        {
            var truncateDecimal = balance.TruncateDecimal(2);
            balanceText.text = $"{truncateDecimal:0#.00} {currency}";
        }

        private void InfoClick()
        {
            infoPopup.SetActive(true);
        }

        public void SetGameLimits(float minBet, float maxBet, float maxBetPayout)
        {
            var currentCulture = CultureInfo.CurrentCulture;

            minBetTxt.SetText(minBet.ToString("N2", currentCulture));
            maxBetTxt.SetText(maxBet.ToString("N2", currentCulture));
            maxBetPayoutTxt.SetText(maxBetPayout.ToString("N2", currentCulture));
        }

        #region OnEnable/OnDisable

        private void OnEnable()
        {
            infoButton.onClick.AddListener(InfoClick);
        }

        private void OnDisable()
        {
            infoButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region IViewState

        public void Lock()
        {
            infoButton.interactable = false;
            menuButton.SetInteractable(false);
        }

        public void Unlock()
        {
            infoButton.interactable = true;
            menuButton.SetInteractable(true);
        }

        #endregion
    }
}