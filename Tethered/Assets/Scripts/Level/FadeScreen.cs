using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Tethered.Interactables.Events;
using Tethered.Patterns.EventBus;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tethered.Level
{
    public class FadeScreen : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image fadeImage;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        private EventBinding<EndLevel> onEndLevel;

        private void Awake()
        {
            // Get components
            fadeImage = GetComponent<Image>();

            
        }

        private void OnEnable()
        {
            onEndLevel = new EventBinding<EndLevel>(FadeOut);
            EventBus<EndLevel>.Register(onEndLevel);
        }

        private void OnDisable()
        {
            EventBus<EndLevel>.Deregister(onEndLevel);
        }

        private void Start()
        {
            // Fade from black to color
            Fade(1f, 0f, () => Fade(0f, fadeDuration));
        }

        /// <summary>
        /// Handle fading out on Level End
        /// </summary>
        private void FadeOut(EndLevel eventData)
        {
            Fade(1f, fadeDuration, () =>
            {
                // Kill all tweens
                DOTween.KillAll();

                // Load the new scene
                SceneManager.LoadScene(eventData.LevelIndex);
            });
        }

        /// <summary>
        /// Handle Fade-Tweening for the Screen
        /// </summary>
        private void Fade(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = fadeImage.DOFade(endValue, duration);

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            fadeTween.onComplete = onComplete;
        }
    }
}