using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwapSwitchAnimation : SwitchAnimation
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite onSprite, offSprite;
    
    protected override IEnumerator Animation(bool isOn)
    {
        float elapsedTime = 0f;

        float start = !isOn ? 1f : 0f;
        float end = isOn ? 1f : 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        image.sprite = isOn ? onSprite : offSprite;
    }
}
