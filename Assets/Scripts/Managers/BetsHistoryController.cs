using System;
using BetInstancer;
using UnityEngine;
using UnityEngine.UI;
using Ws.DataTypes;

namespace Managers
{
    public sealed class BetsHistoryController : MonoBehaviour, IDisposable
    {
        [Header("Service")]
        [SerializeField] private ServiceController serviceController;
        
        [Header("Components")]
        [SerializeField] private BetsHistoryInstancer betsHistoryInstancer;
        [SerializeField] private Button betHistoryButton;
        [SerializeField] private Button loadMoreButton;
        [SerializeField] private GameObject tmpText;
        [SerializeField] private GameObject loadingCircle;
        
        [Header("Scroll View Holder")]
        [SerializeField] private GameObject scrollViewContent;
        [SerializeField] private Button scrollViewCloseButton;
        
        [Header("Configs")]
        [SerializeField] private int contentPerPage = 5;
        
        private int _currentPage;
        private bool _hasContent;

        public void OnEnable()
        {
            scrollViewCloseButton.onClick.AddListener(CloseContent);
            scrollViewCloseButton.onClick.AddListener(Dispose);
            betHistoryButton.onClick.AddListener(GetBetsHistory);
            serviceController.onUserBets += OnUserBets;
            loadMoreButton.onClick.AddListener(GetBetsHistory);
            tmpText.SetActive(true);
            loadingCircle.SetActive(false);
        }

        public void OnDisable()
        {
            serviceController.onUserBets -= OnUserBets;
            scrollViewCloseButton.onClick.RemoveListener(CloseContent);
            scrollViewCloseButton.onClick.RemoveListener(Dispose);
            betHistoryButton.onClick.RemoveListener(GetBetsHistory);
            loadMoreButton.onClick.RemoveAllListeners();
        }

        private async void GetBetsHistory()
        {
            tmpText.SetActive(false);
            loadingCircle.SetActive(true);
            _currentPage++;
            await serviceController.MinesGameClient.UserBets(_currentPage, contentPerPage, -1);
        }

        private void OnUserBets(OnUserBetsEvent evt)
        {
            loadingCircle.SetActive(false);
            tmpText.SetActive(true);
            
            loadMoreButton.interactable = evt.minesHistory.Count <= contentPerPage && evt.minesHistory.Count > 0;
            if(evt.minesHistory.Count <= 0) return;
            
            _hasContent = true;
            betsHistoryInstancer.AddRange(evt.minesHistory);
        }

        private void CloseContent()
        {
            if(!scrollViewContent.activeSelf) return;
            scrollViewContent.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            _currentPage = 0;

            if(!_hasContent) return;
            if(scrollViewContent.activeSelf) return;

            _hasContent = false;
            betsHistoryInstancer.Dispose();
        }
    }
}