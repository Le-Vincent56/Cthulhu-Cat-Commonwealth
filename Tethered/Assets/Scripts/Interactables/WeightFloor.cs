using System.Collections;
using System.Collections.Generic;
using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using UnityEngine;

public class WeightFloor : MonoBehaviour
{
    [SerializeField] private float attractionAmount;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>().Weight == PlayerWeight.Heavy)
        {
            EventBus<IncreaseAttraction>.Raise(new IncreaseAttraction()
            {
                GainedAttraction = attractionAmount
            });
            Destroy(gameObject);
        }

    }
}
