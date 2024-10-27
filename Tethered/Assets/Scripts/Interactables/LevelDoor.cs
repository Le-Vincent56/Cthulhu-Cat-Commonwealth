using DG.Tweening;
using Tethered.Interactables;
using Tethered.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDoor : Interactable
{
    [SerializeField] private int levelIndex;

    public override void Interact(InteractController controller)
    {
        DOTween.KillAll();

        // Load the new scene
        SceneManager.LoadScene(levelIndex);
    }
}
