using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Settings;
using UnityEngine;
using Utils.Extensions;
using Ws.DataTypes;

namespace Ws
{
    public sealed class MinesGameClient : MonoBehaviour, IDisposable, IMessageBrokerCallbacks
    {
        private IMessageBroker _messageBroker;
        public IMessageBroker MessageBroker
        {
            get => _messageBroker;
            set
            {
                if (_messageBroker != null)
                {
                    _messageBroker.OnOpenEvent -= OnOpen;
                    _messageBroker.OnCloseEvent -= OnClose;
                    _messageBroker.OnErrorEvent -= OnError;
                    _messageBroker.OnSettingsEvent -= OnSettings;
                    _messageBroker.UnRegisterCallback(this);
                }

                _messageBroker = value;
                _messageBroker.OnOpenEvent += OnOpen;
                _messageBroker.OnCloseEvent += OnClose;
                _messageBroker.OnErrorEvent += OnError;
                _messageBroker.OnSettingsEvent += OnSettings;
                _messageBroker.RegisterCallback(this);
            }
        }

        public BrokerStatus Status;
        
        public Action onConnected;
        public Action onDisconnected;
        public Action<GameSettings> onSettings;
        public Action<string> onError;
        public Action<OnErrorEvent> onGameError;
        public Action<OnPlayEvent> onPlay;
        public Action<OnCashOutEvent> onCashOut;
        public Action<OnUserChangeEvent> onUserChange;
        public Action<OnUserBetsEvent> onUserBets;
        public Action<OnConsultEvent> onConsult;
        
        /// <summary>
        /// Connect to WebSocket or Js instance using `accessToken` 
        /// </summary>
        /// <returns></returns>
        public async Task Connect()
        {
            Status = BrokerStatus.Connecting;
            await MessageBroker.Connect();
            Status = BrokerStatus.Open;
        }
        
        /// <summary>
        /// Disconnect from WebSocket
        /// </summary>
        /// <returns></returns>
        public async Task Disconnect()
        {
            Status = BrokerStatus.Closing;
            await MessageBroker.Disconnect();
            Status = BrokerStatus.Closed;
        }
        
        /// <summary>
        /// Send Join Lobby command to websocket server
        /// </summary>
        /// <returns></returns>
        public Task JoinLobby() => MessageBroker.Send(Commands.CommandName.JoinLobby, new Commands.JoinLobbyCommand(Application.isMobilePlatform ? "mobile" : "desktop"));
        public Task StartGame(decimal amount, int numberOfMines, string clientHash) => MessageBroker.Send(Commands.CommandName.Start, new Commands.StartCommand(amount.TruncateDecimal(2), numberOfMines, clientHash));
        public Task PlayGame(int position) => MessageBroker.Send(Commands.CommandName.Play, new Commands.PlayCommand(position));
        public Task CashOut() => MessageBroker.Send(Commands.CommandName.CashOut, new Commands.EmptyCommand());
        public Task UserBets(int page, int amount, int orderBy) => MessageBroker.Send(Commands.CommandName.UserBets, new Commands.UserBetsCommand(page, amount, orderBy));
        public Task Consult(int numberOfMines, int positions) => MessageBroker.Send(Commands.CommandName.Consult, new Commands.ConsultCommand(numberOfMines, positions));
        public Task PlayAuto(int numberOfMines, List<int> positions, decimal amount, string clientHash) => MessageBroker.Send(Commands.CommandName.PlayAuto, new Commands.PlayAutoCommand(numberOfMines, positions, amount.TruncateDecimal(2), clientHash));
        public async void Dispose() => await Disconnect();
        private void OnOpen() => onConnected?.Invoke();
        private void OnClose() => onDisconnected?.Invoke();
        private void OnError(string error) => onError?.Invoke(error);
        public void OnError(OnErrorEvent evt) => onGameError?.Invoke(evt);
        public void OnUserChange(OnUserChangeEvent evt) => onUserChange?.Invoke(evt);
        public void OnPlay(OnPlayEvent evt) => onPlay?.Invoke(evt);
        public void OnUserBets(OnUserBetsEvent evt) => onUserBets?.Invoke(evt);
        public void OnCashOut(OnCashOutEvent evt) => onCashOut?.Invoke(evt);
        public void OnConsult(OnConsultEvent evt) => onConsult?.Invoke(evt);
        private void OnSettings(GameSettings settings) => onSettings?.Invoke(settings);
    }
    
    public enum BrokerStatus
    {
        Closed = 0,
        Open = 1,
        Connecting = 2,
        Closing = 3
    }
}