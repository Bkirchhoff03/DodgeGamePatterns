using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemController : MonoBehaviour
{
    PlayerController playerController;
    public enum keys{
        Left,
        Right,
        Jump
    }
    private List<keys> pressedDirections = new List<keys>();

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }
    private void Update()
    {
        PlayerController.MoveDirection moveDirection = new PlayerController.MoveDirection();
        if(Input.GetKey(KeyCode.A))
        {
            moveDirection.Xdirection -= 1;
            //pressedDirections.Add(keys.Left);
            //playerController.HandleInput(PlayerController.MoveType.Left, pressedDirections);
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection.Xdirection += 1;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            moveDirection.Ydirection = 1;
            playerController.setState(new JumpingState());
        }
        playerController.HandleInput(moveDirection);
    }

    public void HandleMoveLeft(InputAction.CallbackContext context)
    {
        print(context.phase);

        if (context.performed)
        {
            pressedDirections.Add(keys.Left);
//            playerController.HandleInput(PlayerController.MoveType.Left, pressedDirections);
            print("MoveLeft performed");
            
        }
        else if (context.started)
        {
            print("MoveLeft started");
        }
        else if (context.canceled)
        {
            pressedDirections.Remove(keys.Left);
            print("MoveLeft canceled");
//            playerController.HandleInput(PlayerController.MoveType.None, pressedDirections);
        }
    }
    public void HandleMoveRight(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            pressedDirections.Add(keys.Right);
            print("MoveRight performed");
//            playerController.HandleInput(PlayerController.MoveType.Right, pressedDirections);
        }
        else if (context.started)
        {
            print("MoveRight started");
        }
        else if (context.canceled)
        {
            pressedDirections.Remove(keys.Right);
            print("MoveRight canceled");
//            playerController.HandleInput(PlayerController.MoveType.None, pressedDirections);
        }
    }

    public void HandleJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            print("Jump performed");
            pressedDirections.Add(keys.Jump);
//            playerController.HandleInput(PlayerController.MoveType.Jump, pressedDirections);
        }
        else if (context.started)
        {
            print("Jump started");
        }
        else if (context.canceled)
        {
            pressedDirections.Remove(keys.Jump);
//            playerController.HandleInput(PlayerController.MoveType.None, pressedDirections);
        }
    }
}
