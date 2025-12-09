using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum MoveType
    {
        Left,
        Right,
        None,
        Jump
    }
    public IPlayerState state = new DodgingState();
    delegate void MoveAction();
    MoveAction moveAction;
    float curX = 0;
    float curY = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        changeMoveFunc(0);
    // inputActions = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        state = state.Update(this);
    }
    public void HandleInput(PlayerController.MoveType moveInput)
    {
        state = state.HandleInput(this, moveInput);
    }
    public void changeMoveFunc(float num)
    {
        if(num < 0)
        {
            moveAction = MoveLeft;
        }else if(num > 0)
        {
            moveAction = MoveRight;
        }
        else
        {
            moveAction = MoveNo;
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
    public void MoveLeft()
    {
        transform.position += Vector3.left * Time.deltaTime * Constants.moveSpeed;
    }
    public void MoveRight()
    {
        transform.position += Vector3.right * Time.deltaTime * Constants.moveSpeed;
    }
    public void MoveNo()
    {
        //get the objects current position and put it in a variable so we can access it later with less code
        Vector3 pos = transform.position;
        //calculate what the new Y position will be
        float newY = Mathf.Sin(Time.time * 2.0f);
        //set the object's Y to the new calculated Y
        transform.position = new Vector3(transform.position.x, (newY * 0.25f) - 4.0f, transform.position.z);
        //gameObject.transform.GetComponent<MeshRenderer>().material.color = Color.white;
    }
}
