using System;
using OpaGames.Blackcopter.UI.AutoFlowLayout;
using UnityEngine;

[Serializable]
public class OrientationAdjustment
{
    [HideInInspector] public RectTransform rectTransform => landscape.rectTransform;
    [SerializeField] private RectAjustment landscape;
    [SerializeField] private RectAjustment portrait;

    public void SetOrientation(bool isPortrait = false)
    {
        RectTransform rectTransform;
        RectAjustment orientation;
        if (isPortrait)
            orientation = portrait;
        else
            orientation = landscape;
        
        rectTransform = orientation.rectTransform;
        
        rectTransform.SetParent(orientation.parent);
        rectTransform.SetSiblingIndex(orientation.siblingIndex);
        rectTransform.anchoredPosition = orientation.anchoredPosition;
        rectTransform.sizeDelta = orientation.sizeDelta;
        rectTransform.anchorMin = orientation.anchorMin;
        rectTransform.anchorMax = orientation.anchorMax;
        rectTransform.pivot = orientation.pivot;
        rectTransform.localScale = orientation.scale;
    }
}
