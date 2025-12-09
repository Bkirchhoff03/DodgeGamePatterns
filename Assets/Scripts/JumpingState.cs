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
        public IPlayerState HandleInput(PlayerController playerController, PlayerController.MoveType moveInput)
        {
            if (startingPosition == null)
            {
                startingPosition = playerController.transform.position;
                UnityEngine.Debug.Log("Starting Jump Position: " + startingPosition.ToString());
            }
            if (moveInput == PlayerController.MoveType.Left)
            {
                currentDirection = Vector3.left;
                // Handle left movement while jumping
            }
            else if (moveInput == PlayerController.MoveType.Right)
            {
                currentDirection = Vector3.right;
                // Handle right movement while jumping
            }
            else if (moveInput == PlayerController.MoveType.None)
            {
                currentDirection = Vector3.zero;
                // Handle no movement while jumping
            }
            else if (moveInput == PlayerController.MoveType.Jump)
            {
                // Already jumping, maybe ignore or handle double jump
            }
            // Handle input specific to dodging state
            return this;
        }

        public IPlayerState Update(PlayerController playerController)
        {
            IPlayerState nextState = this;
            UnityEngine.Debug.Log("Starting Jump Position: " + startingPosition.ToString());
            playerController.transform.GetComponent<MeshRenderer>().material.color = Color.red;
            if(playerController.transform.position.y < startingPosition.y && currentJumpSpeed.y < 0)
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
