using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "new SoundBank", menuName = "Settings/SoundBank", order = 0)]
    public class SoundBank : ScriptableObject
    {
        public AudioClip startBetSfx;
        public AudioClip cellClickSfx;
        public AudioClip buttonHover;
        public AudioClip explodeSfx;
        public AudioClip winSfx;
    }
}