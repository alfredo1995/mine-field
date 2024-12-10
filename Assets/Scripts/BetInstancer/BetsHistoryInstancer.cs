using System;
using System.Collections.Generic;
using UI.Components;
using UnityEngine;
using Ws.DataTypes;

namespace BetInstancer
{
    public sealed class BetsHistoryInstancer : MonoBehaviour, IDisposable
    {
        [SerializeField] private RectTransform contentHolder;
        [SerializeField] private bool isDescending;
        
        [SerializeField] private BetHistoryPopup betHistoryPopup;
        public void AddRange(List<UserHistory> histories)
        {
            var prefab = Resources.LoadAsync<HistoryData>("HistoryData_[Item]").asset as HistoryData;
            var historiesCount = histories.Count;
            
            for (var i = 0; i < historiesCount; i++)
            {
                var history = histories[i];

                var data = Instantiate(prefab, contentHolder);
                
                var localTime = history.date.ToLocalTime();
                
                data.SetData(localTime, history.betValue, history.cashout, history.payoutMultiplier);
                
                var id = i + 1;
                void OnHistoryClick()
                {
                    betHistoryPopup.SetData(localTime,
                        id.ToString(),
                        history.betValue,  
                        history.payoutMultiplier,
                        history.cashout,
                        history.serverHash,
                        history.clientHash,
                        history.tiles,
                        history.positions_played);

                    betHistoryPopup.Show();
                }
                
                data.OnClick.AddListener(OnHistoryClick);

                if (isDescending)
                    data.transform.SetAsLastSibling();
                else
                    data.transform.SetAsFirstSibling();
            }
        }

        public void Dispose()
        {
            for (var i = contentHolder.childCount - 1; i >= 0; i--)
            {
                Destroy(contentHolder.GetChild(i).gameObject);
            }
            Resources.UnloadUnusedAssets();
        }
    }
}