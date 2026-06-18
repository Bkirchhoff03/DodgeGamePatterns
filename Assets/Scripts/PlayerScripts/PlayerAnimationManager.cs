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
        private readonly string RunningParameter = "Running";
        private bool Idle;
        private readonly string IdleParameter = "Idle";
        private bool Punching;
        private readonly string PunchingParameter = "Punching";
        private bool Crushed;
        private readonly string CrushedParameter = "Crush";
        private float checkAnimationStateTimer = 0.0f;
        private readonly float checkAnimationInterval = 0.5f;
        //private string CrushedFromWhere = "";
        //private float punchFrameTimer = 0.0f;
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
            if(checkAnimationStateTimer >= checkAnimationInterval)
            {
                checkAnimationStates();
                checkAnimationStateTimer = 0.0f;
            }
            else
            {
                checkAnimationStateTimer += Time.deltaTime;
            }
            
        }

        private void checkAnimationStates()
        {
            if(!Crushed && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCrushedAnimation"))
            {
                // Crushed is already false but the animation is stuck — SetCrushed(false) would be a no-op here
                // so force the state directly
                GameManager.instance().Print("Player is not crushed but is playing crushed animation, resetting to idle", 7);
                Idle = true;
                animator.SetBool(CrushedParameter, false);
                animator.SetBool(PunchingParameter, false);
                animator.SetBool(RunningParameter, false);
                animator.SetBool(IdleParameter, true);
                animator.Play("PlayerIdleAnimation");
            }
            if (!Punching && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerPunchingAnimation"))
            {
                GameManager.instance().Print("Player is not punching but is playing punching animation, resetting to idle", 7);
                SetPunching(false); // This will also reset to idle after punching animation finishes
                animator.SetBool(CrushedParameter, Crushed);
                animator.SetBool(PunchingParameter, Punching);
                animator.SetBool(RunningParameter, Running);
                animator.SetBool(IdleParameter, Idle);
            }
            if(!Running && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerRunningAnimation"))
            {
                GameManager.instance().Print("Player is not running but is playing running animation, resetting to idle", 7);
                SetRunning(false); // This will also reset to idle after running animation finishes
                animator.SetBool(CrushedParameter, Crushed);
                animator.SetBool(PunchingParameter, Punching);
                animator.SetBool(RunningParameter, Running);
                animator.SetBool(IdleParameter, Idle);
            }
            if(!Idle && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdleAnimation"))
            {
                GameManager.instance().Print("Player is not idle but is playing idle animation, resetting to idle", 7);
                SetIdle(false); // This will also reset to idle after idle animation finishes
                animator.SetBool(CrushedParameter, Crushed);
                animator.SetBool(PunchingParameter, Punching);
                animator.SetBool(RunningParameter, Running);
                animator.SetBool(IdleParameter, Idle);
            }
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
            GameManager.instance().Print("Attempting to set punching from " + Punching + " to " + value, 7);
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
            }else if(value && Punching)
            {
                animator.Play("PlayerPunchingAnimation", 0, 0f); // Restart punching animation if already punching and trying to punch again (e.g. for chained punches)
            }

        }
        public void SetCrushed(bool value)
        {
            GameManager.instance().Print("Attempting to set crushed from " + Crushed + " to " + value, 7);
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
                animator.SetBool(CrushedParameter, false);
                animator.SetBool(IdleParameter, true);
                // Force the animator out of CrushedAnimation in case the bool-based transition doesn't fire
                // (can happen when crushed states occur back-to-back)
                animator.Play("PlayerIdleAnimation");
            }
            Crushed = value;
            // Apply immediately so the Animator sees the change this frame regardless of script execution order
            animator.SetBool(CrushedParameter, Crushed);
        }
        // In PlayerAnimationManager
        public bool IsPunchAnimationComplete()
        {
            return Punching
                && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerPunchingAnimation")
                && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f;
        }
    }
}
