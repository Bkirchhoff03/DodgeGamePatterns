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
        private Vector3 startingjumpVelocity = new Vector3(0, 5.0f, 0);
        private Vector3 currentJumpSpeed = new Vector3(0, 5.0f, 0);

        public JumpingState()
        {
            // Initialize jumping state if needed
        }
        public IPlayerState HandleInput(PlayerController playerController, PlayerController.MoveDirection moveInput)
        {
            if (startingPosition == null)
            {
                startingPosition = playerController.transform.position;
                UnityEngine.Debug.Log("Starting Jump Position: " + startingPosition.ToString());
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
            playerController.transform.GetComponent<SpriteRenderer>().color = Color.red;
            if (playerController.transform.position.y < startingPosition.y && currentJumpSpeed.y < 0)
            {
                playerController.MoveTo(new Vector3(playerController.transform.position.x, startingPosition.y, playerController.transform.position.z));
                nextState = new DodgingState();
            }
            else
            {
                currentJumpSpeed = new Vector3(currentJumpSpeed.x, currentJumpSpeed.y + -9.8f * Time.deltaTime, currentJumpSpeed.z);
                playerController.Move(currentDirection + currentJumpSpeed);
            }
            return nextState;
            // Update logic specific to dodging state
        }
    }
}
