using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ws.DataTypes
{
    public struct OnUserChangeEvent
    {
        public UserStatus status;
        public string currency;
        public decimal balance;
    }
    
    public struct OnPlayEvent
    {
        public string currency;
        public double bet_value;
        public bool canCashout;
        public double payoutMultiplier;
        public double payoutMultiplierOnNext;
        public Status status;
        /// <summary>
        /// 0 -> Hidden
        /// 1 -> Revealed
        /// 2 -> Exploded
        /// </summary>
        public List<int> tiles;
    }
    
    public struct OnCashOutEvent
    {
        public double payoutMultiplier;
        /// <summary>
        /// 0 -> Hidden
        /// 1 -> Revealed
        /// 2 -> Exploded
        /// </summary>
        public List<int> tiles;
    }
    
    public struct OnUserBetsEvent
    {
        public List<UserHistory> minesHistory;
        [JsonProperty("total_pages")] public int totalPages;
    }

    public struct OnConsultEvent
    {
        public double payoutMultiplier;
    }
    
    public struct OnErrorEvent
    {
        public string message;
    }

    public struct UserHistory
    {
        public string id;
        public DateTime date;
        [JsonProperty("bet_value")] public decimal betValue;
        public double payoutMultiplier;
        public decimal cashout;
        public string clientHash;
        public string serverHash;
        public List<int> tiles;
        public List<int> positions_played;
    }

    public enum UserStatus : byte
    {
        Off,
        Spectating,
        Playing
    }

    public enum Status : byte
    {
        Cashout = 1,
        Crashed = 2,
        Canceled = 3
    }
    
    public enum EventName : byte
    {
        OnError = 3,
        OnPlay = 5,
        OnCashOut = 6,
        OnUserBets = 7,
        OnUserChange = 8,
        OnConsult = 9
    }
}