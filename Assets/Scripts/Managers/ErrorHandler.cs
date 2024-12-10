using OpaGames.Utils;
using PopupSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Extensions;
using Ws.DataTypes;

namespace Managers
{
    public class ErrorHandler : MonoBehaviour
    {
        [SerializeField] private ServiceController serviceController;
        private static Locale Locale => Resources.Load<Locale>($"Localization/Error/{PlayerPrefs.GetString("lang", "en")}");

        private void OnEnable() => serviceController.onGameError += ShowError;
        private void OnDisable() => serviceController.onGameError -= ShowError;

        private void ShowError(OnErrorEvent msg)
        {
            //Force Auto State Stop
            FindObjectOfType<AutoState>().OnExit();
            FindObjectOfType<NormalState>().OnExit();

            var (title, description) = CheckErrorOnError(msg.message);

            Popup.Instance.Show(title, description,
                new PopupButtonData
                {
                    buttonText = "Try Again",
                    buttonColor = PopupColor.Green,
                    onCloseAction = Reconnect
                });
        }

        public static void InsufficientFounds()
        {
            var (title, description) = Locale["OP001"].ParseToTitleDescription();

            Popup.Instance.Show(title, description,
                new PopupButtonData
                {
                    buttonText = "OK",
                    buttonColor = PopupColor.Green
                });
        }

        private (string, string) CheckErrorOnError(string error)
        {
            var (title, description) = Locale[error].ParseToTitleDescription();
            title = string.IsNullOrEmpty(title) ? "Title not found" : title;
            description = string.IsNullOrEmpty(description) ? "Description not found" : description;
            return (title, description);
        }

        private static void Reconnect() => SceneManager.LoadScene(0);
    }
}