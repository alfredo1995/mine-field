using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UI.Components;
using UnityEngine;

namespace BetInstancer
{
    public class BetHistoryPopup : MonoBehaviour
    {
        [SerializeField] private HistoryData historyData;
        [SerializeField] private TMP_Text idText;
    
        [SerializeField] private TMP_Text serverSeedText;
        [SerializeField] private TMP_Text clientSeedText;
    
        [SerializeField] private TMP_Text minesText;
        [SerializeField] private GridContainer gridContainer;
    
        [SerializeField] private GameObject parentView;

        public void SetData(DateTime dateTime, string id, decimal betAmount, double payoutMultiplier, decimal cashOut, string serverSeed, string clientSeed, List<int> tiles, List<int> clickedTiles)
        {
            historyData.SetData(dateTime, betAmount, cashOut, (float)payoutMultiplier);
            
            idText.text = $"id: {id}";

            serverSeedText.text = serverSeed;
            clientSeedText.text = clientSeed;

            var bombs = tiles.Count(tile => tile == 2);

            minesText.text = $"{GameManager.Locale["history_mines"]}: <b>{bombs}</b>";
            gridContainer.SetGrid(tiles, clickedTiles);
        }
    
        public void Show()
        {
            parentView.gameObject.SetActive(true);
        }
    
        public void Hide()
        {
            parentView.gameObject.SetActive(false);
            gridContainer.ClearGrid();
        }
    }
}