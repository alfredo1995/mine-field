using UnityEngine;
using Managers;
using TMPro;

namespace OpaGames.Utils
{
    public class LocaleComponent : MonoBehaviour
    {
        [SerializeField] private string key;
        [SerializeField] private TMP_Text text;

        private void Awake()
        {
            if (!text) text = GetComponent<TMP_Text>();
            text.SetText(GameManager.Locale[key]);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!text) text = GetComponent<TMP_Text>();
            text.SetText(string.Empty);
        }
#endif
    }
}