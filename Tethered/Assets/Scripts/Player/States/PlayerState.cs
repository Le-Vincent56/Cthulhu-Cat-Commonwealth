using Tethered.Patterns.StateMachine;
using UnityEngine;

namespace Tethered.Player.States
{
    public abstract class PlayerState : IState
    {
        protected readonly PlayerController controller;
        protected readonly Animator animator;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int CrawlHash = Animator.StringToHash("Crawl");

        protected const float crossFadeDuration = 0.1f;

        public PlayerState(PlayerController controller, Animator animator)
        {
            this.controller = controller;
            this.animator = animator;
        }

        public virtual void OnEnter()
        {
            // Noop
        }

        public virtual void Update()
        {
            // Noop
        }

        public virtual void FixedUpdate()
        {
            // Noop
        }

        public virtual void OnExit()
        {
            // Noop
        }
    }
}