using OpaGames.Forms;
using OpaGames.UIComponents;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Components
{
    public class AutoBetAddOn : FormAddOn
    {
        [SerializeField] private Switch toggle;
        public UnityEvent<bool> OnChange => toggle.OnChange;
        private bool disabled;
        public void DisableAutoBet(bool disable = true) => disabled = disable;

        public override void SetInteractable(bool value)
        {
            toggle.interactable = value && !disabled;
        }
    }
}