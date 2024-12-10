using System.Collections;
using UnityEngine;

namespace UI.Components
{
    public class ScriptAnimations : MonoBehaviour
    {
        public static IEnumerator ScaleAnimation(Transform target, float startScale, float endScale, float duration)
        {
            var elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                var time = elapsedTime / duration;
                var scale = Mathf.Lerp(startScale, endScale, time);
                target.localScale = new Vector3(scale, scale, scale);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            target.localScale = new Vector3(endScale, endScale, endScale);
        }
        public static IEnumerator Blast(Cell currentCell, Vector2 direction, float initialDelay = .6f,float duration = .3f, float displacementRate = .08f)
        {
            if (initialDelay > 0) yield return new WaitForSeconds(initialDelay);
            RectTransform cell = (RectTransform)currentCell.transform;

            Vector2 basePosition = cell.anchoredPosition;

            float maxDistance = cell.rect.width * displacementRate;

            yield return MoveTowards(cell, basePosition, direction, maxDistance, duration);
            Vector2 cachePosition = cell.anchoredPosition;
            yield return MoveTowards(cell, cachePosition, basePosition - cachePosition, 1, duration);
        }
        public static IEnumerator Shake(Transform target, Vector2 initialDirection, float initialDelay = .5f, float duration = .4f, float displacementRate = .05f)
        {
            if (initialDelay > 0) yield return new WaitForSeconds(initialDelay);
            RectTransform cell = (RectTransform)target;

            Vector2 basePosition = cell.anchoredPosition;

            duration = duration/5f;
            float maxDistance = cell.rect.width * displacementRate;

            Vector2 finalDirection = initialDirection;
            finalDirection.x += Mathf.Abs(Mathf.Abs(finalDirection.x) - 1) * Random.Range(-1f, 1.1f);
            finalDirection.y += Mathf.Abs(Mathf.Abs(finalDirection.y) - 1) * Random.Range(-1f, 1.1f);
            //1
            yield return MoveTowards(cell, basePosition, finalDirection, maxDistance, duration);
            //2
            Vector2 cachePosition = cell.anchoredPosition;
            finalDirection = -initialDirection;
            finalDirection.x += Mathf.Abs(Mathf.Abs(finalDirection.x) - 1) * Random.Range(-1f, 1.1f);
            finalDirection.y += Mathf.Abs(Mathf.Abs(finalDirection.y) - 1) * Random.Range(-1f, 1.1f);
            yield return MoveTowards(cell, cachePosition, finalDirection, maxDistance, duration);
            //3
            cachePosition = cell.anchoredPosition;
            finalDirection = initialDirection;
            finalDirection.x += Mathf.Abs(Mathf.Abs(finalDirection.x) - 1) * Random.Range(-1f, 1.1f) * .7f;
            finalDirection.y += Mathf.Abs(Mathf.Abs(finalDirection.y) - 1) * Random.Range(-1f, 1.1f) * .7f;
            yield return MoveTowards(cell, cachePosition, finalDirection, maxDistance, duration);
            //4
            cachePosition = cell.anchoredPosition;
            finalDirection = -initialDirection;
            finalDirection.x += Mathf.Abs(Mathf.Abs(finalDirection.x) - 1) * Random.Range(-1f, 1.1f) * .7f;
            finalDirection.y += Mathf.Abs(Mathf.Abs(finalDirection.y) - 1) * Random.Range(-1f, 1.1f) * .7f;
            yield return MoveTowards(cell, cachePosition, finalDirection, maxDistance, duration);

            //back to original position
            yield return MoveTowards(cell, cachePosition, basePosition-cachePosition, 1, duration);

            cell.anchoredPosition = basePosition;
        }
        static IEnumerator MoveTowards(RectTransform obj, Vector2 origin, Vector2 direction, float maxDistance, float duration = 1)
        {
            float currentTime = 0;
            do
            {
                yield return null;
                currentTime += Time.deltaTime / duration;
                obj.anchoredPosition = origin + direction * (currentTime * maxDistance);

            } while (currentTime < 1);
        }
    }
}