using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Tethered.Audio
{
    [Serializable]
    public class SoundData
    {
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        public bool loop;
        public bool playOnAwake;
        public bool frequentSound;
    }
}