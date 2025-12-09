using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemController : MonoBehaviour
{
    PlayerController playerController;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }
    private void Update()
    {
        
    }

    public void HandleMoveLeft(InputAction.CallbackContext context)
    {
        print(context.phase);

        if (context.performed)
        {
            playerController.HandleInput(PlayerController.MoveType.Left);
            print("MoveLeft performed");
        }
        else if (context.started)
        {
            print("MoveLeft started");
        }
        else if (context.canceled)
        {
            print("MoveLeft canceled");
            playerController.HandleInput(PlayerController.MoveType.None);
        }
    }
    public void HandleMoveRight(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            print("MoveRight performed");
            playerController.HandleInput(PlayerController.MoveType.Right);
        }
        else if (context.started)
        {
            print("MoveRight started");
        }
        else if (context.canceled)
        {
            print("MoveRight canceled");
            playerController.HandleInput(PlayerController.MoveType.None);
        }
    }

    public void HandleJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerController.HandleInput(PlayerController.MoveType.Jump);
        }
        else if (context.started)
        {
            print("Jump started");
        }
        else if (context.canceled)
        {
            playerController.HandleInput(PlayerController.MoveType.None);
        }
    }
}
