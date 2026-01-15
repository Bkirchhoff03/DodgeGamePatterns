using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Assets.Scripts
{
    class FallingState : IPlayerState
    {
        private Vector3 currentDirection = Vector3.zero;
        private Vector3 startingPosition;

        public void EnterState(PlayerController playerController)
        {
            playerController.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            playerController.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            playerController.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            playerController.gameObject.GetComponent<Rigidbody2D>().gravityScale = Constants.playerGravity;
            
        }
        public void ExitState(PlayerController playerController)
        {
            playerController.transform.GetComponent<Rigidbody2D>().gravityScale = 0f;
            playerController.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
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

            // Handle input specific to dodging state
            return this;
        }
        public IPlayerState Update(PlayerController playerController)
        {
            IPlayerState nextState = this;
            playerController.transform.GetComponent<SpriteRenderer>().color = Color.yellow;
            if (playerController.isGrounded())
            {
                //playerController.MoveTo(new Vector3(playerController.transform.position.x, startingPosition.y, playerController.transform.position.z));
                ExitState(playerController);
                nextState = new DodgingState();
                nextState.EnterState(playerController);
            }
            //currentJumpSpeed = new Vector3(currentJumpSpeed.x, currentJumpSpeed.y + -9.8f * Time.deltaTime, currentJumpSpeed.z);
            playerController.Move(currentDirection);
            return nextState;
        }
    }
    
}
