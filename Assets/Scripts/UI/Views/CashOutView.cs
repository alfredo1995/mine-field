using System.Collections;
using System.Globalization;
using Settings;
using TMPro;
using UnityEngine;
using Utils.Extensions;

namespace UI.Views
{
    public class CashOutView : MonoBehaviour
    {
        [SerializeField] private GameObject cashOutWindow;
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private float textDuration = 1f;
        
        private TextMeshProUGUI _cashOutText;

        public void CashOut(float payoutMultiplier)
        {
            var limit = gameSettings.maxPayout;
            _cashOutText = cashOutWindow.GetComponentInChildren<TextMeshProUGUI>();

            var cashOutAnimator = cashOutWindow.GetComponent<Animator>();
            cashOutAnimator.Play("CashOut");
        
            StartCoroutine(routine: CountUpValue(Mathf.Clamp(payoutMultiplier, 0, limit)));
        }

        private IEnumerator CountUpValue(float payout)
        {
            var time = 0f;
            do
            {
                var currentValue = payout * (time / textDuration);
                _cashOutText.text = $"+{currentValue.ToString("N2", CultureInfo.CurrentCulture)}";
                yield return null;
                time += Time.deltaTime;

            } while (time <= textDuration);
        
            var truncateDecimal = ((decimal)payout).TruncateDecimal(2);
            _cashOutText.text = $"+{truncateDecimal.ToString("N2", CultureInfo.CurrentCulture)}";
        }
    }
}
