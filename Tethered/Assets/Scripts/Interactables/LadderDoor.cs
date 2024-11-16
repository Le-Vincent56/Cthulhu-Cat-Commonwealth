using DG.Tweening;
using Tethered.Interactables.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    public class LadderDoor : Interactable
    {
        [SerializeField] private int hash;
        [SerializeField] private SpriteRenderer extendableLadder;
        [SerializeField] private float targetHeight;
        [SerializeField] private float animationDuration;
        [SerializeField] private bool extended;

        [SerializeField] private int verticalEnterDirection;
        [SerializeField] private float symbolHeightTop;
        [SerializeField] private float symbolHeightBottom;

        [SerializeField] private GameObject floor;

        protected override void Awake()
        {
            base.Awake();

            // Check if the ladder is not extended
            if(!extended)
            {
                // Set the target height then set the height to 0
                targetHeight = extendableLadder.size.y;
                extendableLadder.size = new Vector2(1f, 0f);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - if already extended
            if (extended) return;

            // Exit case - there is not InteractController
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(this);

            // Add the controller to the hashset
            controllers.Add(controller);

            // Get the entering PlayerController
            PlayerController enteringPlayer = controller.GetComponent<PlayerController>();

            // Decide the sprite based on the entering player
            DecideEnterSprite(enteringPlayer);

            // Exit case - if the symbol is shown
            if (symbolShown) return;

            // Set the vertical enter direction
            verticalEnterDirection = (int)Mathf.Sign(controller.transform.position.y - transform.position.y);

            // Show the interact symbol
            ShowInteractSymbol();
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - if already extended
            if (extended) return;

            base.OnTriggerExit2D(collision);
        }

        protected override void ShowInteractSymbol(TweenCallback onComplete = null)
        {
            // Check the enter direction
            if(verticalEnterDirection == 1)
            {
                interactSymbol.transform.localPosition = new Vector3(
                    interactSymbol.transform.localPosition.x,
                    symbolHeightTop, 
                    interactSymbol.transform.localPosition.z
                );
            } else if(verticalEnterDirection == -1)
            {
                interactSymbol.transform.localPosition = new Vector3(
                    interactSymbol.transform.localPosition.x, 
                    symbolHeightBottom, 
                    interactSymbol.transform.localPosition.z
                );
            }

            // Fade in and notify that the symbol is shown
            Fade(1f, symbolFadeDuration, () => symbolShown = true);

            // Scale to target
            Scale(symbolTargetScale, scaleDuration, onComplete);
        }

        public override void Interact(InteractController controller)
        {
            // Exit case - if already extended
            if (extended) return;

            // Get the direction (for when setting Reach state)
            int direction = (int)Mathf.Sign(controller.transform.position.y - transform.position.y);

            // Check if the direction is from downward and if it is Player One
            if(direction == -1 && controller.TryGetComponent(out PlayerOneController playerController))
            {
                // Set reaching
                playerController.SetReaching(true);
            }

            // Exit case - the Sprite Renderer is not in the correct Draw Mode
            if (extendableLadder.drawMode != SpriteDrawMode.Tiled) return;

            // Store the initial height
            float initialWidth = extendableLadder.size.x;
            
            floor.SetActive(false);

            // Tween the height
            DOTween.To(() => extendableLadder.size,
                x => extendableLadder.size = x,
                new Vector2(initialWidth, targetHeight),
                animationDuration)
            .SetEase(Ease.InExpo)
            .OnComplete(OnFullExtend);
        }

        /// <summary>
        /// Action for when the ladder is fully extended
        /// </summary>
        private void OnFullExtend()
        {
            // Set values for when the ladder is fully extended
            extended = true;

            EventBus<EnableLadder>.Raise(new EnableLadder()
            {
                Hash = hash
            });

            // Hide the interact symbol
            HideInteractSymbol();
        }
    }
}