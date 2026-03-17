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
        private IPlayerState lastState;
        private Vector3 currentDirection = Vector3.zero;
        private float crushedTimer = 2.5f;
        private int animationStateHash;
        public CrushedState(IPlayerState lastState)
        {
            if(lastState == null)
            {
                this.lastState = new DodgingState();
            }
            else 
            {
                this.lastState = lastState;
            }
                
            // Initialize jumping state if needed
        }
        public void EnterState(PlayerController playerController)
        {

            Debug.Log("Entering Crushed State");
            playerController.PlayerAnimation.GetComponent<Animator>().Play("PlayerCrushedAnimation");
            animationStateHash = playerController.PlayerAnimation.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash;
        }
        public void ExitState(PlayerController playerController)
        {
            playerController.PlayerAnimation.GetComponent<Animator>().Play(animationStateHash);
            /*playerController.PlayerAnimation.GetComponent<Animator>().ResetTrigger("Crush");
            playerController.PlayerAnimation.GetComponent<Animator>().SetTrigger("CrushFinished");*/
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
            Debug.Log(playerController.PlayerAnimation.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash);
            playerController.transform.GetComponent<SpriteRenderer>().color = Color.red;
            IPlayerState nextState = this;
            if (crushedTimer > 0)
            {
                crushedTimer -= Time.deltaTime;
            }
            else //if(!playerController.PlayerAnimation.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerCrushedAnimation"))
            {
                nextState = lastState;
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
