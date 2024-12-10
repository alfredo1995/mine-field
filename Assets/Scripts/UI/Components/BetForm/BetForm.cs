using OpaGames.Forms;

namespace UI.Components
{
    public class BetForm : Form
    {
        public float CurrentBet => betInputField.Value;

        public void SetFormsInteractable(bool value)
        {
            betInputField.IsInteractable = value;

            SetAddOnsInteractable(value);
        }

        public override void OnBetClick()
        {
        }

        protected override void RefreshBetButton()
        {
        }

        protected override void SetInteractable()
        {
        }
    }
}