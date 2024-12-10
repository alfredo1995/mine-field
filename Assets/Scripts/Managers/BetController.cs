using System;
using System.Collections.Generic;
using UI.Components;
using UnityEngine;

namespace Managers
{
    public class BetController : MonoBehaviour
    {

        [Header("States")] 
        [SerializeField] private NormalState normalBet;
        [SerializeField] private AutoState autoBet;

        [Header("Components")] 
        [SerializeField] private BetForm betForm;
        [SerializeField] private AutoBetAddOn gameStateSwitch;

        public event Action<BetType> OnBetTypeChanged; 

        private BetType CurrentBetType { get; set; } = BetType.Normal;
        public float CurrentBet => betForm.CurrentBet;
        
        private bool _autoBetEnabled;

        #region Unity Built-In
        
        public void Enable()
        {
            gameStateSwitch.OnChange.AddListener(FlipFlop);
            normalBet.autoBetAddOn = gameStateSwitch;
            autoBet.autoBetAddOn = gameStateSwitch;
            ChangeBetState();
        }

        private void OnDisable()
        {
            gameStateSwitch.OnChange.RemoveListener(FlipFlop);
        }

        #endregion

        private void FlipFlop(bool value)
        {
            _autoBetEnabled = value;
            ChangeBetState();
        }

        private void ChangeBetState()
        {
            CurrentBetType = _autoBetEnabled ? BetType.Auto : BetType.Normal;
            
            
            switch (CurrentBetType)
            {
                case BetType.Normal:
                    autoBet.OnExit();
                    normalBet.OnEnter();
                    OnBetTypeChanged?.Invoke(CurrentBetType);
                    break;
                case BetType.Auto:
                    normalBet.OnExit();
                    autoBet.OnEnter();
                    break;
                default:
                    throw new ArgumentException("Invalid Bet Type");
            }
        }

        public void SetMaxPayout(float payout) => normalBet.SetMaxPayout(payout);

        public void SetAutoBetRounds(List<int> rounds) => autoBet.SetRoundsOptions(rounds);

        public void DisableAutoBet()
        {
            gameStateSwitch.DisableAutoBet();
            gameStateSwitch.SetInteractable(false);
        }
    }

    public enum BetType
    {
        Normal,
        Auto
    }
}