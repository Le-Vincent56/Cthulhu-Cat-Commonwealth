using UnityEngine;
using UnityEngine.UI;
using Tethered.Timers;
using Tethered.Input;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Tethered.Menus
{
    public class CutsceneSkipper : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private PlayerOneInputReader playerOneInputReader;
        [SerializeField] private PlayerTwoInputReader playerTwoInputReader;

        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image radialImage;
        [SerializeField] private Image wKey;
        [SerializeField] private Image upKey;
        private bool groupShown;
        private float startingOpacity;

        [Header("Skipping Variables")]
        [SerializeField] private bool playerOneHolding;
        [SerializeField] private bool playerTwoHolding;
        [SerializeField] private bool firstTimeSkip;
        [SerializeField] private bool tryingToSkip;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeCanvasDuration;
        [SerializeField] private float fadeRadialDuration;
        [SerializeField] private float fadeKeyDuration;
        private Tween fadeCanvasTween;
        private Tween fadeRadialTween;
        private Tween fadeWKeyTween;
        private Tween fadeUpKeyTween;

        private RegenerativeTimer countdownTimer;

        private void Awake()
        {
            // Get components
            canvasGroup = GetComponent<CanvasGroup>();

            // Set variables
            firstTimeSkip = true;

            // Set a Countdown Timer for two seconds
            countdownTimer = new RegenerativeTimer(3f);

            // Hook up timer events
            countdownTimer.OnTimerStart += () => firstTimeSkip = false;
            countdownTimer.OnTimerTick += UpdateRadialProgressBar;
            countdownTimer.Regenerated += HideRadialProgressBar;
            countdownTimer.OnTimerFinished += SkipCutscene;
        }

        private void Start()
        {
            // Set the starting opacity
            startingOpacity = wKey.color.a;
        }

        private void OnEnable()
        {
            // Subscribe to input events
            playerOneInputReader.Interact += CheckSkipPlayerOne;
            playerOneInputReader.TryToSkip += ShowSkippingUI;
            playerTwoInputReader.Interact += CheckSkipPlayerTwo;
            playerTwoInputReader.TryToSkip += ShowSkippingUI;
        }

        private void OnDisable()
        {
            // Unsubscribe to input events
            playerOneInputReader.Interact -= CheckSkipPlayerOne;
            playerOneInputReader.TryToSkip -= ShowSkippingUI;
            playerTwoInputReader.Interact -= CheckSkipPlayerTwo;
            playerTwoInputReader.TryToSkip -= ShowSkippingUI;
        }

        private void OnDestroy()
        {
            // Dispose of the Countdown Timer
            countdownTimer.Dispose();
        }

        /// <summary>
        /// Update the Radial Progress Bar
        /// </summary>
        private void UpdateRadialProgressBar()
        {
            // Set the fill amount to the Countdown Timer's progress
            radialImage.fillAmount = countdownTimer.ProgressIncreasing;
        }

        /// <summary>
        /// Hide the Radial Progress Bar
        /// </summary>
        private void HideRadialProgressBar() => Fade(0f, fadeRadialDuration);

        private void ShowSkippingUI()
        {
            // Exit case - the CanvasGroup is already shown
            if (groupShown) return;

            // Set the group as shown
            groupShown = true;

            // Fade in the Canvas Group
            FadeCanvasGroup(1f, fadeCanvasDuration);
        }

        /// <summary>
        /// Skip the cutscene
        /// </summary>
        private void SkipCutscene()
        {
            // Kill all tweens
            DOTween.KillAll();

            // Change the scene
            SceneManager.LoadScene("Scenes/MockLevel1");
        }

        /// <summary>
        /// Check if Player One is holding the skip button
        /// </summary>
        private void CheckSkipPlayerOne(bool startedOrPerformed)
        {
            // Set whether or not Player one is holding the button
            playerOneHolding = startedOrPerformed;

            // Set the target opacity
            float targetOpacity = playerOneHolding ? 1f : startingOpacity;

            // Handle fading the W-Key
            FadeWKey(targetOpacity, fadeKeyDuration);

            // Check whether or not the Players are skipping
            CheckSkipping();
        }

        /// <summary>
        /// Check if Player Two is holding the skip button
        /// </summary>
        private void CheckSkipPlayerTwo(bool startedOrPerformed)
        {
            // Set whether or not Player one is holding the button
            playerTwoHolding = startedOrPerformed;

            float targetOpacity = playerTwoHolding ? 1f : startingOpacity;

            // Handle fading the Up-Key
            FadeUpKey(targetOpacity, fadeKeyDuration);

            // Check whether or not the Players are skipping
            CheckSkipping();
        }

        /// <summary>
        /// Check if the cutscene should start to be skipped
        /// </summary>
        private void CheckSkipping()
        {
            if((playerOneHolding && playerTwoHolding) && !tryingToSkip)
            {
                // Start trying to skip
                tryingToSkip = true;

                // Fade in the Radial Image
                Fade(1f, fadeRadialDuration);

                // Check if it's the first time skipping
                if(firstTimeSkip)
                    // Start the Countdown Timer
                    countdownTimer.Start();
                else
                    // Stop regenerating the timer
                    countdownTimer.StopRegenerating();
            } else if(!(playerOneHolding && playerTwoHolding) && tryingToSkip)
            {
                // Stop trying to skip
                tryingToSkip = false;

                // Regenerate the Countdown Timer
                countdownTimer.StartRegenerating();
            }
        }

        /// <summary>
        /// Handle the Tween-fading for the Canvas Group
        /// </summary>
        private void FadeCanvasGroup(float endValue, float duration)
        {
            // Kill the Fade Canvas Tween if it exists
            fadeCanvasTween?.Kill();

            // Set the Fade Canvas Tween
            fadeCanvasTween = canvasGroup.DOFade(endValue, duration);
        }

        /// <summary>
        /// Handle Tween-fading for the Radial Image
        /// </summary>
        private void Fade(float endValue, float duration)
        {
            // Kill the Fade Radial Tween if it exists
            fadeRadialTween?.Kill();

            // Set the Fade Radial Tween
            fadeRadialTween = radialImage.DOFade(endValue, duration);
        }

        /// <summary>
        /// Handle Tween-fading for the W-Key Image
        /// </summary>
        private void FadeWKey(float endValue, float duration)
        {
            // Kill the Fade Key Tween if it exists
            fadeWKeyTween?.Kill();

            // Set the Fade Key Tween
            fadeWKeyTween = wKey.DOFade(endValue, duration);
        }

        /// <summary>
        /// Handle Tween-fading for the Up-Key Image
        /// </summary>
        private void FadeUpKey(float endValue, float duration)
        {
            // Kill the Fade Key Tween if it exists
            fadeUpKeyTween?.Kill();

            // Set the Fade Key Tween
            fadeUpKeyTween = upKey.DOFade(endValue, duration);
        }
    }
}