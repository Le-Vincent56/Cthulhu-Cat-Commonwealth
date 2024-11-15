using System.Collections;
using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;
using Tethered.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tethered.Monster
{
    public class AttractionSystem : MonoBehaviour
    {
        [Header("Thresholds")]
        [SerializeField] private float currentThreshold;
        [SerializeField] private float thresholdBuffer;

        [Header("Attraction Level")]
        [SerializeField] private float attractionLevel; //don't reference directly use property
        [SerializeField] private float attractionLevelMax;
        [SerializeField] private float decreaseAttractionRate;

        [Header("Timers")]
        [SerializeField] private float decreaseBufferTime;
        [SerializeField] private float decreaseAttractionTime;
        private CountdownTimer decreaseBufferTimer;
        private FrequencyTimer decreaseAttractionTimer;

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
            // Set the current threshold
            currentThreshold = thresholdBuffer;

            // Set the attraction level to 0
            AttractionLevel = 0f;

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
            // Raise the attraction level
            AttractionLevel += eventData.GainedAttraction;

            // Check if the attraction level has exceeded the max level
            if (AttractionLevel >= attractionLevelMax)
            {
                // If so, end the game
                EndGame();

                return;
            }

            // Check if the decrease buffer timer is running
            if (decreaseBufferTimer.IsRunning)
                // If so, reset it
                decreaseBufferTimer.Reset();
            else
                // If not, start it
                decreaseBufferTimer.Start();

            // Check threshold buffers
            BufferThreshold();
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
            yield return new WaitForSeconds(2.0f);
            SceneManager.LoadScene("GameOver");
        }

        /// <summary>
        /// Check to buffer the threshold
        /// </summary>
        private void BufferThreshold()
        {
            // Exit case - if the attractionl level is below the current threshold
            if (AttractionLevel < currentThreshold) return;

            // Add the buffer to the current threshold
            currentThreshold += thresholdBuffer;
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
            if (AttractionLevel == currentThreshold) return;

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