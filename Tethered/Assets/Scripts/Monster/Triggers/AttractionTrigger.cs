using System;
using System.Collections;
using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Monster.Triggers
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class AttractionTrigger : MonoBehaviour
    {
        [Header("Trigger Details")]
        [SerializeField] private int hash;
        [SerializeField] private bool triggerEnabled;
        [SerializeField] private bool hasCallback;

        [Header("Attraction Fields")]
        [SerializeField] private PlayerWeight triggerWeight;
        [SerializeField] private float attractionAmount;
        [SerializeField] private bool isOneTime = false;
        [SerializeField] private bool isOverTime = false; // turns attraction amount to rate per second

        private EventBinding<ToggleTrigger> onToggleTrigger;
        private IEnumerator coroutine;
        private int overTimeTick;

        private void Awake()
        {
            triggerEnabled = true;
            overTimeTick = 0;
            coroutine = RaiseAttractionOverTime();
        }

        private void OnEnable()
        {
            onToggleTrigger = new EventBinding<ToggleTrigger>(ToggleTrigger);
            EventBus<ToggleTrigger>.Register(onToggleTrigger);
        }

        private void OnDisable()
        {
            EventBus<ToggleTrigger>.Deregister(onToggleTrigger);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - the trigger is disabled
            if (!triggerEnabled) return;

            // Exit case - if the entity is not a Player Controller
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Check if the player's weight is as heavy or heavier than the trigger's
            // weight
            if (controller.Weight >= triggerWeight)
            {
                // Disable the Attraction trigger it is a one time trigger.
                if (isOneTime)
                {
                    EventBus<ToggleTrigger>.Raise(new ToggleTrigger() { Hash = hash, Enabled = false });
                }

                // Increase Attraction
                EventBus<IncreaseAttraction>.Raise(new IncreaseAttraction()
                {
                    GainedAttraction = attractionAmount
                });

                // Start over time coroutine if it is an over time trigger.
                if (isOverTime)
                {
                    if(overTimeTick <= 0) StartCoroutine(coroutine);
                    overTimeTick++;
                    
                }
            }
        }

        protected void OnTriggerExit2D (Collider2D collision)
        {
            // Logic for removing players from the overtime trigger.
            if (isOverTime)
            {
                overTimeTick--;
                
                // If no players remain inside the trigger, stop the attraction increase over time.
                if (overTimeTick <= 0)
                {
                    overTimeTick = 0;
                    StopCoroutine(coroutine);
                }
            }
        }

        /// <summary>
        /// Coroutine that triggers if attraction should increase over time.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RaiseAttractionOverTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                EventBus<IncreaseAttraction>.Raise(new IncreaseAttraction()
                {
                    GainedAttraction = attractionAmount
                });
            }
        }

        /// <summary>
        /// Handle trigger toggling
        /// </summary>
        private void ToggleTrigger(ToggleTrigger eventData)
        {
            // Exit case - Hash mismatch
            if (eventData.Hash != hash) return;

            // Set whether or not the trigger is enabled
            triggerEnabled = eventData.Enabled;
            
            if (isOverTime && !eventData.Enabled)
            {
                overTimeTick = 0;
                StopCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Set the Attraction Trigger's hash
        /// </summary>
        public void SetHash(int hash) => this.hash = hash;
    }
}