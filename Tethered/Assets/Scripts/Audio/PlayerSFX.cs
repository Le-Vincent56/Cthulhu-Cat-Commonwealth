using System.Collections.Generic;
using Tethered.Patterns.ServiceLocator;
using Tethered.Timers;
using UnityEngine;

namespace Tethered.Audio
{
    public class PlayerSFX : MonoBehaviour
    {
        private SFXManager sfxManager;
        [SerializeField] private List<SoundData> footstepSounds = new();
        [SerializeField] private List<SoundData> ladderSounds = new();

        private CountdownTimer footstepTimer;
        [SerializeField] private float footstepTime;

        private CountdownTimer ladderTimer;
        [SerializeField] private float ladderTime;

        private void Awake()
        {
            // Create the footstep timer
            footstepTimer = new CountdownTimer(footstepTime);

            // Define a completion action
            footstepTimer.OnTimerStop += () =>
            {
                // Play the sound
                sfxManager.CreateSound()
                    .WithSoundData(GetRandomSound(footstepSounds))
                    .WithRandomPitch()
                    .Play();

                // Restart the footstep timer
                footstepTimer.Start();
            };

            // Create the ladder timer
            ladderTimer = new CountdownTimer(ladderTime);

            // Define a completion action
            ladderTimer.OnTimerStop += () =>
            {
                // Play the sound
                sfxManager.CreateSound()
                    .WithSoundData(GetRandomSound(ladderSounds))
                    .WithRandomPitch()
                    .Play();

                // Restart the ladder timer
                ladderTimer.Start();
            };
        }

        private void Start()
        {
            // Get the SFX Manager as a service
            sfxManager = ServiceLocator.ForSceneOf(this).Get<SFXManager>();
        }

        /// <summary>
        /// Get a random sound out of a list of SoundDatas
        /// </summary>
        private SoundData GetRandomSound(List<SoundData> soundDataList)
        {
            return soundDataList[Random.Range(0, soundDataList.Count)];
        }

        /// <summary>
        /// Enable footsteps by starting the timer
        /// </summary>
        public void EnableFootsteps()
        {
            footstepTimer = new CountdownTimer(footstepTime);

            footstepTimer.OnTimerStop += () =>
            {
                // Play the sound
                sfxManager.CreateSound()
                    .WithSoundData(GetRandomSound(footstepSounds))
                    .WithRandomPitch()
                    .Play();

                // Restart the footstep timer
                footstepTimer.Start();
            };

            footstepTimer.Start();
        }

        /// <summary>
        /// Disable footsteps by pausing the timer
        /// </summary>
        public void DisableFootsteps() => footstepTimer.Pause();

        /// <summary>
        /// Enable ladder sounds by starting the timer
        /// </summary>
        public void EnableLadder()
        {
            // Create the ladder timer
            ladderTimer = new CountdownTimer(ladderTime);

            // Define a completion action
            ladderTimer.OnTimerStop += () =>
            {
                // Play the sound
                sfxManager.CreateSound()
                    .WithSoundData(GetRandomSound(ladderSounds))
                    .WithRandomPitch()
                    .Play();

                // Restart the ladder timer
                ladderTimer.Start();
            };

            ladderTimer.Start();
        }

        /// <summary>
        /// Disable ladder sounds by pausing the timer
        /// </summary>
        public void DisableLadder() => ladderTimer.Pause();
    }
}
