using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemController : MonoBehaviour
{
    PlayerController playerController;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void HandleMoveLeft(InputAction.CallbackContext context)
    {
        print(context.phase);

        if (context.performed)
        {
            playerController.state.handleInput(playerController, PlayerController.MoveInput.Left);  
        }
        else if (context.started)
        {
            print("MoveLeft started");
        }
        else if (context.canceled)
        {
            playerController.state.handleInput(playerController, PlayerController.MoveInput.None);
        }
    }
    public void HandleMoveRight(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            playerController.state.handleInput(playerController, PlayerController.MoveInput.Right);
        }
        else if (context.started)
        {
            print("MoveRight started");
        }
        else if (context.canceled)
        {
            playerController.state.handleInput(playerController, PlayerController.MoveInput.None);
        }
    }
}
