using DG.Tweening;
using Tethered.Audio;
using Tethered.Patterns.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private float appearSpeed;
    [SerializeField] private float scrollSpeed;

    [SerializeField] private TMP_Text[] texts;
    [SerializeField] private Image bg;
    [SerializeField] private SpriteRenderer fade;

    [SerializeField] private Vector3 finalPos;

    [Header("SFX")]
    [SerializeField] private SoundData necklaceSnap;
    private SFXManager sfxManager;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get services
        sfxManager = ServiceLocator.ForSceneOf(this).Get<SFXManager>();

        foreach (var text in texts)
        {
            text.alpha = 0;
        }

        DOTween.Sequence()
            // Part 1
            .AppendInterval(scrollSpeed)
            .Append(
                DOTween.To(() => texts[0].alpha, x => texts[0].alpha = x, 1, appearSpeed)
            )
            .AppendInterval(scrollSpeed)
            .Append(
                DOTween.To(() => texts[1].alpha, x => texts[1].alpha = x, 1, appearSpeed)
            )
            .AppendInterval(scrollSpeed + 0.4f)
            .AppendCallback(() =>
            {
                DOTween.To(() => texts[0].alpha, x => texts[0].alpha = x, 0, appearSpeed);
                DOTween.To(() => texts[1].alpha, x => texts[1].alpha = x, 0, appearSpeed);
            })
            // Part 2
            .AppendInterval(scrollSpeed)
            .Append(
                DOTween.To(() => texts[2].alpha, x => texts[2].alpha = x, 1, appearSpeed)
            )
            .AppendInterval(scrollSpeed)
            .Append(
                DOTween.To(() => texts[3].alpha, x => texts[3].alpha = x, 1, appearSpeed)
            )
            .AppendInterval(scrollSpeed + 0.4f)
            .AppendCallback(() =>
            {
                DOTween.To(() => texts[2].alpha, x => texts[2].alpha = x, 0, appearSpeed);
                DOTween.To(() => texts[3].alpha, x => texts[3].alpha = x, 0, appearSpeed);
            })
            .Append(
                DOTween.To(() => texts[4].alpha, x => texts[4].alpha = x, 1, 0.01f)
            )
            .Append(
                (texts[4].rectTransform.DOShakeAnchorPos(1f, 40, 32, 20,
                    false, true, ShakeRandomnessMode.Harmonic)).OnPlay(() => PlaySound())
            )
            .Append(
                DOTween.To(() => texts[4].alpha, x => texts[4].alpha = x, 0, 0.1f)
            )
            .Append(
                DOTween.To(() => bg.color, x => bg.color = x, Color.clear, 0.8f)
            )
            .AppendInterval(1f)
            .Append(
                DOTween.To(() => Camera.main.transform.position, p => Camera.main.transform.position = p, finalPos, 10f)
            )
            .Append(
                DOTween.To(() => fade.color, x => fade.color = x, Color.black, 1.5f)
            )
            .Append(
                DOTween.To(() => bg.color, x => bg.color = x, Color.black, 0.2f)
            )
            .AppendCallback(() =>
                {
                    SceneManager.LoadScene("Scenes/MockLevel1");
                }
            );
    }

    /// <summary>
    /// Play the Button Sound Effect
    /// </summary>
    private void PlaySound() => sfxManager.CreateSound().WithSoundData(necklaceSnap).Play();
}
