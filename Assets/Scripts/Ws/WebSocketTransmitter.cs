using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using Settings;
using UnityEngine;
using UnityEngine.Networking;
using Ws.DataTypes;
using Ws.DataTypes.NoTask.CrashGamePlatform.DataTypes;

namespace Ws
{
    public class WebSocketTransmitter : MonoBehaviour, IMessageBroker
    {
        private readonly List<IMessageBrokerCallbacks> _callbacks = new();
        private WebSocket _websocket;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        private bool IsOpen => _websocket != null && _websocket.State == WebSocketState.Open;
        private bool IsConnecting => _websocket != null && _websocket.State == WebSocketState.Connecting;
        public Endpoint Endpoint { get; set; }

        public event Action OnOpenEvent;
        public event Action OnCloseEvent;
        public event Action<string> OnErrorEvent;
        public event Action<GameSettings> OnSettingsEvent; 

        private Task Send(byte[] data)
        {
            if (!IsOpen)
            {
                OnErrorEvent?.Invoke("You are not connected!");
                return Task.CompletedTask;
            }
            var task = _websocket.Send(data);
            return task;
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        private void Update()
        {
            if (IsOpen) _websocket.DispatchMessageQueue();
        }
#endif

        public async Task Disconnect()
        {
            if (!IsOpen)
            {
                OnErrorEvent?.Invoke("You are not connected");
                return;
            }

            await _websocket.Close();

            _websocket.OnOpen -= OnOpen;
            _websocket.OnClose -= OnClose;
            _websocket.OnError -= OnError;
            _websocket.OnMessage -= OnMessage;
        }
        public Task Connect()
        {
            if (IsOpen)
            {
                OnErrorEvent?.Invoke("You Already Connected");
                return Task.CompletedTask;
            }

            if (IsConnecting)
            {
                OnErrorEvent?.Invoke("You current connecting...");
                return Task.CompletedTask;
            }

            //Get the token from the server
            var token = Endpoint["playerSession"] ?? Endpoint["user_token"] ?? "none";
            var server = Endpoint["server_url"] ?? "none";
            var platform = Endpoint["platform_token"] ?? Endpoint["platform_id"] ?? "none";
            var casinoId = Endpoint["casino_id"] ?? Endpoint["operator"] ?? "none";
            var gameId = Endpoint["game_token"] ?? Endpoint["game_id"] ?? Endpoint["gameId"] ?? "none";
            var language = Endpoint["language"] ?? Endpoint["lang"] ?? "en";

            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("lang", language == "pt" ? "pt" : "en");
            PlayerPrefs.Save();

            _websocket = new WebSocket($"{server}?user_token={token}&platform_token={platform}&casino_id={casinoId}&game_id={gameId}");

            _websocket.OnOpen += OnOpen;
            _websocket.OnClose += OnClose;
            _websocket.OnError += OnError;
            _websocket.OnMessage += OnMessage;

            StartCoroutine(GetRequest(OnSuccess));

            return Task.CompletedTask;
        }

        private async void OnSuccess()
        {
            await _websocket.Connect();
        }

        public void RegisterCallback(IMessageBrokerCallbacks target)
        {
            _callbacks.Add(target);
        }
        public void UnRegisterCallback(IMessageBrokerCallbacks target)
        {
            _callbacks.Remove(target);
        }
        public async Task Send(Commands.CommandName command, object obj)
        {
            await _semaphoreSlim.WaitAsync();
            
            var raw = JsonConvert.SerializeObject(obj);
            var data = Encoding.UTF8.GetBytes(raw);
            var bytes = new byte[data.Length + 1];

            bytes[0] = (byte)command;
            Buffer.BlockCopy(data, 0, bytes, 1, data.Length);
            await Send(bytes);

            _semaphoreSlim.Release();
        }
        private T ParseEvent<T>(byte[] bytes) where T : struct
        {
            var raw = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(raw);
        }
        private void OnOpen() => OnOpenEvent?.Invoke();
        private void OnClose(WebSocketCloseCode closecode) => OnCloseEvent?.Invoke();
        private void OnError(string errorMsg) => OnErrorEvent?.Invoke(errorMsg);
        private void OnMessage(byte[] message)
        {
            var eventType = (EventName)message[0];
            var data = new byte[message.Length - 1];
            Buffer.BlockCopy(message, 1, data, 0, data.Length);

            switch (eventType)
            {
                case EventName.OnError:
                    var errorEvent = ParseEvent<OnErrorEvent>(data);
                    _callbacks.ForEach(callback => callback.OnError(errorEvent));
                    break;
                case EventName.OnPlay:
                    var playEvent = ParseEvent<OnPlayEvent>(data);
                    _callbacks.ForEach(callback => callback.OnPlay(playEvent));
                    break;
                case EventName.OnUserChange:
                    var userChangeEvent = ParseEvent<OnUserChangeEvent>(data);
                    _callbacks.ForEach(callback => callback.OnUserChange(userChangeEvent));
                    break;
                case EventName.OnCashOut:
                    var cashOutEvent = ParseEvent<OnCashOutEvent>(data);
                    _callbacks.ForEach(callback => callback.OnCashOut(cashOutEvent));
                    break;
                case EventName.OnUserBets:
                    var userBetsEvent = ParseEvent<OnUserBetsEvent>(data);
                    _callbacks.ForEach(callback => callback.OnUserBets(userBetsEvent));
                    break;
                case EventName.OnConsult:
                    var consultEvent = ParseEvent<OnConsultEvent>(data);
                    _callbacks.ForEach(callback => callback.OnConsult(consultEvent));
                    break;
                default:
                    throw new ArgumentException("Unknown event type");
            }
        }

        private IEnumerator GetRequest(Action onSuccess)
        {
            using var request = UnityWebRequest.Get($"{Endpoint.GetBaseEndpoint("https", "server_url").Replace("/ws/", string.Empty)}/settings");
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                var settings = Resources.Load<GameSettings>("SettingsData");
                JsonUtility.FromJsonOverwrite(request.downloadHandler.text, settings);
                OnSettingsEvent?.Invoke(settings);

                onSuccess?.Invoke();
            }
            else
            {
                var errorRequestSettings = new OnErrorEvent
                {
                    message = request.error
                };
                _callbacks.ForEach(callback => callback.OnError(errorRequestSettings));
            }
        }

        private async void OnDestroy()
        {
            if (IsOpen)
                await Disconnect();
        }
    }
}