using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Assets.Scripts
{
    public class FallingState : IPlayerState
    {
        private bool moving = false;
        private int leftNoneRight = 0;
        private Vector3 currentDirection = Vector3.zero;
        private Vector3 startingPosition;
        private float timeSinceStopped = 0f;
        private const float idleDelay = Constants.idleDelay;
        private float timeInState = 0f;
        private const float wedgeDetectionDelay = 0.3f;
        private const float wedgeVelocityThreshold = 1.0f;

        public void EnterState(PlayerController playerController)
        {
            playerController.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            playerController.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            playerController.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            playerController.gameObject.GetComponent<Rigidbody2D>().gravityScale = Constants.playerGravity;
            
        }
        public void ExitState(PlayerController playerController)
        {
            playerController.transform.GetComponent<SpriteRenderer>().color = Color.white;
        }
        public string getName()
        {
            return Constants.fallingStateName;
        }
        public bool canBeDamaged()
        {
            return false;
        }
        public IPlayerState HandleInput(PlayerController playerController, PlayerController.MoveDirection moveInput)
        {
            if (startingPosition == null)
            {
                startingPosition = playerController.transform.position;
            }
            if (moveInput.isPunch != 0)
            {
                playerController.HandlePunch(moveInput);
            }
            if (moveInput.Xdirection < 0)
            {
                currentDirection = Vector3.left;
                moving = true;
                leftNoneRight = -1;
                timeSinceStopped = 0f;
            }
            else if (moveInput.Xdirection > 0)
            {
                currentDirection = Vector3.right;
                moving = true;
                leftNoneRight = 1;
                timeSinceStopped = 0f;
            }
            else
            {
                currentDirection = Vector3.zero;
                timeSinceStopped += Time.deltaTime;
                if (timeSinceStopped >= idleDelay)
                {
                    moving = false;
                    leftNoneRight = 0;
                }
            }

            // Handle input specific to dodging state
            return this;
        }
        public IPlayerState Update(PlayerController playerController)
        {
            GameManager.instance().Print("Falling state linear velocity " + playerController.gameObject.GetComponent<Rigidbody2D>().linearVelocity);
            IPlayerState nextState = this;
            timeInState += Time.deltaTime;
            //playerController.PlayerAnimationGameObject.transform.GetComponent<SpriteRenderer>().color = Color.yellow;
            if (playerController.isGrounded())
            {
                GameManager.instance().Print("Player is grounded, switching to dodging state from falling state at " + playerController.gameObject.transform.position , 1);
                //playerController.MoveTo(new Vector3(playerController.transform.position.x, startingPosition.y, playerController.transform.position.z));
                ExitState(playerController);
                nextState = new DodgingState();
                nextState.EnterState(playerController);
            }
            else if (timeInState > wedgeDetectionDelay &&
                     Mathf.Abs(playerController.gameObject.GetComponent<Rigidbody2D>().linearVelocity.y) < wedgeVelocityThreshold)
            {
                // Player is wedged between fallers (V-shape) — side contacts prevent a Top collision
                // from being detected. Escape to DodgingState so they can move out horizontally.
                ExitState(playerController);
                nextState = new DodgingState();
                nextState.EnterState(playerController);
                GameManager.instance().Print("Player is likely wedged between fallers, switching to dodging state", 1);
            }
            if (moving && !playerController.animationManager.isRunning())// !playerController.PlayerAnimator.GetBool("Running"))
            {
                playerController.animationManager.SetRunning(true);
            }
            else if (!moving && playerController.animationManager.isRunning())
            {
                playerController.animationManager.SetRunning(false);
            }
            if (leftNoneRight != 0)
            {
                if (leftNoneRight == -1)
                {
                    playerController.animationManager.lookLeft();
                }
                else
                {
                    playerController.animationManager.lookRight();
                }
            }
            //currentJumpSpeed = new Vector3(currentJumpSpeed.x, currentJumpSpeed.y + -9.8f * Time.deltaTime, currentJumpSpeed.z);
            playerController.Move(currentDirection);
            return nextState;
        }
    }
    
}
