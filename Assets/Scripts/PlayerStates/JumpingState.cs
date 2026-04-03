using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class JumpingState : IPlayerState
    {
        private bool moving = false;
        private int leftNoneRight = 0;
        private Vector3 currentDirection = Vector3.zero;
        private Vector3 startingPosition;
        private float timeSinceStopped = 0f;
        private const float idleDelay = Constants.idleDelay;
        private Vector3 startingjumpVelocity = new Vector3(0, 22f, 0);
        private Vector3 currentJumpSpeed = new Vector3(0, 5.0f, 0);

        public JumpingState()
        {
            // Initialize jumping state if needed
        }
        public void EnterState(PlayerController playerController) {
            playerController.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            playerController.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

            playerController.gameObject.GetComponent<Rigidbody2D>().linearVelocity = startingjumpVelocity;
            playerController.gameObject.GetComponent<Rigidbody2D>().gravityScale = Constants.playerGravity;
        }
        public void ExitState(PlayerController playerController) {
            /*playerController.transform.GetComponent<Rigidbody2D>().gravityScale = 0f;
            playerController.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;*/
            
        }

        public IPlayerState HandleInput(PlayerController playerController, PlayerController.MoveDirection moveInput)
        {
            if (startingPosition == null)
            {
                startingPosition = playerController.transform.position;
                //UnityEngine.Debug.Log("Starting Jump Position: " + startingPosition.ToString());
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
        public string getName()
        {
            return Constants.jumpingStateName;
        }
        public IPlayerState Update(PlayerController playerController)
        {
            IPlayerState nextState = this;
            //UnityEngine.Debug.Log("Starting Jump Position: " + startingPosition.ToString());
            Rigidbody2D rb = playerController.gameObject.GetComponent<Rigidbody2D>();
            //playerController.PlayerAnimationGameObject.transform.GetComponent<SpriteRenderer>().color = Color.green;
            /*if (playerController.transform.position.y < startingPosition.y) // && currentJumpSpeed.y < 0)
            {
                playerController.MoveTo(new Vector3(playerController.transform.position.x, startingPosition.y, playerController.transform.position.z));
                ExitState(playerController);
                nextState = new DodgingState();
                nextState.EnterState(playerController);
            }
            else */
            Debug.Log("Current Jump Speed: " + rb.linearVelocity.y);
            if (rb.linearVelocity.y <= 0f)
            {
                ExitState(playerController);
                nextState = new FallingState();
                nextState.EnterState(playerController);
                Debug.Log("Transitioning to Falling State");
            }
            else
            {
                //currentJumpSpeed = new Vector3(currentJumpSpeed.x, currentJumpSpeed.y + -9.8f * Time.deltaTime, currentJumpSpeed.z);
                playerController.Move(currentDirection);
            }
            if (moving && !playerController.PlayerAnimator.GetBool("Running"))
            {
                playerController.PlayerAnimator.SetBool("Running", true);
            }
            else if (!moving && playerController.PlayerAnimator.GetBool("Running"))
            {
                playerController.PlayerAnimator.SetBool("Running", false);
            }
            if (leftNoneRight != 0)
            {
                playerController.PlayerAnimationGameObject.GetComponent<SpriteRenderer>().flipX = leftNoneRight == -1;
            }
            return nextState;
            // Update logic specific to dodging state
        }
        public bool canBeDamaged()
        {
            return false;
        }
    }
}
