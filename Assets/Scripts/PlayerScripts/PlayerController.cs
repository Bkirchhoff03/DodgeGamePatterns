using Assets.Scripts;
using Assets.Scripts.PlayerScripts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public class MoveDirection
    {
        public int Xdirection;
        public int Ydirection;
        public int isPunch;
    }

    [System.Serializable]
    public class PlayerData
    {
        public string name;
        public Vector3 position;
        public Vector2 currentSpeed;
        public string currentStateName;
    }
    public IPlayerState state;
    delegate void MoveAction();
    private GameObject fallerThatsBeingRidden;
    public GameObject punchingArm;
    public GameObject PlayerAnimationGameObject;
    public Animator PlayerAnimator;
    public PlayerAnimationManager animationManager;
    private float leftOrRightOrNone = 0f;
    private bool running = false;
    //private bool isPunchingLeft = false;
    //private float punchingVelocity;
    //private bool isPunchingRight = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.instance() != null && GameManager.instance().spawnFallersFromFile)
        {
            return;
        }
        state = new DodgingState();
        state.EnterState(this);
        punchingArm.GetComponent<SpriteRenderer>().enabled = false;
        PlayerAnimator = PlayerAnimationGameObject.GetComponent<Animator>();
        animationManager = PlayerAnimationGameObject.GetComponent<PlayerAnimationManager>();
        GetComponent<Rigidbody2D>().mass = 0.00001f;

    }

    // Update is called once per frame
    void Update()
    {
        state = state.Update(this);
        
    }
    public void HandleInput(PlayerController.MoveDirection moveInput)
    {
        state = state.HandleInput(this, moveInput);
    }
    public void setState(IPlayerState newState)
    {
        if(newState.getName() != state.getName())
        {
            if(state.getName() == Constants.fallingStateName && newState.getName() == Constants.jumpingStateName)
            {
                return;
            }
            if(newState.getName() == Constants.jumpingStateName && state.getName() == Constants.ridingFallerStateName)
            {
                gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            }
            state.ExitState(this);
            state = newState;
            state.EnterState(this);
        }
    }
    public void Move(Vector3 direction)
    {
        transform.position += direction * Time.deltaTime * Constants.moveSpeed;
        if (direction.x < 0 && leftOrRightOrNone >= 0)
        {
            //SetAnimationDirection(direction);
            leftOrRightOrNone = -1;
        }
        else if (direction.x > 0 && leftOrRightOrNone <= 0)
        {
            //SetAnimationDirection(direction);
            leftOrRightOrNone = 1;
        }
    }
    
    private void SetAnimationDirection(Vector3 direction)
    {
        if (direction.x > 0.0f)
        {
            if (!PlayerAnimator.GetBool("Running"))
            {
                PlayerAnimator.SetBool("Running", true);
            }
            //PlayerAnimationGameObject.GetComponent<Animator>().speed = 1f;
            PlayerAnimationGameObject.GetComponent<SpriteRenderer>().flipX = false;
            /*if(PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                PlayerAnimator.Play("PlayerRunningAnimation");
            }*/
            GameManager.instance().Print("Moving right flip x off");
        }
        else if (direction.x < 0.0f)
        {
            if (!PlayerAnimator.GetBool("Running"))
            {
                PlayerAnimator.SetBool("Running", true);
            }
            //PlayerAnimationGameObject.GetComponent<Animator>().speed = 1f;
            GameManager.instance().Print("Moving left flip x on");
            PlayerAnimationGameObject.GetComponent<SpriteRenderer>().flipX = true;
            /*if (PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                PlayerAnimator.Play("PlayerRunningAnimation");
            }*/
        }
        else
        {
            GameManager.instance().Print("Not moving horizontally, Go idle");
            PlayerAnimator.SetBool("Running", false);
            //PlayerAnimationGameObject.GetComponent<SpriteRenderer>().flipX = false;
            /*if(!PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                PlayerAnimator.Play("Idle");
            }*/
            //PlayerAnimationGameObject.GetComponent<Animator>().Play("Idle");
            //PlayerAnimationGameObject.GetComponent<Animator>().speed = 0f;
        }
    }
    public void MoveTo(Vector3 position)
    {
        transform.position = position;
    }
    public bool canBeDamaged()
    {
        return state.canBeDamaged();
    }
    public void crush()
    {
        IPlayerState previousState = state;
        state = new CrushedState(previousState);
        state.EnterState(this);
    }
    public void rideFaller(GameObject faller)
    {
        fallerThatsBeingRidden = faller;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = faller.GetComponent<Rigidbody2D>().linearVelocity;
        gameObject.GetComponent<Rigidbody2D>().mass = 0.00001f;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = Constants.playerGravity;
        state = new RidingFallerState(faller);
        state.EnterState(this);
    }
    public void HandlePunch(MoveDirection moveInput)
    {
        if(moveInput.isPunch == 1)
        {
            punchingArm.GetComponent<PunchingArmController>().PunchRight();
        }else if(moveInput.isPunch == -1)
        {
            punchingArm.GetComponent<PunchingArmController>().PunchLeft();
        }
    }
    public PlayerData GetMyData()
    {
        PlayerData data = new PlayerData();
        data.name = gameObject.name;
        data.position = gameObject.transform.position;
        data.currentSpeed = GetComponent<Rigidbody2D>().linearVelocity;
        data.currentStateName = state.getName();
        return data;
    }
    public void SetFromData(PlayerData data)
    {
        gameObject.name = data.name;
        gameObject.transform.position = data.position;
        GetComponent<Rigidbody2D>().linearVelocity = data.currentSpeed;
        state = GetStateFromName(data.currentStateName);
        state.EnterState(this);
    }

    public IPlayerState GetStateFromName(string currentStateName)
    {
        GameManager.instance().Print("Setting player state from name: " + currentStateName);
        switch (currentStateName)
        {
            case Constants.crushedStateName:
                return new CrushedState(null);
            case Constants.dodgingStateName:
                return new DodgingState();
            case Constants.jumpingStateName:
                return new JumpingState();
            case Constants.ridingFallerStateName:
                return new RidingFallerState(FallerManager.instance().GetFallerBeingRidden().fallerObject);
            default:
                return new DodgingState();
        }
    }

    internal bool isGrounded()
    {
        if (gameObject.transform.position.y <= 0.1f) //Physics2D.Raycast(transform.position, Vector2.down, 0.1f))
        {
            MoveTo(new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z));
            return true;
        }else
        {
            return false;
        }
    }

    public void BounceOff(GameObject faller, GameManager.PlayerFallerCollisionType collisionType)
    {
        GameManager.instance().Print("Bouncing off faller. Collision type: " + collisionType);
        //ensure player does not go through the faller, but stays on the same side of the faller as they were before the collision
        Vector3 newPosition = transform.position;
        switch (collisionType)
        {
            case GameManager.PlayerFallerCollisionType.Top:
                newPosition.y = faller.transform.position.y + faller.GetComponent<SpriteRenderer>().bounds.size.y / 2 + GetComponent<SpriteRenderer>().bounds.size.y / 2;
                break;
            case GameManager.PlayerFallerCollisionType.Bottom:
                newPosition.y = faller.transform.position.y - faller.GetComponent<SpriteRenderer>().bounds.size.y / 2 - GetComponent<SpriteRenderer>().bounds.size.y / 2;
                break;
            case GameManager.PlayerFallerCollisionType.Left:
                newPosition.x = faller.transform.position.x - faller.GetComponent<SpriteRenderer>().bounds.size.x / 2 - GetComponent<SpriteRenderer>().bounds.size.x / 2;
                break;
            case GameManager.PlayerFallerCollisionType.Right:
                newPosition.x = faller.transform.position.x + faller.GetComponent<SpriteRenderer>().bounds.size.x / 2 + GetComponent<SpriteRenderer>().bounds.size.x / 2;
                break;
        }
        MoveTo(newPosition);
    }
}
