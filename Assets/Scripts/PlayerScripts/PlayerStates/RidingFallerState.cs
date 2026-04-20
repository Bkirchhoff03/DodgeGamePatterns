using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class RidingFallerState : IPlayerState
    {
        private bool moving = false;
        private int leftNoneRight = 0;
        private float timeSinceStopped = 0f;
        private const float idleDelay = Constants.idleDelay;
        private float leftSideFallerBound = Constants.minX;
        private float rightSideFallerBound = Constants.maxX;
        private bool isFallingOffFaller = false;
        private Vector3 currentDirection = Vector3.zero;
        private GameObject ridingFaller;
        public void EnterState(PlayerController playerController) {
            if (ridingFaller == null)
            {
                ridingFaller = FallerManager.instance().GetFallerBeingRidden().fallerObject;
            }
            ridingFaller.GetComponent<FallerController>().StartRiding();
        }
        public void ExitState(PlayerController playerController)
        {
            ridingFaller.GetComponent<FallerController>().StopRiding();
        }
        public RidingFallerState(GameObject faller) {
            if(faller == null)
            {
                Debug.LogError("Faller cannot be null when entering RidingFallerState");
                return;
            }
            ridingFaller = faller;
        }
        public IPlayerState HandleInput(PlayerController playerController, PlayerController.MoveDirection moveInput)
        {
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
            /*if (moveInput.Ydirection > 0)
            {
                currentDirection += Vector3.up;
            }*/
            // Handle input specific to dodging state
            return this;
        }
        public string getName()
        {
            return Constants.ridingFallerStateName;
        }
        public IPlayerState Update(PlayerController playerController)
        {
            if (ridingFaller == null)
            {
                Debug.LogError("RidingFallerState Update called with null ridingFaller");
                ExitState(playerController);
                return new FallingState();
            }
            IPlayerState newState = this;
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

            if (playerController.isGrounded())
            {
                playerController.transform.GetComponent<Rigidbody2D>().gravityScale = 0f;
                playerController.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                playerController.PlayerAnimationGameObject.GetComponent<SpriteRenderer>().color = Color.white;
                ExitState(playerController);
                newState = new DodgingState();
                newState.EnterState(playerController);
            }
            else if (!isFallingOffFaller)
            {
                //playerController.PlayerAnimationGameObject.transform.GetComponent<SpriteRenderer>().color = Color.magenta;
                if (leftNoneRight != 0)
                { 
                    Rigidbody2D rb = playerController.transform.GetComponent<Rigidbody2D>();
                    //rb.linearVelocity = new Vector2(leftNoneRight * Constants.moveSpeed, rb.linearVelocity.y);
                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                    playerController.Move(currentDirection);
                }
                else
                {
                    Rigidbody2D rb = playerController.transform.GetComponent<Rigidbody2D>();
                    rb.linearVelocity = new Vector2(0f, Mathf.Min(rb.linearVelocity.y, 0f));
                }
                if (!ridingFaller.GetComponent<FallerController>().isRidingMe(playerController.transform.position))
                {
                    newState = new FallingState();
                    newState.EnterState(playerController);
                    isFallingOffFaller = true;
                }
                
            }
            
            return newState;
            // Update logic specific to dodging state
        }
        public void FallBounds(float leftSideBound, float rightSideBound)
        {
            leftSideFallerBound = leftSideBound;
            rightSideFallerBound = rightSideBound;
        }
        public bool canBeDamaged()
        {
            return true;
        }
    }
}
