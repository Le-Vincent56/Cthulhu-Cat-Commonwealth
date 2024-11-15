using System.Collections.Generic;
using Tethered.Patterns.ServiceLocator;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

namespace Tethered.Audio
{
    public class SFXManager : MonoBehaviour
    {
        private IObjectPool<SoundEmitter> soundEmitterPool;
        private readonly List<SoundEmitter> activeSoundEmitters = new();
        public readonly Queue<SoundEmitter> FrequentSoundEmitters = new();

        [SerializeField] private SoundEmitter soundEmitterPrefab;
        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        [SerializeField] private int maxSoundInstances = 30;

        private void Awake()
        {
            // Register the SoundManager as a service
            ServiceLocator.ForSceneOf(this).Register(this);
        }

        private void Start()
        {
            // Initialize the Sound Emitter pool
            InitializePool();
        }

        /// <summary>
        /// Create a SoundBuilder to build sounds
        /// </summary>
        public SoundBuilder CreateSound() => new SoundBuilder(this);

        /// <summary>
        /// Create a de-activated Sound Emitter
        /// </summary>
        private SoundEmitter CreateSoundEmitter()
        {
            // Instantiate the Sound Emitter
            SoundEmitter soundEmitter = Instantiate(soundEmitterPrefab);

            // De-activate the Sound Emitter
            soundEmitter.gameObject.SetActive(false);

            return soundEmitter;
        }

        /// <summary>
        /// Take a Sound Emitter from the pool
        /// </summary>
        private void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            // Set the Sound Emitter as active
            soundEmitter.gameObject.SetActive(true);

            // Add it to the active list
            activeSoundEmitters.Add(soundEmitter);
        }

        /// <summary>
        /// Return a Sound Emitter to the pool
        /// </summary>
        private void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            // Set the Sound Emitter as in-active
            soundEmitter.gameObject.SetActive(false);

            // Remove it from the active list
            activeSoundEmitters.Remove(soundEmitter);
        }

        /// <summary>
        /// Destroy a Sound Emitter
        /// </summary>
        private void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            // Destroy the Sound Emitter object
            Destroy(soundEmitter.gameObject);
        }

        /// <summary>
        /// Initialize the Sound Emitter pool
        /// </summary>
        private void InitializePool()
        {
            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
        }

        /// <summary>
        /// Get a Sound Emitter from the pool
        /// </summary>
        public SoundEmitter Get() => soundEmitterPool.Get();

        /// <summary>
        /// Return a Sound Emitter to the pool
        /// </summary>
        public void ReturnToPool(SoundEmitter soundEmitter) => soundEmitterPool.Release(soundEmitter);

        /// <summary>
        /// Check if a Sound can be played
        /// </summary>
        public bool CanPlaySound(SoundData data)
        {
            // Exit case - if the sound is not a frequent sound; assume that it can always be played
            if (!data.frequentSound) return true;

            // Check if the Frequent Sound Emitters queue exceeds the max amount of sound instances and 
            // if a SoundEmitter can be dequeued from the queue
            if (FrequentSoundEmitters.Count >= maxSoundInstances && FrequentSoundEmitters.TryDequeue(out SoundEmitter soundEmitter))
            {
                // Try/catch
                try
                {
                    // Stop the Sound Emitter
                    soundEmitter.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("SoundEmitter is already released");
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Stop an active sound from playing
        /// </summary>
        public void StopSound(SoundData data)
        {
            // Iterate through each active Sound Emitter
            foreach(SoundEmitter emitter in activeSoundEmitters)
            {
                // Skip if the data does not match
                if (emitter.Data != data) continue;

                // Stop the Sound Emitter
                emitter.Stop();
            }
        }
    }
}