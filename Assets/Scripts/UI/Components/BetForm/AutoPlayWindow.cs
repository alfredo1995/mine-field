using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    public class AutoPlayWindow : MonoBehaviour
    {
        [SerializeField] List<NumberRoundsButton> roundsButtons;
        [SerializeField] Button start, close;
        private NumberRoundsButton _current;
        [SerializeField] private GameObject panel;

        public event Action<AutoPlayForm> onAutoPlay;

        private void Awake()
        {
            Init();
            start.onClick.AddListener(StartAutoGame);
            close.onClick.AddListener(CloseWindow);
        }

        private void Init()
        {
            _current = roundsButtons[0];
            _current.SetSelected(true);
            
            foreach (var roundButton in roundsButtons)
            {
                roundButton.Button.onClick.AddListener(() => SelectRoundNumber(roundButton));
            }
        }

        public void SetRounds(List<int> rounds)
        {
            for (int i = 0; i < roundsButtons.Count; i++)
            {
                roundsButtons[i].Value.SetText(rounds[i].ToString());
            }
        }

        private void SelectRoundNumber(NumberRoundsButton roundButton)
        {
            _current.SetSelected(false);
            _current = roundButton;
            _current.SetSelected(true);
        }

        private void StartAutoGame()
        {
            var rounds = int.Parse(_current.Value.text);
            var autoPlayForm = new AutoPlayForm(rounds);
            onAutoPlay?.Invoke(autoPlayForm);
            CloseWindow();
        }

        private void CloseWindow()
        {
            panel.SetActive(false);
        }

        private void OnDestroy()
        {
            start.onClick.RemoveAllListeners();
            close.onClick.RemoveAllListeners();

            foreach (var roundButton in roundsButtons)
            {
                roundButton.Button.onClick.RemoveAllListeners();
            }
            
            _current.SetSelected(false);
        }
    }

    public struct AutoPlayForm
    {
        public readonly int rounds;

        public AutoPlayForm(int rounds)
        {
            this.rounds = rounds;
        }
    }
}