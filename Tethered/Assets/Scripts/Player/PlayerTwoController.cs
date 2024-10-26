using UnityEngine;
using Tethered.Input;
using Tethered.Player.States;
using Tethered.Patterns.StateMachine;
using System.Collections.Generic;

namespace Tethered.Player
{
    public class PlayerTwoController : PlayerController
    {
        [Header("Input")]
        [SerializeField] private PlayerTwoInputReader inputReader;

        [Header("Crawling")]
        [SerializeField] private bool crawling;
        [SerializeField] private int currentPathIndex;
        [SerializeField] private List<Vector2> path;
        [SerializeField] private Vector2 crawlOffsetAndSize;
        [SerializeField] private Vector2 normalOffsetAndSize;

        protected override void Awake()
        {
            // Call the base Awake method
            base.Awake();

            // Initialize the path list
            path = new List<Vector2>();

            // Set the initial offset and size
            normalOffsetAndSize = new Vector2(boxCollider.offset.y, boxCollider.size.y);
        }

        protected override void Update()
        {
            // Update movement direction
            moveDirectionX = inputReader.NormMoveX;

            // Update the state machine
            base.Update();
        }

        /// <summary>
        /// Set up the Younger Sibling's individual states
        /// </summary>
        protected override void SetupStates(IdleState idleState, LocomotionState locomotionState, ClimbState climbState)
        {
            // Create states
            CrawlState crawlState = new CrawlState(this, animator);

            // Define state transitions
            stateMachine.At(idleState, crawlState, new FuncPredicate(() => crawling));
            stateMachine.At(locomotionState, crawlState, new FuncPredicate(() => crawling));
            stateMachine.At(climbState, crawlState, new FuncPredicate(() => crawling));

            stateMachine.At(crawlState, idleState, new FuncPredicate(() => !crawling && moveDirectionX == 0));
            stateMachine.At(crawlState, locomotionState, new FuncPredicate(() => !crawling && moveDirectionX != 0));
            stateMachine.At(crawlState, climbState, new FuncPredicate(() => climbing));
        }

        /// <summary>
        /// Enable Player Two's input
        /// </summary>
        public override void EnableInput() => inputReader.Enable();

        /// <summary>
        /// Disable Player Two's input
        /// </summary>
        public override void DisableInput() => inputReader.Disable();

        /// <summary>
        /// Start crawling for the Younger Sibling
        /// </summary>
        public void StartCrawl(List<Vector2> path)
        {
            // Set crawling to true
            crawling = true;

            // Initialize the path
            this.path = path;
            currentPathIndex = 1;

            // Set the crawl offset and size
            boxCollider.offset = new Vector2(boxCollider.offset.x, crawlOffsetAndSize.x);
            boxCollider.size = new Vector2(boxCollider.size.x, crawlOffsetAndSize.y);
        }

        /// <summary>
        /// Handle crawl logic
        /// </summary>
        public void Crawl()
        {
            // Exit case - if the path list is null
            if (path == null) return;

            // Exit case - if not crawling
            if (!crawling) return;

            // Get the current target point on the path
            Vector2 targetPoint = path[currentPathIndex];

            // Calculate the direction towards the target point
            Vector2 currentPosition = rb.position;
            Vector2 direction = (targetPoint - currentPosition).normalized;

            // Calculate the step size based on the speed and time
            float step = movementSpeed * Time.fixedDeltaTime;

            // Move the player towards the target point
            rb.MovePosition(currentPosition + direction * step);

            // Check if the player has reached the target point
            if (Vector2.Distance(currentPosition, targetPoint) <= 0.1f)
            {
                // Move to the next point in the path
                currentPathIndex++;

                // If we've reached the end of the path, stop crawling
                if (currentPathIndex >= path.Count)
                {
                    EndCrawl();
                }
            }
        }

        /// <summary>
        /// End crawling for the Younger Sibling
        /// </summary>
        public void EndCrawl()
        {
            crawling = false;

            // Set the default offset and size
            boxCollider.offset = new Vector2(boxCollider.offset.x, normalOffsetAndSize.x);
            boxCollider.size = new Vector2(boxCollider.size.x, normalOffsetAndSize.y);
        }
    }
}