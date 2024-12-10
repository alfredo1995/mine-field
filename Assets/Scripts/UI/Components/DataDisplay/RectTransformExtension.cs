using System.Collections.Generic;
using OpaGames.Blackcopter.UI.AutoFlowLayout;
using UnityEngine;

namespace UI.Components.DataDisplay
{
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformExtension : MonoBehaviour
    {
        private RectTransform rectTransform;

        public List<RectAjustment> rectPresets;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
    
        public void SetPreset(int index)
        {
            if (index < 0 || index >= rectPresets.Count) return;
            RectAjustment adjustment = rectPresets[index];
            rectTransform.SetParent(adjustment.parent);
            rectTransform.SetSiblingIndex(adjustment.siblingIndex);
            rectTransform.anchoredPosition = adjustment.anchoredPosition;
            rectTransform.anchorMin = adjustment.anchorMin;
            rectTransform.anchorMax = adjustment.anchorMax;
            rectTransform.pivot = adjustment.pivot;
            rectTransform.sizeDelta = adjustment.sizeDelta;
            rectTransform.localScale = adjustment.scale;
        }
    }
}
