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
        public IPlayerState HandleInput(PlayerController playerController, PlayerController.MoveDirection moveInput)
        {
            IPlayerState nextState = this;
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
    }
}
