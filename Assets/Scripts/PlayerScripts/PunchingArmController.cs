using Assets.Scripts;
using UnityEngine;

public class PunchingArmController : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerController;
    private bool isPunchingLeft = false;
    private bool isPunchingRight = false;
    private float punchingVelocity;
    private float punchingVelocityAbsolute = 1.75f;
    private bool backend = false;
    private int backendTimer = 0;
    private int backendDuration = 13;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = gameObject.transform.parent.gameObject;
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerCenter = player.transform.position;
        if (isPunchingRight)
        {

            GameManager.instance().Print("Punching arm world position: " + transform.position + " Punching arm local position: " + transform.localPosition, 0);
            ExecuteRightPunch();
            if (transform.position.x <= playerCenter.x)
            {
                isPunchingRight = false;
                transform.position = new Vector3(playerCenter.x, playerCenter.y + Constants.PunchingArmOffsetY, playerCenter.z);
                playerController.animationManager.SetPunching(false);
                playerController.animationManager.lookRight(true);
                //playerController.PlayerAnimationGameObject.GetComponent<Animator>().SetBool("Punching", false);
            }
        }
        else if (isPunchingLeft)
        {

            GameManager.instance().Print("Punching arm world position: " + transform.position + " Punching arm local position: " + transform.localPosition, 0);
            ExecutePunchLeft();
            if (transform.position.x >= playerCenter.x)
            {
                isPunchingLeft = false;
                transform.position = new Vector3(playerCenter.x, playerCenter.y + Constants.PunchingArmOffsetY, playerCenter.z);
                playerController.animationManager.SetPunching(false);
                playerController.animationManager.lookLeft(true);
                //playerController.PlayerAnimationGameObject.GetComponent<Animator>().SetBool("Punching", false);
            }
        }
        else
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            transform.position = playerCenter;
            playerController.animationManager.SetPunching(false);
        }
        transform.localPosition = new Vector3(transform.localPosition.x, Constants.PunchingArmOffsetY, transform.localPosition.z);
    }

    public void PunchRight()
    {
        if (isPunchingRight)
        {
            return;
        }
        playerController.animationManager.lookRight(true);
        playerController.animationManager.SetPunching(true);
        GetComponent<Collider2D>().enabled = true;
        //GetComponent<SpriteRenderer>().enabled = true;
        isPunchingRight = true;
        isPunchingLeft = false;
        punchingVelocity = punchingVelocityAbsolute;
    }
    private void ExecuteRightPunch()
    {
        playerController.animationManager.lookRight(true);
        if(!backend && transform.localPosition.x >= Constants.PunchingArmOffsetX)
        {
            transform.localPosition = new Vector3(Constants.PunchingArmOffsetX, transform.localPosition.y, transform.localPosition.z);
            backend = true;
            backendTimer = 0;
            punchingVelocity = 0.0f;
            //punchingVelocity = -punchingVelocityAbsolute;
        }
        else if(backend && backendTimer < backendDuration)
        {
            backendTimer++;
        }else if(backend && backendTimer >= backendDuration)
        {
            CancelPunch();
            return;
        }
        //punchingVelocity -= Time.deltaTime * 3.0f;
        transform.localPosition += new Vector3(punchingVelocity * Time.deltaTime, 0.0f, 0.0f);
        
    }
    public void PunchLeft()
    {
        if (isPunchingLeft)
        {
            return;
        }
        playerController.animationManager.lookLeft(true);
        playerController.animationManager.SetPunching(true);
        GetComponent<Collider2D>().enabled = true;
        //GetComponent<SpriteRenderer>().enabled = true;
        isPunchingLeft = true;
        isPunchingRight = false;
        punchingVelocity = -punchingVelocityAbsolute; 
    }
    public void ExecutePunchLeft()
    {
        playerController.animationManager.lookLeft(true);
        if (!backend && transform.localPosition.x <= -Constants.PunchingArmOffsetX)
        {
            transform.localPosition = new Vector3(-Constants.PunchingArmOffsetX, transform.localPosition.y, transform.localPosition.z);
            backend = true;
            backendTimer = 0;
            punchingVelocity = 0.0f;
            //punchingVelocity = punchingVelocityAbsolute;
        }
        else if (backend && backendTimer < backendDuration)
        {
            backendTimer++;
        }
        else if (backend && backendTimer >= backendDuration)
        {
            CancelPunch();
            return;
        }
        //punchingVelocity += Time.deltaTime * 3.0f;
        transform.localPosition += new Vector3(punchingVelocity * Time.deltaTime, 0.0f, 0.0f);
    }
    public float getPunchingVelocity()
    {
        if (isPunchingLeft)
        {
            return -punchingVelocityAbsolute;
        }
        else if (isPunchingRight)
        {
            return punchingVelocityAbsolute;
        }
        return 0f;
    }
    public void CancelPunch()
    {
        isPunchingLeft = false;
        isPunchingRight = false;
        backend = false;
        GetComponent<Collider2D>().enabled = false;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + Constants.PunchingArmOffsetY, player.transform.position.z);
        playerController.animationManager.SetPunching(false);

    }
}
