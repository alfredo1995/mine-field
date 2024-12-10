using System;
using UnityEngine;

namespace OpaGames.Blackcopter.UI.AutoFlowLayout
{
    public class AutoFlowLayout : MonoBehaviour
    {
        [Header("Main Rect Settings")] [SerializeField]
        private RectTransform canvasRect;
        public static Vector2 screenSize { get; private set; }
        public static bool IsLandScape { get; private set; }
        public static Action OnPortrait;
        public static Action OnLandscape;
        public static Action OnChangeSize;

        private void Start()
        {
            var rect = canvasRect.rect;
            var resultIsLandScape = (int)rect.width >= (int)rect.height;
            IsLandScape = !resultIsLandScape;
            Readjustment((int)rect.width, (int)rect.height);
        }

        private void Update()
        {
            var rect = canvasRect.rect;
            Readjustment((int)rect.width, (int)rect.height);
        }

        /// <param name="width">width of the screen</param>
        /// <param name="height"> height of the screen</param>
        private void Readjustment(int width, int height)
        {
            if (CheckIsLandScape(width, height))
            {
                LandscapeAdjust();
            }
            else
            {
                PortraitAdjust();
            }
            if(Math.Abs(screenSize.x - width) > Mathf.Epsilon 
               || Math.Abs(screenSize.y - height) > Mathf.Epsilon)
            {
                OnChangeSize?.Invoke();
                screenSize = new(width, height);
            }
            
        }
        private bool CheckIsLandScape(int width, int height)
        {
            var resultIsLandScape = width >= height;
            if(resultIsLandScape && !IsLandScape)
                OnLandscape?.Invoke();
            else if(!resultIsLandScape && IsLandScape)
                OnPortrait?.Invoke();
            return resultIsLandScape;
        }
        private void LandscapeAdjust()
        {
            IsLandScape = true;
        }

        private void PortraitAdjust()
        {
            IsLandScape = false;
        }
    }
}