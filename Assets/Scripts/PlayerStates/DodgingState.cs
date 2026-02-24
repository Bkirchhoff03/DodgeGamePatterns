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
        public void EnterState(PlayerController playerController) {
            playerController.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            playerController.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            playerController.GetComponent<Rigidbody2D>().mass = 0.0001f;
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
            return nextState;
        }
        public string getName()
        {
            return Constants.dodgingStateName;
        }
        public IPlayerState Update(PlayerController playerController)
        {
            playerController.transform.GetComponent<SpriteRenderer>().color = Color.blue;
            playerController.Move(currentDirection);
            return this;
            // Update logic specific to dodging state
        }
        public bool canBeDamaged()
        {
            return true;
        }
    }
}
