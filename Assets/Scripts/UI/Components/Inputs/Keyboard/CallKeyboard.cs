using OpaGames.Blackcopter.UI.AutoFlowLayout;
using TMPro;
using UI.Components.Keyboard;
using UnityEngine;

public class CallKeyboard : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [Space, SerializeField] private Vector2 keyboardOffset;

    private void Awake()
    {
        inputField.onSelect.AddListener(OpenKeyboard);
    }

    private void OpenKeyboard(string str)
    {
        if(AutoFlowLayout.IsLandScape) return;
        KeyBoard.instance.Show(keyboardOffset, inputField);
    }
}
