using Assets.Scripts;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public class MoveDirection
    {
        public int Xdirection;
        public int Ydirection;
        
    }
    public IPlayerState state;
    delegate void MoveAction();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            state = newState;
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
}
