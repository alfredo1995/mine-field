using System;
using UnityEngine;

namespace OpaGames.Blackcopter.UI.AutoFlowLayout
{
    [Serializable]
    public struct RectAjustment
    {
        public RectTransform parent;
        public int siblingIndex;
        public RectTransform rectTransform;
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector2 scale;
    }
}
