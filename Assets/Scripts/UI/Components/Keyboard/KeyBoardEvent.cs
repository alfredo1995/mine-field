using System;
using UnityEngine;

namespace UI.Components.Keyboard
{
    public class KeyBoardEvent : MonoBehaviour
    {
        public KeyCode keyCode;
        
        public event Action OnKeyDown;
        private void Update()
        {
            if(Input.GetKeyDown(keyCode)) OnKeyDown?.Invoke();
        }
    }
}