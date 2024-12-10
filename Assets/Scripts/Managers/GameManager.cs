using System;
using System.Collections.Generic;
using System.Globalization;
using PopupSystem;
using Settings;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using OpaGames.Utils;
using UI.Views;
using Ws.DataTypes;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Behaviours")] 
        [SerializeField] private AudioController audioController;
        [SerializeField] private BetController betController;
        [SerializeField] private ServiceController serviceController;

        [Header("Views")] 
        [SerializeField] private MinesView minesView;
        [SerializeField] private CashOutView cashOutView;

        private UserStatus _userStatus = UserStatus.Off;
        private Status _gameStatus = Status.Canceled;
        
        private List<int> _currentTiles = new();
        
        public static Locale Locale => Resources.Load<Locale>($"Localization/{PlayerPrefs.GetString("lang", "en")}");
        
        //Global Balance Consumer
        public static decimal BalanceConsumer;

        private void OnEnable()
        {
            serviceController.onSettings += OnSuccess;
            
            var lang = PlayerPrefs.GetString("lang", "en");
            
            //Set Every the Current Culture
            CultureInfo.DefaultThreadCurrentCulture = lang == "en" ? new CultureInfo("en-US") : new CultureInfo("pt-BR");
        }

        private void OnSuccess(GameSettings settings)
        {
            serviceController.onJoinLobby += OnJoinLobby;
            serviceController.onPlay += OnPlayEvent;
            serviceController.onUserChange += OnUserChange;
            serviceController.onCashOut += OnCashOut;
            serviceController.onConsult += OnConsult;
            serviceController.onDisconnect += OnDisconnect;
            betController.OnBetTypeChanged += OnBetTypeChanged;
            
            minesView.SetSettings(settings);

            minesView.OnMinesChanged += Consult;
            minesView.AddBetChangeListener(Consult);
            
            betController.SetMaxPayout(settings.maxPayout);
            betController.SetAutoBetRounds(settings.autoRoundsValues);
            if(!settings.autoBetEnabled) betController.DisableAutoBet();
        }

        private void OnDisable()
        {
            serviceController.onSettings -= OnSuccess;
            serviceController.onJoinLobby -= OnJoinLobby;
            serviceController.onPlay -= OnPlayEvent;
            serviceController.onUserChange -= OnUserChange;
            serviceController.onCashOut -= OnCashOut;
            serviceController.onConsult -= OnConsult;
            betController.OnBetTypeChanged -= OnBetTypeChanged;

            serviceController.onDisconnect -= OnDisconnect;
            
            minesView.OnMinesChanged -= Consult;
        }

        private void OnJoinLobby()
        {
            betController.Enable();
        }

        private void OnUserChange(OnUserChangeEvent evt)
        {
            if (minesView.HasPreload())
            {
                minesView.HideLoader();
                audioController.PlayMusic();
            }
            
            BalanceConsumer = evt.balance;
            
            minesView.SetWallet(evt.balance, evt.currency);
            UpdateUserStatus(evt.status);
        }

        private void OnPlayEvent(OnPlayEvent evt)
        {
            UpdateGameStatus(evt.status);

            //Cache the tiles
            var index = -1;
            
            if(evt.status != Status.Crashed && evt.status != Status.Cashout)
                index = _currentTiles.GetDeltaIndex(evt.tiles);
            
            _currentTiles = evt.tiles;

            if (index != -1 && _currentTiles[index] == 1 && evt.status != Status.Crashed && evt.status != Status.Cashout)
            {
                //Force Sounds
                audioController.PlayCellClickSfx();
            }
            
            //Set Global CashOut Sound
            if (evt.status == Status.Cashout) audioController.PlayWinSfx();
        }

        private void OnCashOut(OnCashOutEvent evt)
        {
            UpdateGameStatus(Status.Cashout);
            audioController.PlayWinSfx();
            minesView.GridView.Lock();
            minesView.MenuView.Unlock();
            
            //Views
            cashOutView.CashOut((float) evt.payoutMultiplier * betController.CurrentBet);
        }

        private void OnConsult(OnConsultEvent evt)
        {
            minesView.SetNextPayoutMultiplier((float)evt.payoutMultiplier, betController.CurrentBet);
        }
        
        private void OnBetTypeChanged(BetType betType)
        {
            Consult();
        }
        
        private async void Consult()
        {
            await serviceController.MinesGameClient.Consult(minesView.GetMines(), 1);
        }

        private void OnStateChange()
        {
            switch (_userStatus)
            {
                case UserStatus.Spectating:
                    minesView.ResetView();
                    minesView.MenuView.Unlock();
                    Consult();
                    break;
                case UserStatus.Playing:
                    minesView.PopulateSampleArea();

                    minesView.MenuView.Lock();
                    minesView.MinersView.Lock();
                    minesView.GridView.Unlock();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateUserStatus(UserStatus userStatus)
        {
            if (userStatus == _userStatus) return;
            _userStatus = userStatus;

            OnStateChange();
        }

        private void UpdateGameStatus(Status status)
        {
            if (status == _gameStatus) return;
            _gameStatus = status;

            if (status != Status.Crashed) return;
            
            audioController.PlayExplodeSfx();
            minesView.GridView.Lock();
            
            _gameStatus = Status.Canceled;
        }

        private void OnDisconnect()
        {
            Popup.Instance.Show("Disconnected", "You have been disconnected, try again.",
                new PopupButtonData
                {
                    buttonText = "Try Again",
                    buttonColor = PopupColor.Green,
                    onCloseAction = Reconnect
                });
        }

        private void Reconnect()
        {
            SceneManager.LoadScene(0);
        }
    }
}