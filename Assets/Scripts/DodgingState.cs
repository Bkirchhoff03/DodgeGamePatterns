using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class DodgingState : IPlayerState
    {
        private Vector3 currentDirection = Vector3.zero;
        private List<PlayerController.MoveType> pressedDirections = new List<PlayerController.MoveType>();
        public IPlayerState HandleInput(PlayerController playerController, PlayerController.MoveType moveInput)
        {
            IPlayerState nextState = this;
            if (moveInput == PlayerController.MoveType.Left)
            {
                if (!pressedDirections.Contains(PlayerController.MoveType.Left))
                {
                    pressedDirections.Add(PlayerController.MoveType.Left);
                    if (pressedDirections.Contains(PlayerController.MoveType.Right))
                    {
                        currentDirection = Vector3.zero;
                    }
                    else
                    {
                        currentDirection = Vector3.left;
                    }
                }
                else if (pressedDirections.Contains(PlayerController.MoveType.Right))
                {
                    pressedDirections.Remove(PlayerController.MoveType.Right);
                }
            }
            else if (moveInput == PlayerController.MoveType.Right)
            {
                if (!pressedDirections.Contains(PlayerController.MoveType.Right))
                {
                    pressedDirections.Add(PlayerController.MoveType.Right);
                }
                currentDirection = Vector3.right;
            }
            else if (moveInput == PlayerController.MoveType.None)
            {
                if (!pressedDirections.Contains(PlayerController.MoveType.None))
                {
                    PlayerController.MoveType last = pressedDirections.Last();
                    if(last == PlayerController.MoveType.Left)
                    {
                        currentDirection = Vector3.left;
                    }
                    else if (last == PlayerController.MoveType.Right)
                    {
                        currentDirection = Vector3.right;
                    }
                    pressedDirections.Add(PlayerController.MoveType.None);
                }
                currentDirection = Vector3.zero;
            }
            else if (moveInput == PlayerController.MoveType.Jump)
            {
                // Transition to jumping state
                nextState = new JumpingState();
            }
            // Handle input specific to dodging state
            return nextState;
        }

        public IPlayerState Update(PlayerController playerController)
        {
            playerController.transform.GetComponent<MeshRenderer>().material.color = Color.blue;
            playerController.Move(currentDirection);
            return this;
            // Update logic specific to dodging state
        }
    }
}
