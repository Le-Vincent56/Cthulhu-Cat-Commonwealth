using UnityEngine;
using UnityEngine.Events;

namespace Tethered.Timers
{
    public class RegenerativeTimer : Timer
    {
        private bool regenerating;

        public override bool IsFinished => CurrentTime <= 0;

        public UnityAction Regenerated = delegate { };

        public RegenerativeTimer(float value) : base(value)
        {
            regenerating = false;
        }

        public override void Tick()
        {
            // Check if the Timer is running
            if (IsRunning)
            {
                // Check if regenerating and if the Current Time is still within the range of [0, initialTime]
                if(regenerating && CurrentTime < initialTime)
                {
                    // Add the delta time
                    CurrentTime += Time.deltaTime;

                    // Clamp to between 0 and the initial time
                    CurrentTime = Mathf.Clamp(CurrentTime, 0, initialTime);

                    // Send the tick event
                    OnTimerTick.Invoke();
                }
                // Otherwise, check if not regenerating and if the Current Time is above 0
                else if(!regenerating && CurrentTime > 0)
                {
                    // Subtract the time
                    CurrentTime -= Time.deltaTime;

                    // Clamp to between 0 and the initial time
                    CurrentTime = Mathf.Clamp(CurrentTime, 0, initialTime);

                    // Send the tick event
                    OnTimerTick.Invoke();
                }
            }

            // Check if the Timer is Running and the Current Time is less than or equal to 0
            if (IsRunning && !regenerating && CurrentTime <= 0)
                // Stop the Timer
                Stop();

            // Ceheck if the Timer is running and regenerating and if the Current Time is greater than the initial time
            if (IsRunning && regenerating && CurrentTime >= initialTime)
            {
                Debug.Log($"Fully Regenerated: " +
                    $"\nCurrent Time: {CurrentTime}" +
                    $"\nInitial Time: {initialTime}");

                // Invoke the Regenerated event
                Regenerated.Invoke();

                // Pause the timer
                Pause();
            }
        }

        /// <summary>
        /// Start regenerating
        /// </summary>
        public void StartRegenerating()
        {
            regenerating = true;
            
            // Exit case - the Regenerative Timer is running
            if (IsRunning) return;

            // Resume if paused
            Resume();
        }

        /// <summary>
        /// Stop regenerating
        /// </summary>
        public void StopRegenerating()
        {
            regenerating = false;

            // Exit case - the Regenerative Timer is running
            if (IsRunning) return;

            // Resume if paused
            Resume();
        }
    }
}
