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
        public bool isPunch;
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
        state = new DodgingState();
    }

    // Update is called once per frame
    void Update()
    {
        state = state.Update(this);
        if(isPunchingRight)
        {
            ExecuteRightPunch();
            if(punchingArm.transform.localPosition.x <= 0.0f)
            {
                isPunchingRight = false;
                punchingArm.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }else if(isPunchingLeft)
        {
            ExecutePunchLeft();
            if(punchingArm.transform.localPosition.x >= 0.0f)
            {
                isPunchingLeft = false;
                punchingArm.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }
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
    public void punchRight()
    {
        isPunchingRight = true;
        isPunchingLeft = false;
        punchingVelocity = 2.0f;
    }
    private void ExecuteRightPunch()
    {
        punchingVelocity -= Time.deltaTime * 3.0f;
        punchingArm.transform.position += new Vector3(punchingVelocity * Time.deltaTime, 0.0f);
    }
    public void punchLeft()
    {
        isPunchingLeft = true;
        isPunchingRight = false;
        punchingVelocity = -2.0f;
    }
    public void ExecutePunchLeft()
    {
        punchingVelocity += Time.deltaTime * 3.0f;
        punchingArm.transform.position += new Vector3(punchingVelocity*Time.deltaTime, 0.0f);
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
