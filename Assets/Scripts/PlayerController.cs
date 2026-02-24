using Assets.Scripts;
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
    public IPlayerState state;
    delegate void MoveAction();
    private GameObject fallerThatsBeingRidden;
    public GameObject punchingArm;
    private bool isPunchingLeft = false;
    private float punchingVelocity;
    private bool isPunchingRight = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //punchingArm = transform.Find("PunchingArm").gameObject;
        state = new DodgingState();
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
        state = new CrushedState();
    }
    public void rideFaller(GameObject faller)
    {
        fallerThatsBeingRidden = faller;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = faller.GetComponent<Rigidbody2D>().linearVelocity;
        gameObject.GetComponent<Rigidbody2D>().mass = 0.00001f;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = Constants.gameGravity;
        state = new RidingFallerState(faller);
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
}
