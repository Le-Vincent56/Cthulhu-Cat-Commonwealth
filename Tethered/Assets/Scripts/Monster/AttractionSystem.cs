using System.Collections;
using Tethered.Audio;
using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;
using Tethered.Patterns.ServiceLocator;
using Tethered.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tethered.Monster
{
    public class AttractionSystem : MonoBehaviour
    {
        [Header("Thresholds")]
        [SerializeField] private float currentThreshold;
        [SerializeField] private int[] bufferPoints;

        [Header("Attraction Level")]
        [SerializeField] private float attractionLevel; //don't reference directly use property
        [SerializeField] private float attractionLevelMax;
        [SerializeField] private float decreaseAttractionRate;
        private bool canGainAttraction;

        [Header("Timers")]
        [SerializeField] private float decreaseBufferTime;
        [SerializeField] private float decreaseAttractionTime;
        private CountdownTimer decreaseBufferTimer;
        private FrequencyTimer decreaseAttractionTimer;

        [Header("SFX")]
        [SerializeField] private SoundData[] thresholdSFX;
        private SFXManager sfxManager;

        private EventBinding<IncreaseAttraction> onIncreaseAttraction;
        
        // properties
        
        /// <summary>
        /// Set up to always trigger AttractionChanged event when attraction level changes.
        /// Used for Fog of War, UI, and other systems that need to know total attraction level.
        /// </summary>
        public float AttractionLevel
        {
            get => attractionLevel;
            private set
            {
                attractionLevel = value;
                EventBus<AttractionChanged>.Raise(new AttractionChanged()
                {
                    AttractionLevelTotal = attractionLevel,
                    AttractionLevelMax = attractionLevelMax,
                });
            }
        }

        private void Awake()
        {
            // Set the current threshold to the first buffer point
            currentThreshold = bufferPoints[0];

            // Set the max to the last buffer point
            attractionLevelMax = bufferPoints[bufferPoints.Length - 1];

            // Set the attraction level to 0
            AttractionLevel = 0f;

            // Allow the players to gain attraction
            canGainAttraction = true;

            // Initialize the Timer
            InitializeTimers();
        }

        private void OnEnable()
        {
            onIncreaseAttraction = new EventBinding<IncreaseAttraction>(RaiseAttractionLevel);
            EventBus<IncreaseAttraction>.Register(onIncreaseAttraction);
        }

        private void OnDisable()
        {
            EventBus<IncreaseAttraction>.Deregister(onIncreaseAttraction);
        }

        private void Start()
        {
            sfxManager = ServiceLocator.ForSceneOf(this).Get<SFXManager>();
        }

        /// <summary>
        /// Initialize the timers
        /// </summary>
        private void InitializeTimers()
        {
            // Initialize the decrease buffer timer
            decreaseBufferTimer = new CountdownTimer(decreaseBufferTime);
            decreaseBufferTimer.OnTimerStop += StartDecreasingAttraction;

            // Initialize the decrease attraction timer
            decreaseAttractionTimer = new FrequencyTimer(decreaseAttractionTime);
            decreaseAttractionTimer.OnTick += DecreaseAttraction;
        }

        /// <summary>
        /// Callback function to raise the attraction level
        /// </summary>
        private void RaiseAttractionLevel(IncreaseAttraction eventData)
        {
            // Exit case - Attraction cannot be gained
            if (!canGainAttraction) return;

            // Raise the attraction level
            AttractionLevel += eventData.GainedAttraction;

            // Check if the decrease buffer timer is running
            if (decreaseBufferTimer.IsRunning)
                // If so, reset it
                decreaseBufferTimer.Reset();
            else
                // If not, start it
                decreaseBufferTimer.Start();

            // Check threshold buffers
            BufferThreshold();

            // Check if the attraction level has exceeded the max level
            if (AttractionLevel >= attractionLevelMax)
            {
                // Prevent the player from gaining any more attraction
                canGainAttraction = false;

                // If so, end the game
                EndGame();
            }
        }

        /// <summary>
        /// End the game
        /// </summary>
        private void EndGame()
        {
            Debug.Log("Game ended");
            StartCoroutine(EndGameWait());
            
        }

        IEnumerator EndGameWait()
        {
            yield return new WaitForSeconds(5.0f);
            SceneManager.LoadScene("GameOver");
        }

        /// <summary>
        /// Check to buffer the threshold
        /// </summary>
        private void BufferThreshold()
        {
            int thresholdIndex = 0;
            float newThreshold = currentThreshold;

            for(int i = 0; i < bufferPoints.Length; i++)
            {
                if (attractionLevel < bufferPoints[i]) continue;

                // Set the threshold index
                thresholdIndex = i;
                
                // Set the current threshold
                newThreshold = bufferPoints[i];
            }

            // Exit case - the threshold hasn't changed
            if (newThreshold == currentThreshold) return;

            // Set the new threshold
            currentThreshold = newThreshold;

            // Play the corresponding SFX
            sfxManager.CreateSound().WithSoundData(thresholdSFX[thresholdIndex - 1]).Play();
        }

        /// <summary>
        /// Start the decrease attraction timer to start decreasing attraction
        /// </summary>
        private void StartDecreasingAttraction() => decreaseAttractionTimer.Start();

        /// <summary>
        /// Decrease the attraction level
        /// </summary>
        private void DecreaseAttraction()
        {
            // Exit case - if the attraction level has reached the current threshold
            if (AttractionLevel == currentThreshold)
            {
                decreaseAttractionTimer.Reset();
                decreaseAttractionTimer.Stop();
                return;
            }

            // Decrease the attraction level by the attraction rate
            AttractionLevel -= decreaseAttractionRate;

            // Check if the attraction level has gone below or equal to the current
            // threshold
            if(AttractionLevel <= currentThreshold)
            {
                // If so, set the attraction level to the current threshold
                AttractionLevel = currentThreshold;

                // Reset and stop the attraction timer
                decreaseAttractionTimer.Reset();
                decreaseAttractionTimer.Stop();
            }
        }
    }
}