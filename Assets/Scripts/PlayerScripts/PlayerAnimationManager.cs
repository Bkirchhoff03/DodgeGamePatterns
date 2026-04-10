using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PlayerScripts
{
    public class PlayerAnimationManager : MonoBehaviour
    {
        private GameObject animatorGameObject;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private bool directionFacingLeft = false;
        public PlayerAnimationState currentState;
        private bool Running;
        private string RunningParameter = "Running";
        private bool Idle;
        private string IdleParameter = "Idle";
        private bool Punching;
        private string PunchingParameter = "Punching";
        private bool Crushed;
        private string CrushedParameter = "Crush";
        private string CrushedFromWhere = "";
        public enum PlayerAnimationState
        {
            Idle,
            Run,
            Punch,
            Crushed
        }
        void Start()
        {
            animatorGameObject = this.gameObject;
            spriteRenderer = this.GetComponent<SpriteRenderer>();
            animator = animatorGameObject.GetComponent<Animator>();
            Running = false;
            Idle = true;
            Punching = false;
            Crushed = false;
        }
        private void Update()
        {
            //GameManager.instance().Print("Current Animation State: " + animator.GetCurrentAnimatorClipInfo(0)[0].clip.name + " Crushed: " + Crushed + " Punching: " + Punching + " Running: " + Running + " Idle: " + Idle, 1);
            animator.SetBool(CrushedParameter, Crushed);
            animator.SetBool(PunchingParameter, Punching);
            animator.SetBool(RunningParameter, Running);
            animator.SetBool(IdleParameter, Idle);
            /*if (Crushed)
            {
                animator.SetBool(CrushedParameter, true);
                animator.SetBool(PunchingParameter, Punching);
                animator.SetBool(RunningParameter, Running);
                animator.SetBool(IdleParameter, Idle);
            }
            else
            {
                animator.SetBool(CrushedParameter, false);
                if (Punching)
                {
                    animator.SetBool(PunchingParameter, true);
                }
                else
                {
                    animator.SetBool(PunchingParameter, false);
                    if (Running)
                    {
                        animator.SetBool(RunningParameter, true);
                    }
                    else
                    {
                        animator.SetBool(RunningParameter, false);
                        if (Idle)
                        {
                            animator.SetBool(IdleParameter, true);
                        }
                        else
                        {
                            animator.SetBool(IdleParameter, false);
                        }
                    }
                }
            }*/
        }
        public bool isRunning()
        {
            return Running;
        }
        public bool isIdle()
        {
            return Idle;
        }
        public bool isPunching()
        {
            return Punching;
        }
        public bool isCrushed()
        {
            return Crushed;
        }
        public void lookLeft(bool isArmController = false)
        {
            if (!isArmController && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerPunchingAnimation"))
            {
                return; // Don't flip the sprite if the arm controller is trying to flip while punching, as it will cause visual bugs
            }
            directionFacingLeft = true;
            spriteRenderer.flipX = directionFacingLeft;
        }
        public void lookRight(bool isArmController = false)
        {
            if(!isArmController && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerPunchingAnimation"))
            {
                return; // Don't flip the sprite if the arm controller is trying to flip while punching, as it will cause visual bugs
            }
            directionFacingLeft = false;
            spriteRenderer.flipX = directionFacingLeft;
        }
        
        public void SetToNewState(PlayerAnimationState state)
        {
            if (currentState == state) return;
            switch (currentState)
            {
                case PlayerAnimationState.Idle:
                    animator.SetBool(IdleParameter, false);
                    break;
                case PlayerAnimationState.Run:
                    animator.SetBool(RunningParameter, false);
                    break;
                case PlayerAnimationState.Punch:
                    animator.SetBool(PunchingParameter, false);
                    break;
                case PlayerAnimationState.Crushed:
                    animator.SetBool(CrushedParameter, false);
                    break;
            }
            animator.SetBool(state.ToString(), true);
            currentState = state;
        }
        public void SetRunning(bool value)
        {
            //GameManager.instance().Print("Attempting to set running from " + Running + " to " + value, 1);
            if (value && !Running)
            {
                // Attempt a transition to running
                if (Crushed)
                {
                    return; // Can't transition to running if currently crushed
                }
                if(Punching)
                {
                    Running = value; // Set running so that it goes to running animation after punching finishes
                    Idle = false; // Set idle to false so that it doesn't go back to idle after punching finishes

                    return;
                }
                Running = value;
                Idle = false; // Set idle to false so that it doesn't go back to idle after punching finishes
            }else if(!value && Running)
            {
                Running = value;
                // Goes from running to not running, reset to idle
                Idle = true;
            }
        }
        public void SetIdle(bool value)
        {
            //GameManager.instance().Print("Attempting to set idle from " + Idle + " to " + value, 1);
            if (value && !Idle)
            {
                // Attempt a transition to idle
                if (Crushed)
                {
                    return; // Can't transition to idle if currently crushed
                }
                if (Punching)
                {
                    Idle = value; // Set idle so that it goes to idle animation after punching finishes
                    Running = false; // Set running to false so that it doesn't go back to running after punching finishes
                    return;
                }
                Running = false; // Set running to false running and idle can't be true at the same time
                Idle = value;
            }
            else if (!value && Idle)
            {
                // Transition away from idle, only set running to true as the other options are crushed or punching which should be handled separately
                Running = true;
                Idle = false;
            }
        }
        public void SetPunching(bool value)
        {
            //GameManager.instance().Print("Attempting to set punching from " + Punching + " to " + value, 1);
            if (value && !Punching)
            {
                // Attempt a transition to punching
                if (Crushed)
                {
                    return; // Can't transition to punching if currently crushed
                }
                Punching = value;
            }else if(!value && Punching)
            {
                Punching = value;
            }

        }
        public void SetCrushed(bool value)
        {
            //GameManager.instance().Print("Attempting to set crushed from " + Crushed + " to " + value, 1);
            if(value && !Crushed)
            {
                // When crushed, reset all states so it goes idle after crush finishes
                Idle = false;
                Running = false;
                Punching = false;
            }
            else if(!value && Crushed)
            {
                // Goes from crushed to not crushed, reset to idle
                Idle = true;
            }
            Crushed = value;
        }
    }
}
