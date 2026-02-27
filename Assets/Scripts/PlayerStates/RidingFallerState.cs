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
        private float leftSideFallerBound = Constants.minX;
        private float rightSideFallerBound = Constants.maxX;
        private bool isFallingOffFaller = false;
        private Vector3 currentDirection = Vector3.zero;
        private GameObject ridingFaller;
        public void EnterState(PlayerController playerController) {
            ridingFaller.GetComponent<FallerController>().StartRiding();
        }
        public void ExitState(PlayerController playerController)
        {
            ridingFaller.GetComponent<FallerController>().StopRiding();
        }
        public RidingFallerState(GameObject faller) {
            ridingFaller = faller;
        }
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
            }
            else if (moveInput.Xdirection > 0)
            {
                currentDirection = Vector3.right;
            }
            else
            {
                currentDirection = Vector3.zero;
            }
            /*if (moveInput.Ydirection > 0)
            {
                currentDirection += Vector3.up;
            }*/
            // Handle input specific to dodging state
            return nextState;
        }
        public string getName()
        {
            return Constants.ridingFallerStateName;
        }
        public IPlayerState Update(PlayerController playerController)
        {
            IPlayerState newState = this;
            if (playerController.isGrounded())
            {
                playerController.transform.GetComponent<Rigidbody2D>().gravityScale = 0f;
                playerController.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                playerController.transform.GetComponent<SpriteRenderer>().color = Color.white;
                newState = new DodgingState();
                newState.EnterState(playerController);
            }
            else if (!isFallingOffFaller)
            {
                playerController.transform.GetComponent<SpriteRenderer>().color = Color.magenta;
                playerController.Move(currentDirection);
                if (ridingFaller.GetComponent<FallerController>().isRidingMe(playerController.transform.position))
                {
                    newState = new FallingState();
                    newState.EnterState(playerController);
                    isFallingOffFaller = true;
                }
                /*if(playerController.transform.position.x < leftSideFallerBound || playerController.transform.position.x > rightSideFallerBound)
                {
                    isFallingOffFaller = true;
                    playerController.transform.GetComponent<Rigidbody2D>().gravityScale = 9.8f;
                    playerController.transform.GetComponent<SpriteRenderer>().color = Color.white;
                }*/
            }
            else
            {
                playerController.Move(currentDirection);
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
