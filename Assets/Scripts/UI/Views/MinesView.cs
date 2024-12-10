using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using OpaGames.UIComponents;
using Pattern;
using Settings;
using UI.Components;
using UI.Views;
using UnityEngine;

namespace UI
{
    public class MinesView : MonoBehaviour
    {
        [Header("Pre-Loader")] [SerializeField]
        private GameObject preLoader;
        
        [Header("Ball Background")]
        [SerializeField] private GameObject ballBackground;

        [Header("Views")] 
        [SerializeField] private GridView gridView;
        [SerializeField] private MenuView menuView;

        [SerializeField] private RandomButton randomButton;
        [SerializeField] private ValueInputField betField;
        [SerializeField] private BetButton betButton;
        [SerializeField] private MinersGroup minesGroup;
        [SerializeField] private BetsWindow betsWindow;

        private GameObject buttonAnimator;
        
        public IViewState GridView => gridView;
        public IViewState MenuView => menuView;
        public IViewState MinersView => minesGroup;

        public Action OnMinesChanged
        {
            get => minesGroup.onMinesAmountChanged;
            set => minesGroup.onMinesAmountChanged = value;
        }

        public void AddBetChangeListener(Action callback)
        {
            betField.OnValueChanged += _ => callback?.Invoke();
        }

        public void SetWallet(decimal balance, string currency)
        {
            menuView.SetCurrency(balance, currency);
        }

        public void ResetView()
        {
            gridView.ClearGrid();
            betButton.SetText($"<size=80>{GameManager.Locale["bet_button"]}</size>");
            if (!buttonAnimator) buttonAnimator = betButton.GetComponentInChildren<Animator>().gameObject;
            buttonAnimator.SetActive(true);
            betButton.SetInteractable(true);
        }

        public void HideLoader()
        {
            preLoader.SetActive(false);
            ballBackground.SetActive(true);
        }
        
        public bool HasPreload() => preLoader != null;

        public int GetMines()
        {
            return minesGroup.GetMinesAmount();
        }

        public void PopulateSampleArea()
        {
            randomButton.PopulateSamplingArea(gridView.Size);
        }

        private void SetMaxAndMinBet(float minBet, float maxBet, float defaultValue)
        {
            betField.SetMinAllowedValue(minBet);
            betField.SetMaxAllowedValue(maxBet);
            betField.Value = defaultValue;
        }

        public void SetNextPayoutMultiplier(float payoutMultiplierOnNext, float amount)
        {
            minesGroup.SetNextPayoutMultiplier(payoutMultiplierOnNext, amount);
        }

        public void SetSettings(GameSettings settings)
        {
            var minesGridSize = settings.gridSize * settings.gridSize;

            minesGroup.SetMaxPayout(settings.maxPayout);
            minesGroup.SetOptions(settings.minMines, settings.maxMines, "");
            SetMaxAndMinBet(settings.minBet, settings.maxBet, settings.defaultBet);
            gridView.CreateGrid(minesGridSize);
            randomButton.CreateSamplingArea(minesGridSize);
            menuView.SetGameLimits(settings.minBet, settings.maxBet, settings.maxPayout);
            betsWindow.ApplyLimits(settings.betValues);

            Resources.UnloadAsset(settings);
        }
    }
}