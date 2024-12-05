using UnityEngine;
using UnityEngine.UI;
using Tethered.Timers;
using Tethered.Input;
using DG.Tweening;

namespace Tethered.Menus
{
    public class CutsceneSkipper : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private PlayerOneInputReader playerOneInputReader;
        [SerializeField] private PlayerTwoInputReader playerTwoInputReader;

        [Header("References")]
        [SerializeField] private Image radialImage;
        [SerializeField] private Image wKey;
        [SerializeField] private Image upKey;
        private float startingOpacity;

        [Header("Skipping Variables")]
        [SerializeField] private bool playerOneHolding;
        [SerializeField] private bool playerTwoHolding;
        [SerializeField] private bool tryingToSkip;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeRadialDuration;
        [SerializeField] private float fadeKeyDuration;
        private Tween fadeRadialTween;

        private Tween fadeWKeyTween;
        private Tween fadeUpKeyTween;

        private RegenerativeTimer countdownTimer;

        private void Awake()
        {
            // Set a Countdown Timer for two seconds
            countdownTimer = new RegenerativeTimer(3f);

            // Hook up timer events
            countdownTimer.OnTimerTick += UpdateRadialProgressBar;
            countdownTimer.Regenerated += HideRadialProgressBar;
            countdownTimer.OnTimerStop += SkipCutscene;
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
            playerTwoInputReader.Interact += CheckSkipPlayerTwo;
        }

        private void OnDisable()
        {
            // Unsubscribe to input events
            playerOneInputReader.Interact -= CheckSkipPlayerOne;
            playerTwoInputReader.Interact -= CheckSkipPlayerTwo;
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

        /// <summary>
        /// Skip the cutscene
        /// </summary>
        private void SkipCutscene()
        {
            Debug.Log("Scene skipped");
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
            // Check if either of the players are not holding the skip button
            if(!playerOneHolding || !playerTwoHolding)
            {
                // Set not trying to skip
                tryingToSkip = false;

                // Soft stop and reset the Countdown Timer
                countdownTimer.Regenerate();
            }
            // Otherwise, check if not already trying to skip
            else if(!tryingToSkip)
            {
                // Set trying to skip
                tryingToSkip = true;

                // Fade in the Radial Image
                Fade(1f, fadeRadialDuration);

                // Start the CountdownTimer
                countdownTimer.Start();
            }
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