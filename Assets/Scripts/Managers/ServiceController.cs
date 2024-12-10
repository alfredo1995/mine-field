using System;
using Settings;
using UnityEngine;
using Ws;
using Ws.DataTypes;

namespace Managers
{
    public class ServiceController : MonoBehaviour
    {
        private MinesGameClient _minesGameClient;
        public MinesGameClient MinesGameClient => _minesGameClient;

        public Action<OnPlayEvent> onPlay;
        public Action<OnCashOutEvent> onCashOut;
        public Action<OnUserChangeEvent> onUserChange;
        public Action<OnUserBetsEvent> onUserBets;
        public Action<OnConsultEvent> onConsult;
        public Action<GameSettings> onSettings;
        public Action<OnErrorEvent> onGameError;

        public Action onJoinLobby;
        public Action onDisconnect;

        private async void OnEnable()
        {
            _minesGameClient = MinesGameClientFactory.Create();

            _minesGameClient.onConnected += OnConnected;
            _minesGameClient.onDisconnected += OnDisconnected;
            _minesGameClient.onError += Debug.Log;
            _minesGameClient.onGameError += OnGameError;
            _minesGameClient.onPlay += OnPlay;
            _minesGameClient.onCashOut += OnCashOut;
            _minesGameClient.onUserBets += OnUserBets;
            _minesGameClient.onConsult += OnConsult;
            _minesGameClient.onUserChange += OnUserChange;
            _minesGameClient.onSettings += OnSettings;
            
            await _minesGameClient.Connect();
        }

        

        private async void OnDisable()
        {
            _minesGameClient.onConnected -= OnConnected;
            _minesGameClient.onDisconnected -= OnDisconnected;
            _minesGameClient.onError -= Debug.Log;
            _minesGameClient.onGameError -= OnGameError;
            _minesGameClient.onPlay -= OnPlay;
            _minesGameClient.onCashOut -= OnCashOut;
            _minesGameClient.onUserBets -= OnUserBets;
            _minesGameClient.onConsult -= OnConsult;
            _minesGameClient.onUserChange -= OnUserChange;
            _minesGameClient.onSettings -= OnSettings;

            if (_minesGameClient.Status == BrokerStatus.Open)
                await _minesGameClient.Disconnect();
        }

        private void OnSettings(GameSettings settings) => onSettings?.Invoke(settings);
        private void OnUserChange(OnUserChangeEvent evt) => onUserChange?.Invoke(evt);
        private void OnUserBets(OnUserBetsEvent evt) => onUserBets?.Invoke(evt);
        private void OnGameError(OnErrorEvent evt) => onGameError?.Invoke(evt);
        private void OnCashOut(OnCashOutEvent evt) => onCashOut?.Invoke(evt);
        private void OnPlay(OnPlayEvent evt) => onPlay?.Invoke(evt);
        private void OnConsult(OnConsultEvent evt) => onConsult?.Invoke(evt);
        private void OnDisconnected() => onDisconnect?.Invoke();
        private void OnConnected() => Invoke(nameof(Join), 2f);

        private async void Join()
        {
            await _minesGameClient.JoinLobby();
            onJoinLobby?.Invoke();
        }

        private void OnDestroy() => _minesGameClient.Dispose();
    }
}