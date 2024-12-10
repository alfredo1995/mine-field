using Newtonsoft.Json;
using UnityEngine;

namespace Ws.DataTypes
{
    public class Commands : MonoBehaviour
    {
        public struct EmptyCommand { } 
        
        public struct JoinLobbyCommand
        {
            public string device;

            public JoinLobbyCommand(string device)
            {
                this.device = device;
            }
        }
        
        public struct StartCommand
        {
            public decimal bet_value;
            public int numberOfMines;
            public string clientHash;
            
            public StartCommand(decimal amount, int numberOfMines, string clientHash)
            {
                bet_value = amount;
                this.numberOfMines = numberOfMines;
                this.clientHash = clientHash;
            }
        }
        
        public struct PlayCommand
        {
            public int position;
            
            public PlayCommand(int position)
            {
                this.position = position;
            }
        }
        
        public struct UserBetsCommand
        {
            public int page;
            public int amount;
            [JsonProperty("order_by")] public int orderBy;
            
            public UserBetsCommand(int page, int amount, int orderBy)
            {
                this.page = page;
                this.amount = amount;
                this.orderBy = orderBy;
            }
        }

        public struct ConsultCommand
        {
            public int numberOfMines;
            public int positions;
            
            public ConsultCommand(int numberOfMines, int positions)
            {
                this.numberOfMines = numberOfMines;
                this.positions = positions;
            }
        }
        
        public struct PlayAutoCommand
        {
            public int numberOfMines;
            public System.Collections.Generic.List<int> positions;
            public decimal bet_value;
            public string clientHash;
            
            public PlayAutoCommand(int numberOfMines, System.Collections.Generic.List<int> positions, decimal amount, string clientHash)
            {
                bet_value = amount;
                this.numberOfMines = numberOfMines;
                this.positions = positions;
                this.clientHash = clientHash;
            }
        }

        public enum CommandName
        {
            JoinLobby = 2,
            Start = 3,
            Play = 4,
            CashOut = 5,
            UserBets = 6, 
            Consult = 7,
            PlayAuto = 8
        }
        
        
    }
}