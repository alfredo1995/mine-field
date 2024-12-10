using UnityEngine;
using Ws.DataTypes.NoTask.CrashGamePlatform.DataTypes;
using Object = UnityEngine.Object;

namespace Ws
{
    public static class MinesGameClientFactory
    {
        private const string CLIENT_NAME = "MinesGamePlatform";

        public static MinesGameClient Create()
        {
            var go = new GameObject(CLIENT_NAME);
            var client = go.AddComponent<MinesGameClient>();
            var websocket = go.AddComponent<WebSocketTransmitter>();

            Object.DontDestroyOnLoad(go);

            client.MessageBroker = websocket;

#if UNITY_EDITOR || DEVELOPMENT_BUILD

            const string serverUrl = "wss://vm-mines-hml.eastus.cloudapp.azure.com/ws/";
            const string gameId = "mines_12premium";
            const string platformToken = "65da997069bc6e5c6c83f23e560f181e";
            var devUrl = $"https://vm-mines-glogic.brazilsouth.cloudapp.azure.com/mines/?platform_token={platformToken}&lang=pt&gameMode=fun&casino_id=opagames&playerSession=opa-mikagay&server_url={serverUrl}&game_id={gameId}";
            websocket.Endpoint = new Endpoint(devUrl);
#else
            websocket.Endpoint = new Endpoint(Application.absoluteURL);
#endif
            return client;
        }
    }
}