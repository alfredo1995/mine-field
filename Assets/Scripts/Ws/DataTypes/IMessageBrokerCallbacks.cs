namespace Ws.DataTypes
{
    public interface IMessageBrokerCallbacks
    {
        void OnError(OnErrorEvent evt);
        void OnUserChange(OnUserChangeEvent evt);
        void OnPlay(OnPlayEvent evt);
        void OnUserBets(OnUserBetsEvent evt);
        void OnCashOut(OnCashOutEvent evt);
        void OnConsult(OnConsultEvent evt);
    }
}