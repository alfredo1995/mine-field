using System.Collections;
using Settings;
using UnityEngine;
using Utils.Extensions;

namespace Managers
{
    public class AudioController : MonoBehaviour
    { 
        [SerializeField] private SoundBank soundBank;
        [SerializeField] private AudioSource audioSource, musicSource;
        private float fadeDuration = 1.0f;

        public void PlayBGMusic()
        {
            StartCoroutine(musicSource.FadeOutAndPlayNewMusic(soundBank.startBetSfx, fadeDuration));
        }

        public void PlayMusic()
        {
            musicSource.Play();
            StartCoroutine(AwaitEndMusic());
        }

        private IEnumerator AwaitEndMusic()
        {
            while (musicSource.isPlaying)
            {
                if (musicSource.clip.length - musicSource.time <= fadeDuration)
                {
                    StartCoroutine(musicSource.FadeOutAndPlayNewMusic(soundBank.startBetSfx, fadeDuration));
                    yield break;
                }
                yield return null;
            }
        }

        public void PlayCellClickSfx()
        {
            PlayOneSound(soundBank.cellClickSfx);
        }

        public void PlayButtonHoverSfx()
        {
            PlayOneSound(soundBank.buttonHover);
        }

        public void PlayExplodeSfx()
        {
            PlayOneSound(soundBank.explodeSfx);
        }

        public void PlayWinSfx()
        {
            PlayOneSound(soundBank.winSfx);
        }

        private void PlayOneSound(AudioClip clip)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(clip);
        }

        public void ToggleMusic(bool value)
        {
            musicSource.mute = value;
        }

        public void ToggleSound(bool value)
        {
            audioSource.mute = value;
        }
    }
}