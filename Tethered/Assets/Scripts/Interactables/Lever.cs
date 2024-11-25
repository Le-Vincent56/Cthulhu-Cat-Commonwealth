using Tethered.Interactables.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using Tethered.Timers;
using UnityEngine;

namespace Tethered.Interactables
{
    public class Lever : Interactable
    {
        [Header("Identifier")]
        [SerializeField] private int hash;

        [Header("Level Details")]
        [SerializeField] private bool openDoor;
        [SerializeField] private bool onCooldown;

        private CountdownTimer cooldownTimer;

        protected override void Awake()
        {
            // Call the parent Awake
            base.Awake();

            // Set on cool down to false
            onCooldown = false;

            // Set open door to true
            openDoor = true;

            // Instantiate the countdown timer
            cooldownTimer = new CountdownTimer(0.5f);

            // Set timer events
            cooldownTimer.OnTimerStart += () => onCooldown = true;
            cooldownTimer.OnTimerStop += () => onCooldown = false;
        }

        /// <summary>
        /// Pull the lever
        /// </summary>
        public override void Interact(InteractController controller)
        {
            // Exit case - on cooldown
            if (onCooldown) return;

            // Raise the HandleDoor event
            EventBus<HandleDoor>.Raise(new HandleDoor()
            {
                Hash = hash,
                Open = openDoor,
                Deactivate = false
            });

            // Toggle the lever
            openDoor = !openDoor;

            // Start the countdown timer
            cooldownTimer.Start();

            // Toggle the interact symbol
            HideInteractSymbol(() => ShowInteractSymbol());
        }

        /// <summary>
        /// Set the Lever's hash
        /// </summary>
        public void SetHash(int hash) => this.hash = hash;
    }
}