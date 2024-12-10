using UnityEngine;
using OpaGames.Forms;

[RequireComponent(typeof(FormValueChangeButton))]
public class ValueChangeButtonDisabler : FormAddOn
{
    private FormValueChangeButton _changeButton;
    bool _interactableValue;

    public override void SetInteractable(bool value)
    {
        _changeButton.SetInteractable(value && _interactableValue);
    }

    private void Awake()
    {
        _changeButton = GetComponent<FormValueChangeButton>();
        var type = _changeButton.GetType();

        if(type == typeof(FormValueIncrementButton))
        {
            InteractableButton(
                (_changeButton.valueChange < 0 && _changeButton.changeTarget.Value > _changeButton.changeTarget.MinAllowedValue) || 
                (_changeButton.valueChange > 0 && _changeButton.changeTarget.Value < _changeButton.changeTarget.MaxAllowedValue));
            _changeButton.changeTarget.OnValueChanged += value => InteractableButton(
                (_changeButton.valueChange < 0 && value > _changeButton.changeTarget.MinAllowedValue) ||
                (_changeButton.valueChange > 0 && value < _changeButton.changeTarget.MaxAllowedValue));
        }
        else if (type == typeof(FormValueMultiplyButton))
        {
            InteractableButton(
                (_changeButton.valueChange < 1 && _changeButton.changeTarget.Value > _changeButton.changeTarget.MinAllowedValue) || 
                (_changeButton.valueChange > 1 && _changeButton.changeTarget.Value < _changeButton.changeTarget.MaxAllowedValue));
            _changeButton.changeTarget.OnValueChanged += value => InteractableButton(
                (_changeButton.valueChange < 1 && value > _changeButton.changeTarget.MinAllowedValue) ||
                (_changeButton.valueChange > 1 && value < _changeButton.changeTarget.MaxAllowedValue));
        }
    }

    private void InteractableButton(bool value)
    {
        _interactableValue = value;
        _changeButton.SetInteractable(_interactableValue);
    }
}
