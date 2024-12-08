using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Tethered.Audio;
using Tethered.Patterns.ServiceLocator;


namespace Tethered.Menus
{
    public class CreditsMenu : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image fade;
        [SerializeField] private Button backButton;

        [Header("Audio")]
        [SerializeField] private SoundData uiButtonSound;
        private SFXManager sfxManager;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;


        // Start is called before the first frame update
        void Start()
        {
            // Initialize the buttons in the menu
            Button backBtn = backButton.GetComponent<Button>();

            // Add EventListeners to the buttons for when they're clicked
            backBtn.onClick.AddListener(GoToMain);

            // Get the SFX Manager
            sfxManager = ServiceLocator.ForSceneOf(this).Get<SFXManager>();

            // Fade from black to color
            Fade(1f, 0f, () => Fade(0f, fadeDuration));
        }

        // Opens the game scene
        void GoToMain()
        {
            // Play the sound
            PlaySound();

            // Fade to Main Menu
            Fade(1f, fadeDuration, () => SceneManager.LoadScene(sceneName: "MainMenu"));
        }

        /// <summary>
        /// Play the Button Sound Effect
        /// </summary>
        private void PlaySound() => sfxManager.CreateSound().WithSoundData(uiButtonSound).WithRandomPitch().Play();

        /// <summary>
        /// Handle Fade-Tweening for the Screen
        /// </summary>
        private void Fade(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = fade.DOFade(endValue, duration);

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            fadeTween.onComplete = onComplete;
        }
    }
}

