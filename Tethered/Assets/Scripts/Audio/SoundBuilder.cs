using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tethered.Audio
{
    public class SoundBuilder
    {
        private readonly SFXManager sfxManager;
        private SoundData soundData;
        private Vector3 position = Vector3.zero;
        private bool randomPitch;

        public SoundBuilder(SFXManager sfxManager)
        {
            this.sfxManager = sfxManager;
        }

        /// <summary>
        /// Add a SoundData to play
        /// </summary>
        public SoundBuilder WithSoundData(SoundData soundData)
        {
            this.soundData = soundData;
            return this;
        }

        /// <summary>
        /// Set a position to play the sound
        /// </summary>
        public SoundBuilder WithPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }

        /// <summary>
        /// Randomize the pitch of the sound
        /// </summary>
        public SoundBuilder WithRandomPitch()
        {
            randomPitch = true;
            return this;
        }

        /// <summary>
        /// Build and play the sound
        /// </summary>
        public void Play()
        {
            // Exit case - the sound cannot be played through the SFX Manager
            if (!sfxManager.CanPlaySound(soundData)) return;

            // Get a Sound Emitter
            SoundEmitter soundEmitter = sfxManager.Get();

            // Initialize the Sound Emitter using the SoundData
            soundEmitter.Initialize(soundData);

            // Set the position of the Sound Emitter
            soundEmitter.transform.position = position;
            soundEmitter.transform.parent = sfxManager.transform;

            // Check if the pitch needs to be randomized
            if(randomPitch)
                soundEmitter.WithRandomPitch();

            // Check if the sound is a frequent sound
            if(soundData.frequentSound)
                // Enqueue it into the list
                sfxManager.FrequentSoundEmitters.Enqueue(soundEmitter);

            // Play the sound
            soundEmitter.Play();
        }
    }
}