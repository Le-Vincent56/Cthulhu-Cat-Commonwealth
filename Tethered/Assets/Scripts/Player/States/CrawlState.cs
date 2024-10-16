using UnityEngine;

namespace Tethered.Player.States
{
    public class CrawlState : PlayerState
    {
        public CrawlState(PlayerController controller, Animator animator)
            : base(controller, animator)
        {
        }

        public override void OnEnter()
        {
            animator.CrossFade(CrawlHash, crossFadeDuration);
        }

        public override void Update()
        {
            // Exit case - if the PlayerController is not a PlayerTwoController
            if (controller is not PlayerTwoController youngerController) return;

            // Handle crawling
            youngerController.Crawl();
        }
    }
}