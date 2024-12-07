using Tethered.Player;
using Tethered.Player.States;
using UnityEngine;

public class MovingObjectPrepState : PlayerState
{
    private readonly MoveableController moveableController;

    public MovingObjectPrepState(
        PlayerController controller,
        Animator animator,
        MoveableController moveableController
    ) : base(controller, animator)
    {
        this.moveableController = moveableController;
    }

    public override void OnEnter()
    {
        // Disable input to prevent movement
        controller.DisableInput();

        // Cross fade into the animation
        animator.CrossFade(MoveObjectPrepHash, crossFadeDuration);

        // Set the position of the Player
        moveableController.PositionWithMoveable(() =>
        {
            // Allow the Player to move objects
            moveableController.CanMoveObject = true;

            // Enable input
            controller.EnableInput();
        });
    }
}
