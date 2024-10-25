using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    public class LadderDoor : Interactable
    {
        [SerializeField] private SpriteRenderer extendableLadder;
        [SerializeField] private float targetHeight;
        [SerializeField] private float animationDuration;
        [SerializeField] private bool extended;

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

        public override void Interact(InteractController controller)
        {
            // Exit case - if extended
            if (extended)
            {
                // Climb here?
                return;
            }

            // TODO: Get the direction (for when setting Reach state)
            int direction = (int)Mathf.Sign(controller.transform.position.y - transform.position.y);

            // Exit case - the Sprite Renderer is not in the correct Draw Mode
            if (extendableLadder.drawMode != SpriteDrawMode.Tiled) return;

            // Store the initial height
            float initialWidth = extendableLadder.size.x;

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
            extended = true;
        }
    }
}