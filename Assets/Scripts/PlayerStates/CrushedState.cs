using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class CrushedState : IPlayerState
    {

        private Vector3 currentDirection = Vector3.zero;
        private float crushedTimer = 0.5f;
        public CrushedState()
        {
            // Initialize jumping state if needed
        }
        public IPlayerState HandleInput(PlayerController playerController, PlayerController.MoveDirection moveInput)
        {
            return this;
        }
        public string getName()
        {
            return Constants.crushedStateName;
        }
        public IPlayerState Update(PlayerController playerController)
        {
            playerController.transform.GetComponent<SpriteRenderer>().color = Color.red;
            IPlayerState nextState = this;
            if (crushedTimer > 0)
            {
                crushedTimer -= Time.deltaTime;
            }
            else
            {
                nextState = new DodgingState();
                nextState.EnterState(playerController);
            }
            return nextState;
        }
        public bool canBeDamaged()
        {
            return false;
        }
    }
}
