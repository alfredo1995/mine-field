using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace UI.Components
{
    public class HistoryData : MonoBehaviour
    {
        [SerializeField] private TMP_Text DateText, TimeText, BetText, CashoutText, MultiplierText;
        [SerializeField] private Image background;
        [SerializeField] private Image[] Divisions;
        [SerializeField] private Button historyButton;

        [Header("Styling parameters")] 
        [SerializeField] private Sprite winSprite;
        [SerializeField] private Sprite loseSprite;
        [SerializeField] private Color winDivision, loseDivision;

        public Button.ButtonClickedEvent OnClick
        {
            get => historyButton.onClick;
            set => historyButton.onClick = value;
        }

        public void SetData(DateTime time, decimal bet, decimal cashOut, double multiplier)
        {
            ConvertToLanguage(time);
            
            BetText.text = $"{bet.ToString("N2", CultureInfo.CurrentCulture)}";
            CashoutText.text = $"{cashOut.TruncateDecimal(2).ToString(CultureInfo.CurrentCulture)}";
            MultiplierText.text = $"{((decimal)multiplier).TruncateDecimal(2).ToString("#0.00", CultureInfo.InvariantCulture)}x";
            
            //NOTE: currently there's not a way do know if player won or not because of cashOut and multiplier being inaccurate
            ApplyStyle(bet < cashOut);
        }
        
        private void ConvertToLanguage(DateTime time)
        {
            var defaultLanguage = PlayerPrefs.GetString("lang", "en");
            DateText.text = defaultLanguage.Equals("en") ? time.ToString("d") : time.ToString("dd/MM/yyyy");
            TimeText.text = defaultLanguage.Equals("en") ? time.ToString("t") : time.ToString("HH:mm:ss");
        }

        private void ApplyStyle(bool win = false)
        {
            var newColor = win ? winDivision : loseDivision;
            
            var length = Divisions.Length;
            for (var i = 0; i < length; i++)
                Divisions[i].color = newColor;

            background.sprite = win ? winSprite : loseSprite;
        }
    }
}