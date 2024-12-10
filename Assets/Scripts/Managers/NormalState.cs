using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pattern;
using UI.Components;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;
using Ws.DataTypes;
using Grid = UI.Components.Grid;

namespace Managers
{
    public class NormalState : MonoBehaviour, IState
    {
        [SerializeField] ServiceController serviceClient;

        [Header("Components")] 
        [SerializeField] private MinersGroup minersGroup;

        [SerializeField] private Grid grid;
        [SerializeField] private RandomButton randomButton;
        [SerializeField] private BetForm betForm;
        [SerializeField] private OpaGames.UIComponents.BetButton betButton;
        [HideInInspector] public AutoBetAddOn autoBetAddOn;
        private Image _betButtonImage;
        private GameObject betButtonAnimator;
        private BetButtonExtension _buttonExtension;

        private float _maxPayout;
        private bool _canCashOut;
        private int _currentCellIndex;

        private Color _disabledColor, _unselectedColor;

        private void CashOut(OnCashOutEvent evt)
        {
            _canCashOut = false;

            SetCellsColors(_unselectedColor);

            //TODO: move to view
            grid.UpdateGrid(evt.tiles);
            randomButton.IsInteractable = false;
            betButton.SetText($"<size=80>{GameManager.Locale["bet_button"]}</size>");
            betButton.SetInteractable(false);
            _buttonExtension.SetBetState();
            ActiveInteraction(false);
        }

        //play command
        private async void PlayCell(int index)
        {
            _currentCellIndex = index;
            randomButton.RemoveSamplingNumber(index);

            betButton.SetInteractable(false);
            grid.SetInteractable(false);

            await serviceClient.MinesGameClient.PlayGame(index);
            randomButton.IsInteractable = false;
        }

        private void OnPlayEvent(OnPlayEvent evt)
        {
            _canCashOut = evt.canCashout;

            minersGroup.SetNextPayoutMultiplier((float)evt.payoutMultiplierOnNext, (float)evt.bet_value);
            betButton.SetInteractable(_canCashOut && evt.status != Status.Crashed);
            
            if (!_betButtonImage)
            {
                _betButtonImage = betButton.GetComponent<Image>();
                betButtonAnimator = betButton.GetComponentInChildren<Animator>().gameObject;
            }
            betButtonAnimator.SetActive(false);
            _buttonExtension.SetCashoutState();
            SetCashOut(evt.bet_value * evt.payoutMultiplier, evt.currency);

            if (evt.status == Status.Crashed) OnCrashed(evt.tiles);
            else
            {
                grid.SetInteractableUnselectedCell(true);
                SetCellsInteractable(true);
                grid.UpdateGrid(evt.tiles, _currentCellIndex);
                randomButton.IsInteractable = true;
            }
            
        }

        private void SetCashOut(double value, string currency)
        {
            var cashOut = $"{((decimal)Mathf.Min((float)value, _maxPayout)).TruncateDecimal(2):N2}";
            betButton.SetText($"<size=45>{GameManager.Locale["bet_button_collect"]}{cashOut} {currency}</size>");
        }

        private void OnCrashed(List<int> tiles)
        {
            grid.BlastAnimation(_currentCellIndex,tiles,Crashed);
        }

        private void Crashed()
        {
            SetCellsColors(_unselectedColor);

            grid.SetInteractable(false);
            randomButton.IsInteractable = false;

            _canCashOut = false;
            _buttonExtension.SetBetState();
            betButton.SetText($"<size=80>{GameManager.Locale["bet_button"]}</size>");
            ActiveInteraction(true);
        }

        private void RandomButtonClick(int number)
        {
            grid.SelectCell(number);
        }

        //start command
        private async void StartGame()
        {
            betButton.SetInteractable(false);
            ActiveInteraction(false);

            if (_canCashOut)
            {
                grid.SetInteractable(false);
                await CashOutAsync();
            }
            else
            {
                if ((decimal) betForm.CurrentBet > GameManager.BalanceConsumer)
                {
                    ErrorHandler.InsufficientFounds();
                    betButton.SetInteractable(true);
                    ActiveInteraction(true);
                    return;
                }
                await StartGameAsync();
            }
        }

        private async Task CashOutAsync() => await serviceClient.MinesGameClient.CashOut();

        private async Task StartGameAsync() =>
            await serviceClient.MinesGameClient.StartGame((decimal) betForm.CurrentBet, minersGroup.GetMinesAmount(), Guid.NewGuid().ToString());

        public void OnEnter()
        {
            if(!_buttonExtension) _buttonExtension = betButton.GetComponent<BetButtonExtension>(); 
            
            grid.onSelectedCell += PlayCell;
            randomButton.onRandomize += RandomButtonClick;

            grid.ResetGrid();

            betButton.onClick += StartGame;
            betButton.SetInteractable(true);

            //client events
            serviceClient.onPlay += OnPlayEvent;
            serviceClient.onCashOut += CashOut;
            serviceClient.onUserChange += UserStateChange;

            _disabledColor = Color.white;
            _disabledColor.a = .4f;

            _unselectedColor = Color.white;
            _unselectedColor.a = .65f;

            SetCellsInteractable();
        }

        public void OnExit()
        {
            betButton.onClick -= StartGame;

            grid.onSelectedCell -= PlayCell;
            randomButton.onRandomize -= RandomButtonClick;

            //client events
            serviceClient.onPlay -= OnPlayEvent;
            serviceClient.onCashOut -= CashOut;
            serviceClient.onUserChange -= UserStateChange;

            SetCellsInteractable(true);
        }

        public void SetMaxPayout(float payout) => _maxPayout = payout;

        private void UserStateChange(OnUserChangeEvent evt)
        {
            if (evt.status != UserStatus.Spectating) return;
            
            randomButton.PopulateSamplingArea(grid.Size);
            SetCellsInteractable();
            ActiveInteraction(true);
        }

        private void SetCellsInteractable(bool interactable = false)
        {
            foreach (var unselected in randomButton.GetRemainingSamplingArea())
            {
                grid.SetInteractableBackground(unselected, interactable);
            }
        }

        private void SetCellsColors(Color color)
        {
            foreach (var unselected in randomButton.GetRemainingSamplingArea())
            {
                grid.ApplyCellColor(unselected, color);
            }
        }

        private void ActiveInteraction(bool active)
        {
            minersGroup.SetInteractable(active);
            betForm.SetFormsInteractable(active);
            autoBetAddOn.SetInteractable(active);
        }
    }
}