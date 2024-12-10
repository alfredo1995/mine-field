using System.Collections;
using UnityEngine;

namespace PopupSystem
{
    public class CanvasUtility : MonoBehaviour
    {
        public static IEnumerator Fade(CanvasGroup cGroup, float startAlpha, float endAlpha, float duration)
        {
            float startTime = Time.time;
            float alpha = startAlpha;

            if (duration > 0f)
            {
                //Anim start
                while (Mathf.Abs(alpha - endAlpha) > .001)
                {
                    alpha = Mathf.Lerp(startAlpha, endAlpha, (Time.time - startTime) / duration);
                    cGroup.alpha = alpha;

                    yield return null;
                }
            }

            cGroup.alpha = endAlpha;
        }
    }
}