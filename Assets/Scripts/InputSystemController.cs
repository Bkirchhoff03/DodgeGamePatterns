using Assets.Scripts;
using System;
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
    private KeyCode Pause = KeyCode.Escape;
    private KeyCode MoveLeft = KeyCode.A;
    private KeyCode MoveRight = KeyCode.D;
    private KeyCode Jump = KeyCode.Space;
    private KeyCode EMT = KeyCode.M;

    private KeyCode PunchLeft = KeyCode.Mouse0;
    private KeyCode PunchRight = KeyCode.Mouse1;

    private KeyCode SpawnAtMouse = KeyCode.P;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        setKeyBinds();
    }
    public void setKeyBinds()
    {
        KeyCode moveLeft = ParseStringtoKeyCode(PlayerPrefs.GetString("MoveLeft_Key"));
        if (moveLeft != KeyCode.None)
        {
            MoveLeft = moveLeft;
        }

        KeyCode moveRight = ParseStringtoKeyCode(PlayerPrefs.GetString("MoveRight_Key"));
        if (moveRight != KeyCode.None)
        {
            MoveRight = moveRight;
        }

        KeyCode jump = ParseStringtoKeyCode(PlayerPrefs.GetString("Jump_Key"));
        if (jump != KeyCode.None)
        {
            Jump = jump;
        }

        KeyCode pause = ParseStringtoKeyCode(PlayerPrefs.GetString("Pause_Key"));
        if (pause != KeyCode.None)
        {
            Pause = pause;
        }

        KeyCode punchLeft = ParseStringtoKeyCode(PlayerPrefs.GetString("PunchLeft_Key"));
        if (punchLeft != KeyCode.None)
        {
            PunchLeft = punchLeft;
        }

        KeyCode punchRight = ParseStringtoKeyCode(PlayerPrefs.GetString("PunchRight_Key"));
        if (punchRight != KeyCode.None)
        {
            PunchRight = punchRight;
        }

        KeyCode spawnAtMouse = ParseStringtoKeyCode(PlayerPrefs.GetString("SpawnAtMouse_Key"));
        if (spawnAtMouse != KeyCode.None)
        {
            SpawnAtMouse = spawnAtMouse;
        }

        KeyCode emt = ParseStringtoKeyCode(PlayerPrefs.GetString("EMT_Key"));
        if (emt != KeyCode.None)
        {
            EMT = emt;
        }
    }
    private KeyCode ParseStringtoKeyCode(string keyString)
    {
        if (string.IsNullOrEmpty(keyString))
        {
            return KeyCode.None;
        }
        if (Enum.TryParse(keyString, out KeyCode myKey))
        {
            return myKey;
        }
        else 
        {
            return KeyCode.None;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(Pause) && GameManager.instance() != null) { 
            GameManager.instance().TogglePause(); 
        }
        
        if (GameManager.instance() != null && GameManager.instance().isPaused) { 
            return; 
        }
        
        PlayerController.MoveDirection moveDirection = new PlayerController.MoveDirection();
        if(Input.GetKey(MoveLeft))
        {
            moveDirection.Xdirection -= 1;
            //pressedDirections.Add(keys.Left);
            //playerController.HandleInput(PlayerController.MoveType.Left, pressedDirections);
        }
        if (Input.GetKey(MoveRight))
        {
            moveDirection.Xdirection += 1;
        }
        if (Input.GetKey(Jump))
        {
            moveDirection.Ydirection = 1;
        }
        if(Input.GetKey(PunchLeft))// || Input.GetKey(KeyCode.Q))
        {
            moveDirection.isPunch = -1;
        }
        if(Input.GetKey(PunchRight))// || Input.GetKey(KeyCode.E))
        {
            moveDirection.isPunch = 1;
        }
        if (Input.GetKey(SpawnAtMouse))
        {
            GameManager.instance().SpawnFallerAtClick(Input.mousePosition);
        }
        if (Input.GetKey(EMT))
        {
            GameManager.instance().StartEMT();
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
