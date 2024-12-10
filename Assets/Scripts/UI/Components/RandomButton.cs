using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using OpaGames.Forms;

namespace UI.Components
{
    public class RandomButton : FormAddOn
    {
        [SerializeField] private Button myButton;
        private List<int> _samplingArea = new();
        [SerializeField] private Graphic secondTarget;

        [SerializeField] private Color enableColor;
        
        [SerializeField] private Color disableColor;

        public bool IsInteractable
        {
            set => SetInteractable(value);
        }

        public event Action<int> onRandomize;

        private void Click()
        {
            if (_samplingArea.Count <= 0) return;
            var number = _samplingArea[UnityEngine.Random.Range(0, _samplingArea.Count)];
            RemoveSamplingNumber(number);
            onRandomize?.Invoke(number);
        }

        public void CreateSamplingArea(int size)
        {
            _samplingArea = new List<int>(size);
            for (var i = 0; i < size; i++) _samplingArea.Add(i);
        }

        public void PopulateSamplingArea(int size)
        {
            _samplingArea.Clear();
            for (var i = 0; i < size; i++) _samplingArea.Add(i);
        }

        public void RemoveSamplingNumber(int number)
        {
            if (_samplingArea.Count <= 0) return;
            _samplingArea.Remove(number);
        }

        public void AddSamplingNumber(int number)
        {
            if (_samplingArea.Contains(number)) return;
            _samplingArea.Add(number);
        }
        
        public List<int> GetRemainingSamplingArea()
        {
            var remainingSamplingArea = new List<int>();
            
            foreach (var number in _samplingArea)
            {
                remainingSamplingArea.Add(number);
            }
            
            return remainingSamplingArea;
        }

        public override void SetInteractable(bool value)
        {
            myButton.interactable = value;
            secondTarget.color = value ? enableColor : disableColor;
        }

        #region OnEnable/OnDisable

        private void OnEnable()
        {
            myButton.onClick.AddListener(Click);
        }

        private void OnDisable()
        {
            _samplingArea.Clear();
            myButton.onClick.RemoveListener(Click);
        }

        #endregion
    }
}