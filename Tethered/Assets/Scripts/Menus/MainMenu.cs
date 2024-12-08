using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Tethered.Audio;
using Tethered.Patterns.ServiceLocator;
using DG.Tweening;


namespace Tethered.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image fade;
        [SerializeField] private Button startButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button creditsButton;

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
            Button startBtn = startButton.GetComponent<Button>();
            Button exitBtn = exitButton.GetComponent<Button>();
            Button creditsBtn = creditsButton.GetComponent<Button>();

            // Add EventListeners to the buttons for when they're clicked
            startBtn.onClick.AddListener(OpenGame);
            exitBtn.onClick.AddListener(QuitGame);
            creditsBtn.onClick.AddListener(OpenCredits);

            // Get the SFX Manager
            sfxManager = ServiceLocator.ForSceneOf(this).Get<SFXManager>();

            // Fade from black to color
            Fade(1f, 0f, () => Fade(0f, fadeDuration));
        }

        // Opens the game scene
        void OpenGame()
        {
            PlaySound();

            Fade(1f, fadeDuration, () => SceneManager.LoadScene(sceneName: "IntroCutscene"));
        }

        // Closes the game
        void QuitGame()
        {
            PlaySound();

            Application.Quit();
        }

        // Opens credits
        void OpenCredits()
        {
            PlaySound();

            Fade(1f, fadeDuration, () => SceneManager.LoadScene(sceneName: "CreditsMenu"));
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

