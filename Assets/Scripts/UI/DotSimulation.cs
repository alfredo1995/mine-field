using System.Collections;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class DotSimulation : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private string baseName = "connecting_screen";
        
        private int _dotCount;
        private const int MaxDots = 3;

        private IEnumerator Start()
        {
            var textValue = GameManager.Locale[baseName];
            text.SetText(textValue);
            
            while (gameObject.activeSelf)
            {
                _dotCount = _dotCount % MaxDots + 1;
                yield return new WaitForSeconds(0.5f);
                text.SetText(string.Concat(textValue, new string('.', _dotCount)));
            }
            
        }

        private void OnDisable()
        {
            Destroy(gameObject);
        }
    }
}