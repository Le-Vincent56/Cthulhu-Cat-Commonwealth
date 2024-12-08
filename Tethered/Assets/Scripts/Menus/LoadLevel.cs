using DG.Tweening;
using Tethered.Audio;
using Tethered.Patterns.ServiceLocator;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fadeImage;

    [SerializeField] private SoundData uiButtonSound;
    private SFXManager sfxManager;

    [Header("Tweening Variables")]
    [SerializeField] private float fadeDuration;
    private Tween fadeTween;

    private void Start()
    {
        // Get the SFX Manager
        sfxManager = ServiceLocator.ForSceneOf(this).Get<SFXManager>();

        // Fade from black to color
        Fade(1f, 0f, () => Fade(0f, fadeDuration));
    }

    /// <summary>
    /// Load a level from its scene index
    /// </summary>
    public void LoadLevelFromIndex(int index)
    {
        PlaySound();

        Fade(1f, fadeDuration, () => SceneManager.LoadScene(index));
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
        fadeTween = fadeImage.DOFade(endValue, duration);

        // Exit case - there's no completion action
        if (onComplete == null) return;

        // Hook up completion actions
        fadeTween.onComplete = onComplete;
    }
}
