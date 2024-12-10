using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class GraphicExtension : MonoBehaviour
{
    private Graphic graphic;

    private void Awake()
    {
        graphic = GetComponent<Graphic>();
    }
    
    public void SetColor(string colorHex)
    {
        if (ColorUtility.TryParseHtmlString(colorHex.Insert(0, "#"), out Color color))
        {
            graphic.color = color;
        }
    }
    
    public void SetAlpha(float alpha)
    {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
    }
}
