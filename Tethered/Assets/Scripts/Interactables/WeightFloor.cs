using Tethered.Audio;
using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;
using Tethered.Patterns.ServiceLocator;
using Tethered.Player;
using UnityEngine;

public class WeightFloor : MonoBehaviour
{
    [SerializeField] private float attractionAmount;

    private SFXManager sfxManager;
    [SerializeField] private SoundData soundData;

    private void Start()
    {
        sfxManager = ServiceLocator.ForSceneOf(this).Get<SFXManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>().Weight == PlayerWeight.Heavy)
        {
            EventBus<IncreaseAttraction>.Raise(new IncreaseAttraction()
            {
                GainedAttraction = attractionAmount
            });

            // Play the SFX
            sfxManager.CreateSound().WithSoundData(soundData).Play();

            Destroy(gameObject);
        }

    }
}
