using System;
using System.Threading.Tasks;
using Settings;

namespace Ws.DataTypes
{
    public interface IMessageBroker : IConnection
    {
        void RegisterCallback(IMessageBrokerCallbacks target);
        void UnRegisterCallback(IMessageBrokerCallbacks target);

        Task Send(Commands.CommandName command, object obj);
    }

    public interface IConnection
    {
        event Action OnOpenEvent;
        event Action OnCloseEvent;
        event Action<string> OnErrorEvent;
        event Action<GameSettings> OnSettingsEvent;

        Task Connect();
        Task Disconnect();
    }
}