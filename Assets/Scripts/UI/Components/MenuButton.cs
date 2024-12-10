using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public GameObject window;
    [SerializeField] private Button button;

    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    public void SetInteractable(bool value)
    {
        button.interactable = value;
    }

    private void OnClick()
    {
        window.SetActive(!window.activeSelf);
    }
}
