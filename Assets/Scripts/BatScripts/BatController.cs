using Assets.Scripts;
using UnityEngine;

// A bat that swoops across the level from one side to the other.
// It moves horizontally at a steady rate and dips down to pass through the player's position
// at the moment the bat was spawned, then climbs back up as it leaves.
// Bats hurt the player on contact (see GameManager.HandlePlayerBatCollision) and die to a single punch.
public class BatController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 start;
    private Vector2 target;
    private Vector2 end;
    private float targetT = 0.5f; // Point in the flight (0-1) where the bat passes the target
    private float t = 0f;
    private float flightDuration = 1f;
    private bool isDead = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void Init(Vector3 start, Vector3 target, Vector3 end, float speed)
    {
        this.start = start;
        this.target = target;
        this.end = end;
        // x is linear in t, so the target is passed when x reaches target.x; keep it away from the ends so the dip isn't instant
        targetT = Mathf.Clamp(Mathf.InverseLerp(start.x, end.x, target.x), 0.1f, 0.9f);
        flightDuration = Mathf.Max(0.1f, ApproximatePathLength() / speed);
        transform.position = start;
        if (sr != null)
        {
            sr.flipX = end.x < start.x;
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        t += Time.fixedDeltaTime / flightDuration;
        if (t >= 1f)
        {
            DeleteMe();
            return;
        }
        rb.MovePosition(PathPoint(t));
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        PunchingArmController arm = other.GetComponent<PunchingArmController>();
        if (arm != null)
        {
            Kill(arm);
            return;
        }
        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.instance().HandlePlayerBatCollision(this);
        }
    }

    private void Kill(PunchingArmController arm)
    {
        GameManager.instance().Print("Bat " + gameObject.name + " was punched", 1);
        AudioManager.instance()?.PlayPunchConnect();
        arm.CancelPunch();
        DeleteMe();
    }

    public void DeleteMe()
    {
        isDead = true;
        Destroy(gameObject);
    }

    private Vector2 PathPoint(float time)
    {
        float x = Mathf.Lerp(start.x, end.x, time);
        float y;
        if (time < targetT)
        {
            y = Mathf.Lerp(start.y, target.y, Mathf.SmoothStep(0f, 1f, time / targetT));
        }
        else
        {
            y = Mathf.Lerp(target.y, end.y, Mathf.SmoothStep(0f, 1f, (time - targetT) / (1f - targetT)));
        }
        return new Vector2(x, y);
    }

    private float ApproximatePathLength()
    {
        const int steps = 16;
        float length = 0f;
        Vector2 previous = start;
        for (int i = 1; i <= steps; i++)
        {
            Vector2 point = PathPoint(i / (float)steps);
            length += Vector2.Distance(previous, point);
            previous = point;
        }
        return length;
    }
}
