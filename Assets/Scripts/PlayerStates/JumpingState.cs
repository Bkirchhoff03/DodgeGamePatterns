using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class JumpingState : IPlayerState
    {

        private Vector3 currentDirection = Vector3.zero;
        private Vector3 startingPosition;
        private Vector3 startingjumpVelocity = new Vector3(0, 25.0f, 0);
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
            playerController.transform.GetComponent<Rigidbody2D>().gravityScale = 0f;
            playerController.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            playerController.transform.GetComponent<SpriteRenderer>().color = Color.white;
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
        public string getName()
        {
            return Constants.jumpingStateName;
        }
        public IPlayerState Update(PlayerController playerController)
        {
            IPlayerState nextState = this;
            UnityEngine.Debug.Log("Starting Jump Position: " + startingPosition.ToString());
            playerController.transform.GetComponent<SpriteRenderer>().color = Color.green;
            if (playerController.transform.position.y < startingPosition.y) // && currentJumpSpeed.y < 0)
            {
                playerController.MoveTo(new Vector3(playerController.transform.position.x, startingPosition.y, playerController.transform.position.z));
                ExitState(playerController);
                nextState = new DodgingState();
                nextState.EnterState(playerController);
            }
            else
            {
                //currentJumpSpeed = new Vector3(currentJumpSpeed.x, currentJumpSpeed.y + -9.8f * Time.deltaTime, currentJumpSpeed.z);
                playerController.Move(currentDirection);
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
