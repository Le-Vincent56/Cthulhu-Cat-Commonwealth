using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Monster
{
    public class AttractionTrigger : MonoBehaviour
    {
        [Header("Attraction Fields")]
        [SerializeField] private PlayerWeight triggerWeight;
        [SerializeField] private float attractionAmount;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - if the entity is not a Player Controller
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Check if the player's weight is as heavy or heavier than the trigger's
            // weight
            if (controller.Weight >= triggerWeight)
                EventBus<IncreaseAttraction>.Raise(new IncreaseAttraction()
                {
                    GainedAttraction = attractionAmount
                });
        }
    }
}