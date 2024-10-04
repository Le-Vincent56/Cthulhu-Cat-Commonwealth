using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;


namespace Tethered.Monster.Trigger
{
    public class AttractionTrigger : MonoBehaviour
    {
        private EventBinding<TriggerAttraction> onTriggerAttraction;

        private void OnEnable()
        {
            // Initialize the EventBinding
            //onTriggerAttraction = new EventBinding<TriggerAttraction>(OnCollisionEnter); --WILL NOT WORK!! (method takes in a parametet

            // Register to the EventBus
            //EventBus<IncreaseAttraction>.Register(onTriggerAttraction);
        }

        private void OnDisable()
        {
            // Deregister to the EventBus
            //EventBus<IncreaseAttraction>.Deregister(onTriggerAttraction);
        }


        void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.red);
            }
            //if (collision.relativeVelocity.magnitude > 2)
               // onIncreaseInteraction;
        }
    }
}

