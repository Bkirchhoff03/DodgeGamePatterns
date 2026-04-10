using UnityEngine;

public class PunchingArmController : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerController;
    private bool isPunchingLeft = false;
    private bool isPunchingRight = false;
    private float punchingVelocity;
    private float punchingVelocityAbsolute = 2.5f;
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
            ExecuteRightPunch();
            if (transform.position.x <= playerCenter.x)
            {
                isPunchingRight = false;
                transform.position = playerCenter;
                playerController.animationManager.SetPunching(false);
                playerController.animationManager.lookRight(true);
                //playerController.PlayerAnimationGameObject.GetComponent<Animator>().SetBool("Punching", false);
            }
        }
        else if (isPunchingLeft)
        {
            ExecutePunchLeft();
            if (transform.position.x >= playerCenter.x)
            {
                isPunchingLeft = false;
                transform.position = playerCenter;
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
        transform.position = new Vector3(transform.position.x, playerCenter.y + 0.5f, playerCenter.z);
    }

    public void PunchRight()
    {
        if (isPunchingRight)
        {
            return;
        }
        playerController.animationManager.lookLeft(true);
        playerController.animationManager.SetPunching(true);
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        isPunchingRight = true;
        isPunchingLeft = false;
        punchingVelocity = punchingVelocityAbsolute;
    }
    private void ExecuteRightPunch()
    {
        playerController.animationManager.lookLeft(true);
        if(transform.position.x >= player.transform.position.x + 0.5f)
        {
            transform.position = new Vector3(player.transform.position.x + 0.5f, transform.position.y, transform.position.z);
            punchingVelocity = -punchingVelocityAbsolute;
        }
        //punchingVelocity -= Time.deltaTime * 3.0f;
        transform.localPosition += new Vector3(punchingVelocity * Time.deltaTime, 0.0f);
    }
    public void PunchLeft()
    {
        if (isPunchingLeft)
        {
            return;
        }
        playerController.animationManager.lookRight(true);
        playerController.animationManager.SetPunching(true);
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        isPunchingLeft = true;
        isPunchingRight = false;
        punchingVelocity = -punchingVelocityAbsolute;
    }
    public void ExecutePunchLeft()
    {
        playerController.animationManager.lookRight(true);
        if (transform.position.x <= player.transform.position.x - 0.5f)
        {
            transform.position = new Vector3(player.transform.position.x - 0.5f, transform.position.y, transform.position.z);
            punchingVelocity = punchingVelocityAbsolute;
        }
        //punchingVelocity += Time.deltaTime * 3.0f;
        transform.localPosition += new Vector3(punchingVelocity * Time.deltaTime, 0.0f);
    }
    public float getPunchingVelocity()
    {
        if (isPunchingLeft)
        {
            if (punchingVelocity < 0)
            {
                return punchingVelocity;
            }
            else
            {
                return 0f;
            }
        }
        else if (isPunchingRight)
        {
            if (punchingVelocity > 0)
            {
                return punchingVelocity;
            }
            else
            {
                return 0f;
            }
        }
        return 0f;
    }
    public void CancelPunch()
    {
        isPunchingLeft = false;
        isPunchingRight = false;
        GetComponent<Collider2D>().enabled = false;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z);
        playerController.PlayerAnimationGameObject.GetComponent<Animator>().SetBool("Punching", false);

    }
}
