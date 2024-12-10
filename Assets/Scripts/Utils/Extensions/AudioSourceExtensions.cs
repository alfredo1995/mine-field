using UnityEngine;
using System.Collections;
namespace Utils.Extensions
{
    public static class AudioSourceExtensions
    {
        public static IEnumerator FadeOutAndPlayNewMusic(this AudioSource musicSource, AudioClip newClip, float fadeDuration)
        {
            float startVolume = musicSource.volume;
            float t;
            for (t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
            musicSource.Stop();
            musicSource.clip = newClip;
            musicSource.loop = true;
            musicSource.Play();

            for (t = fadeDuration; t >= 0; t -= Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }

            musicSource.volume = startVolume;
        }
    }
}