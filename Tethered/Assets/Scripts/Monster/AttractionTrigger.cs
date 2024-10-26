using Tethered.Interactables.Events;
using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Monster
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class AttractionTrigger : MonoBehaviour
    {
        [Header("Trigger Details")]
        [SerializeField] private int hash;
        [SerializeField] private bool triggerEnabled;

        [Header("Attraction Fields")]
        [SerializeField] private PlayerWeight triggerWeight;
        [SerializeField] private float attractionAmount;
        [SerializeField] private bool isOneTime = false;

        private EventBinding<ToggleTrigger> onToggleTrigger;

        private void Awake()
        {
            triggerEnabled = true;
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - the trigger is disabled
            if (!triggerEnabled) return;

            // Exit case - if the entity is not a Player Controller
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Check if the player's weight is as heavy or heavier than the trigger's
            // weight
            if (controller.Weight >= triggerWeight)
            {
                if (isOneTime)
                {
                    EventBus<ToggleTrigger>.Raise(new ToggleTrigger() { Hash = hash, Enabled = false });
                }

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
            // Exit case - 
            if (eventData.Hash != hash) return;

            // Set whether or not the trigger is enabled
            triggerEnabled = eventData.Enabled;

        }

        /// <summary>
        /// Set the Attraction Trigger's hash
        /// </summary>
        public void SetHash(int hash) => this.hash = hash;
    }
}