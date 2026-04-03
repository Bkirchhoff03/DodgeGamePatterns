using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class DodgingState : IPlayerState
    {
        private bool moving = false;
        private int leftNoneRight = 0;
        private Vector3 currentDirection = Vector3.zero;
        private float timeSinceStopped = 0f;
        private const float idleDelay = Constants.idleDelay;
        public void EnterState(PlayerController playerController) {
            playerController.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            playerController.GetComponent<Rigidbody2D>().gravityScale = Constants.playerGravity;
            playerController.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            //playerController.GetComponent<Rigidbody2D>().mass = 0.0001f;
        }
        public void ExitState(PlayerController playerController) { }

        public IPlayerState HandleInput(PlayerController playerController, PlayerController.MoveDirection moveInput)
        {
            IPlayerState nextState = this;
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
            return nextState;
        }
        public string getName()
        {
            return Constants.dodgingStateName;
        }
        public IPlayerState Update(PlayerController playerController)
        {
            //playerController.PlayerAnimationGameObject.transform.GetComponent<SpriteRenderer>().color = Color.blue;
            if (moving && !playerController.PlayerAnimator.GetBool("Running"))
            {
                playerController.PlayerAnimator.SetBool("Running", true);
            }
            else if (!moving && playerController.PlayerAnimator.GetBool("Running"))
            {
                playerController.PlayerAnimator.SetBool("Running", false);
            }
            if(leftNoneRight != 0)
            {
                playerController.PlayerAnimationGameObject.GetComponent<SpriteRenderer>().flipX = leftNoneRight == -1;
            }
            Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(leftNoneRight * Constants.moveSpeed, rb.linearVelocity.y);
            //playerController.Move(currentDirection);
            return this;
            // Update logic specific to dodging state
        }
        public bool canBeDamaged()
        {
            return true;
        }
    }
}
