using UnityEngine;

namespace Tethered.Player.States
{
    public class MovingObjectIdleState : PlayerState
    {
        private readonly MoveableController moveableController;

        public MovingObjectIdleState(
            PlayerController controller, 
            Animator animator, 
            MoveableController moveableController
        ) : base(controller, animator)
        {
            this.moveableController = moveableController;
        }

        public override void OnEnter()
        {

        }
    }
}
