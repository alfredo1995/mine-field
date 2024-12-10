using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "new GameSettings", menuName = "Game/Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        public int gridSize = 25;
        public float minBet = 1.0f;
        public float maxBet = 700.00f;
        public int minMines = 1;
        public int maxMines = 24;
        public float maxPayout = 50.000f;
        public float defaultBet = 4.00f;
        public bool autoBetEnabled = true;
        public float autoDelayTime = 1.5f;
        public List<float> betValues = new() { 1, 2, 5, 10, 20, 50, 75, 100, 200, 300, 400, 500 };
        public List<int> autoRoundsValues = new() { 3, 10, 25, 150, 200, 500 };
    }
}