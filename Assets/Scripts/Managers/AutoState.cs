using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pattern;
using Settings;
using TMPro;
using UnityEngine;
using Ws.DataTypes;
using UI.Components;
using UI.Views;
using UnityEngine.UI;
using Grid = UI.Components.Grid;

namespace Managers
{
    public class AutoState : MonoBehaviour, IState
    {
        [SerializeField] private AudioController audioController;
        [SerializeField] private ServiceController serviceClient;
        [SerializeField] private GameSettings settings;

        [Header("Components")] 
        [SerializeField] private MinersGroup minersGroup;

        [SerializeField] private Grid grid;
        [SerializeField] private BetForm betForm;
        [SerializeField] private OpaGames.UIComponents.BetButton betButton;
        [SerializeField] private AutoPlayWindow autoBetForm;
        [HideInInspector] public AutoBetAddOn autoBetAddOn; 

        [Header("Views")] 
        [SerializeField] private CashOutView cashOutView;
        [SerializeField] private MenuView menuView;
        [SerializeField] private RandomButton randomButton;

        [SerializeField] private Sprite selectedSprite, defaultSprite;

        [SerializeField] private TMP_Text autoBetCountText;
        [SerializeField] private Button breakLoopButton;
        private GameObject buttonAnimator;

        private Color _unselectedColor;
        private readonly List<int> _selectedCells = new();

        private bool _canPlayAuto;
        private bool _cancelAuto;

        private int _autoBetCount;
        private int _autoBetAmount = 3;
        
        private string GenHash => Guid.NewGuid().ToString();
        private bool HasMoney => (decimal) betForm.CurrentBet <= GameManager.BalanceConsumer;

        public void OnEnter()
        {
            if (!buttonAnimator) buttonAnimator = betButton.GetComponentInChildren<Animator>().gameObject;
            
            _canPlayAuto = true;
            _autoBetCount = 0;

            //Subscribe Listeners
            betButton.onClick += OnBetButtonClick;
            breakLoopButton.onClick.AddListener(ClickCancel);

            //Subscribe Actions
            serviceClient.onPlay += Loop;

            //Subscribe Events
            grid.onSelectedCell += SelectCell;
            autoBetForm.onAutoPlay += AutoBetRound;
            randomButton.onRandomize += RandomButtonClick;

            //Select Grid State
            grid.SetInteractable(true);
            randomButton.IsInteractable = true;

            //populate sampling area
            randomButton.PopulateSamplingArea(grid.Size);
            betButton.SetInteractable(false);
            buttonAnimator.SetActive(false);
            autoBetCountText.SetText(_autoBetAmount.ToString());

            _unselectedColor = Color.white;
            _unselectedColor.a = .65f;
        }

        public void OnExit()
        {
            if (!buttonAnimator) buttonAnimator = betButton.GetComponentInChildren<Animator>().gameObject;
            //Stop All Loops * DEBUG 
            StopAllCoroutines();
            
            //click grid
            grid.onSelectedCell -= SelectCell;
            autoBetForm.onAutoPlay -= AutoBetRound;
            randomButton.onRandomize -= RandomButtonClick;

            //client events
            serviceClient.onPlay -= Loop;
            
            breakLoopButton.onClick.RemoveAllListeners();
            betButton.onClick -= OnBetButtonClick;
            
            _canPlayAuto = false;

            grid.SetInteractable(false);
            ActiveInteraction(true);

            //disable selected cells and clear list
            var cellsCount = _selectedCells.Count;

            for (var i = 0; i < cellsCount; i++)
            {
                randomButton.AddSamplingNumber(_selectedCells[0]);
                grid.ApplyCellImage(_selectedCells[0], defaultSprite, Color.white);
                _selectedCells.RemoveAt(0);
            }

            randomButton.IsInteractable = false;

            autoBetCountText.SetText(string.Empty);

            betButton.SetInteractable(true);
            buttonAnimator.SetActive(true);

            breakLoopButton.gameObject.SetActive(false);
            _cancelAuto = false;
        }

        private void ActiveInteraction(bool active)
        {
            minersGroup.SetInteractable(active);
            betForm.SetFormsInteractable(active);
        }

        private void RandomButtonClick(int value)
        {
            grid.SelectCell(value);
        }

        private async void AutoBetRound(AutoPlayForm autoPlayForm)
        {
            _autoBetAmount = autoPlayForm.rounds;
            autoBetCountText.SetText((_autoBetAmount - _autoBetCount).ToString());
            breakLoopButton.gameObject.SetActive(true);
            await PlayAuto();
        }

        public void SetRoundsOptions(List<int> options) => autoBetForm.SetRounds(options);
        
        private void OnBetButtonClick()
        {
            if (_autoBetCount <= 0 && HasMoney) autoBetForm.gameObject.SetActive(true);
            else ErrorHandler.InsufficientFounds();
        }

        private void ClickCancel()
        {
            _cancelAuto = true;
            breakLoopButton.gameObject.SetActive(false);
        }

        private async Task PlayAuto()
        {
            
            if (!HasMoney)
            {
                await BreakLoop();
                return;
            }

            if (!_canPlayAuto || _selectedCells.Count <= 0) return;

            grid.onSelectedCell -= SelectCell;

            for (int i = 0; i < _selectedCells.Count; i++)
            {
                grid.ApplyCellImage(_selectedCells[i], defaultSprite, Color.white);
            }

            await PlayAutoAsync();

            ReselectCells();

            autoBetAddOn.SetInteractable(false);
            randomButton.IsInteractable = false;
            grid.SetInteractable(false);
            ActiveInteraction(false);
            randomButton.IsInteractable = false;
            betButton.SetInteractable(false);
            buttonAnimator.SetActive(false);
            menuView.Lock();
        }

        private async Task PlayAutoAsync()
        {
            await serviceClient.MinesGameClient.PlayAuto(minersGroup.GetMinesAmount(), _selectedCells,
                (decimal)betForm.CurrentBet, GenHash);
        }

        private async Task Consult()
        {
            var consultValue = Mathf.Clamp(_selectedCells.Count, 1, grid.Size - minersGroup.GetMinesAmount());
            await serviceClient.MinesGameClient.Consult(minersGroup.GetMinesAmount(), consultValue);
        }

        private void Loop(OnPlayEvent evt)
        {
            _autoBetCount++;
            autoBetCountText.SetText((_autoBetAmount - _autoBetCount).ToString());

            if (_autoBetAmount - _autoBetCount == 0) breakLoopButton.gameObject.SetActive(false);

            if (evt.status == Status.Cashout)
                cashOutView.CashOut((float)evt.payoutMultiplier * betForm.CurrentBet);
            
            StartCoroutine(LoopDelay(evt));
        }

        private async Task EndLoop()
        {
            _autoBetCount = 0;

            grid.SetInteractable(true);
            breakLoopButton.gameObject.SetActive(false);

            for (int i = 0; i < _selectedCells.Count; i++)
            {
                grid.ApplyCellImage(_selectedCells[i], defaultSprite, Color.white);
            }

            _selectedCells.Clear();

            randomButton.PopulateSamplingArea(grid.Size);
            randomButton.IsInteractable = true;
            SetCellsColors(Color.white);
            autoBetCountText.SetText(_autoBetAmount.ToString());
            ActiveInteraction(true);
            autoBetAddOn.SetInteractable(true);
            betButton.SetInteractable(false);
            buttonAnimator.SetActive(false);
            menuView.Unlock();
            grid.onSelectedCell += SelectCell;
            await Consult();
        }

        private async Task BreakLoop()
        {
            StopAllCoroutines();
            await EndLoop();
            grid.ResetGrid();
            _cancelAuto = false;
        }

        private System.Collections.IEnumerator LoopDelay(OnPlayEvent evt)
        {
            var timer = new WaitForSeconds(settings.autoDelayTime);

            if (evt.status is Status.Cashout or Status.Crashed)
            {
                grid.UpdateGrid(evt.tiles);
                SetCellsColors(_unselectedColor);

                yield return timer;

                SetCellsColors(Color.white);

                if (_autoBetCount < _autoBetAmount)
                {
                    if (_cancelAuto)
                    {
                        yield return BreakLoop();
                        yield break;
                    }
                    grid.ResetUnselectedCells();
                    ReselectCells();

                    Color color = default;

                    for (int i = 0; i < _selectedCells.Count; i++)
                    {
                        grid.ApplyCellImage(_selectedCells[i], selectedSprite, color);
                    }

                    betButton.SetInteractable(false);
                    buttonAnimator.SetActive(false);

                    yield return timer;
                    
                    yield return PlayAuto();
                }
                else
                {
                    grid.ResetGrid();
                    yield return timer;
                    yield return BreakLoop();
                }
            }
        }

        private void ReselectCells()
        {
            for (int i = 0; i < _selectedCells.Count; i++)
            {
                grid.SelectCell(_selectedCells[i]);
            }
        }

        private void SetCellsColors(Color color)
        {
            var unselectedNumbers = randomButton.GetRemainingSamplingArea();

            foreach (var unselected in unselectedNumbers)
            {
                grid.ApplyCellColor(unselected, color);
            }
        }

        private async void SelectCell(int index)
        {
            if (_selectedCells.Contains(index))
            {
                _selectedCells.Remove(index);
                randomButton.AddSamplingNumber(index);
                grid.ApplyCellImage(index, defaultSprite, Color.white);
            }
            else if (grid.Size - minersGroup.GetMinesAmount() > _selectedCells.Count)
            {
                _selectedCells.Add(index);
                randomButton.RemoveSamplingNumber(index);
                var color = Color.cyan;
                grid.ApplyCellImage(index, selectedSprite, color);
            }
            else
            {
                randomButton.AddSamplingNumber(index);
                grid.DeselectCell(index);
            }

            await Consult();

            betButton.SetInteractable(_selectedCells.Count > 0);
            buttonAnimator.SetActive(_selectedCells.Count > 0);
            ActiveInteraction(_selectedCells.Count == 0);
        }
    }
}