using UnityEngine;

namespace UI.Responsiveness
{
    [System.Serializable]
    public struct OrientationValues
    {
        public Vector2 portrait, landscape;
    }
    [System.Serializable]
    public struct RectAjustment
    {
        public RectTransform rectTransform;
        public OrientationValues anchorMin, anchorMax;
        [Space]
        public OrientationValues anchoredPòsition;
        public OrientationValues sizeDelta;
        public OrientationValues scale;

        public void Ajust(bool portrait = false)
        {
            if (rectTransform == null) return;
            if (portrait)
            {
                rectTransform.anchorMin = anchorMin.portrait;
                rectTransform.anchorMax = anchorMax.portrait;
                rectTransform.anchoredPosition = anchoredPòsition.portrait;
                rectTransform.sizeDelta = sizeDelta.portrait;
                rectTransform.localScale = scale.portrait;
            }
            else
            {
                rectTransform.anchorMin = anchorMin.landscape;
                rectTransform.anchorMax = anchorMax.landscape;
                rectTransform.anchoredPosition = anchoredPòsition.landscape;
                rectTransform.sizeDelta = sizeDelta.landscape;
                rectTransform.localScale = scale.landscape;
            }
        }
    }
}