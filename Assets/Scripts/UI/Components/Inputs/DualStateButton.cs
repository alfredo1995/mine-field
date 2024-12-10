using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DualStateButton : MonoBehaviour
{
    private Button button;
    public Button stateButton => button;
    public bool currentState { get; set; }

    public bool triggerAtStart;
    public bool startingState;
    public UnityEvent onButtonOn, onButtonOff;
    public UnityEvent<bool> onButtonStateChange;
    
    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonClicked);
        if (triggerAtStart)
        {
            currentState = !startingState;
            ButtonClicked();
        }
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(ButtonClicked);
    }

    public void ButtonClicked()
    {
        SimulateClick(!currentState);
    }

    public void SimulateClick(bool state)
    {
        currentState = state;
        if(currentState) onButtonOn?.Invoke();
        else onButtonOff?.Invoke();
        onButtonStateChange?.Invoke(currentState);
    }
}
