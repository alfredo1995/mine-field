using TMPro;
using UnityEngine;

[ExecuteAlways]
public class TextAutoStretch : MonoBehaviour
{
    [SerializeField] private float areaPadding = 10;
    [SerializeField] private TMP_Text text;
    private RectTransform _rectTransform;
    private void Update()
    {
        Adjust();
    }

    private void Adjust()
    {
        if (!_rectTransform) _rectTransform = (RectTransform)transform;
        var size = _rectTransform.sizeDelta;
        size.y = text.bounds.size.y + text.margin.y + text.margin.w + areaPadding;
        _rectTransform.sizeDelta = size;
    }
}
