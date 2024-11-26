using Tethered.Audio;
using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;
using Tethered.Patterns.ServiceLocator;
using Tethered.Player;
using UnityEngine;

public class WeightFloor : MonoBehaviour
{
    [SerializeField] private float attractionAmount;
    [SerializeField] private float distanceToBreak;

    private SFXManager sfxManager;
    private Vector3 centerPoint;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private SoundData soundData;

    private void Awake()
    {
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the center point
        centerPoint = transform.position + new Vector3(spriteRenderer.size.x / 2f, 0f, 0f);
    }

    private void Start()
    {
        sfxManager = ServiceLocator.ForSceneOf(this).Get<SFXManager>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>().Weight == PlayerWeight.Heavy)
        {
            
            // Get the distance to the player
            float distanceToPlayer = (collision.transform.position - centerPoint).magnitude;

            // Check if the player has come within the breaking distance
            if (distanceToPlayer <= distanceToBreak)
            {
                // Gain attraction
                EventBus<IncreaseAttraction>.Raise(new IncreaseAttraction()
                {
                    GainedAttraction = attractionAmount
                });

                // Play the SFX
                sfxManager.CreateSound().WithSoundData(soundData).Play();

                // Destroy the game object
                Destroy(gameObject);
            }
        }
    }
}
