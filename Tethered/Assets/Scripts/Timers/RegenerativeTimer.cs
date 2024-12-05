using UnityEngine;
using UnityEngine.Events;

namespace Tethered.Timers
{
    public class RegenerativeTimer : Timer
    {
        public override bool IsFinished => CurrentTime <= 0;

        public UnityAction Regenerated = delegate { };

        public RegenerativeTimer(float value) : base(value) { }

        public override void Tick()
        {
            // If currently running and the time is greater than 0, reduce the time
            if (IsRunning && CurrentTime > 0)
            {
                // Subtract the time
                CurrentTime -= Time.deltaTime;

                // Send the tick event
                OnTimerTick.Invoke();
            } 
            // If not running and the time is greater than 0, but less than the initial time
            else if(!IsRunning && CurrentTime > 0 && CurrentTime < initialTime)
            {
                // Add the delta time
                CurrentTime += Time.deltaTime;

                // Clamp to between 0 and the initial time
                CurrentTime = Mathf.Clamp(CurrentTime, 0, initialTime);

                // Send the tick event
                OnTimerTick.Invoke();
            }

            // If currently running and the time is equal to or below 0 or equal to or above the initial time,
            // stop the Timer
            if (IsRunning && CurrentTime <= 0) Stop();
            if (!IsRunning && CurrentTime >= initialTime) Regenerated.Invoke();
        }

        public void Regenerate()
        {
            // Exit case - the RegenerativeTimer is not running and therefore does not need to regenerate
            if (!IsRunning) return;

            // Set IsRunning to false to allow regenerating
            IsRunning = false;
        }
    }
}
