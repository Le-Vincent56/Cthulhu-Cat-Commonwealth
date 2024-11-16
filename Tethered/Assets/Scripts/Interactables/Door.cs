using DG.Tweening;
using Tethered.Interactables.Events;
using Tethered.Patterns.EventBus;
using UnityEngine;

namespace Tethered.Interactables
{
    public class Door : MonoBehaviour
    {

        [Header("Identifier")]
        [SerializeField] private int hash;

        private SpriteRenderer doorSprite;

        private float doorFadeDuration;
        private Tween fadeTween;

        private EventBinding<HandleDoor> onOpenDoor;

        public int Hash { get => hash; }

        private void Awake()
        {
            // Get components
            doorSprite = GetComponent<SpriteRenderer>();

            // Set the door fade duration
            doorFadeDuration = 0.5f;
        }

        private void OnEnable()
        {
            onOpenDoor = new EventBinding<HandleDoor>(HandleDoor);
            EventBus<HandleDoor>.Register(onOpenDoor);
        }

        private void OnDisable()
        {
            EventBus<HandleDoor>.Deregister(onOpenDoor);
        }

        /// <summary>
        /// Unlock the Door
        /// </summary>
        public void Unlock(bool deactivate = true)
        {
            // Fade out the door and set inactive
            FadeDoor(0f, doorFadeDuration, () =>
            {
                if (deactivate)
                    gameObject.SetActive(false);
            });

            // Disable linked Interactables
            EventBus<DisableInteractables>.Raise(new DisableInteractables() { Hash = hash });
        }

        /// <summary>
        /// Lock the door
        /// </summary>
        public void Lock()
        {
            // Fade in the door 
            FadeDoor(1f, doorFadeDuration);
        }

        /// <summary>
        /// Callback function to handle opening or closing a door
        /// </summary>
        public void HandleDoor(HandleDoor eventData)
        {
            // Exit case - if the Hashes do not match
            if (eventData.Hash != hash) return;

            // Check if opening the door
            if(eventData.Open)
            {
                // If so, unlock it
                Unlock(eventData.Deactivate);
            }
            else
            {
                // Otherwise, lock it
                Lock();
            }
        }

        /// <summary>
        /// Fade the Door using tweening
        /// </summary>
        private void FadeDoor(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the fade tween if it exists
            fadeTween?.Kill();

            // Fade the interact symbol
            fadeTween = doorSprite.DOFade(endValue, duration);

            // Exit case - no callback was given
            if (onComplete == null) return;

            // Add completion listeners
            fadeTween.onComplete += onComplete;
        }
    }
}