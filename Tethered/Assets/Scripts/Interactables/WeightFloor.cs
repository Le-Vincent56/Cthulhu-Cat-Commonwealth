using System.Collections;
using System.Collections.Generic;
using Tethered.Player;
using UnityEngine;

public class WeightFloor : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>().Weight == PlayerWeight.Heavy)
        {
            Destroy(gameObject);
        }

    }
}
