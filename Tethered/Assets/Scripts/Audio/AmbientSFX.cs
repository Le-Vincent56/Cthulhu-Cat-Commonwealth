using System.Linq;
using Tethered.Audio;
using Tethered.Patterns.ServiceLocator;
using Tethered.Timers;
using UnityEngine;

public class AmbientSFX : MonoBehaviour
{
    private SFXManager sfxManager;
    private SoundData lastSoundPlayed;
    [SerializeField] private SoundData[] ambientSounds;

    private CountdownTimer countdownTimer;
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;
    [SerializeField] private float chosenTime;

    private void Start()
    {
        sfxManager = ServiceLocator.ForSceneOf(this).Get<SFXManager>();

        // Create a new Countdown timer
        chosenTime = Random.Range(minTime, maxTime);
        countdownTimer = new CountdownTimer(chosenTime);

        // Hook up events
        countdownTimer.OnTimerStop += PlaySound;

        // Start the timer
        countdownTimer.Start();
    }

    private void PlaySound()
    {
        // Get the available sound options (no repeats)
        SoundData[] availableOptions = ambientSounds.Where(sd => sd != lastSoundPlayed).ToArray();

        // Get a random sound
        SoundData soundToPlay = availableOptions[Random.Range(0, availableOptions.Length)];

        // Play the sound
        sfxManager.CreateSound().WithSoundData(soundToPlay).Play();

        // Set the last sound played
        lastSoundPlayed = soundToPlay;

        // Set a new timer value for the Countdown Timer
        chosenTime = Random.Range(minTime, maxTime);
        countdownTimer.ChangeInitialTime(chosenTime);

        // Reset the timer and start it again
        countdownTimer.Reset();
        countdownTimer.Start();
    }
}
