using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

// Spawns bats that swoop across the level at the player. Only placed in levels that have bats.
// Bats are kept separate from FallerManager on purpose: they don't fall, freeze, stack, or get saved.
public class BatManager : MonoBehaviour
{
    public GameObject player; // Defaults to GameManager's player if left empty
    public GameObject batPrefab; // Optional; a placeholder bat is built in code if left empty
    public float startDelay = 5f; // Time before the first bat, so the player can get settled
    public float minTimeBetweenBats = 3f;
    public float maxTimeBetweenBats = 6f;
    public int maxBatsAlive = 2;
    public float batSpeed = 6f;
    public float warningTime = 0.75f; // How long the entry-side warning shows before the bat appears
    public float minHeightAbovePlayer = 2f;
    public float maxHeightAbovePlayer = 4f;

    private float spawnTimer;
    private float warningTimer = 0f;
    private bool warning = false;
    private int pendingSide;
    private float pendingStartY;
    private int numberOfBats = 0;
    private GameObject warningMarker;
    private SpriteRenderer warningRenderer;
    private float warningFlashTimer = 0f;
    private readonly List<BatController> batsInPlay = new List<BatController>();

    void Start()
    {
        if (player == null)
        {
            player = GameManager.instance().player;
        }
        spawnTimer = startDelay;
        warningMarker = new GameObject("BatWarning");
        warningMarker.transform.parent = transform;
        warningMarker.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        warningRenderer = warningMarker.AddComponent<SpriteRenderer>();
        warningRenderer.sprite = Resources.Load<Sprite>("Circle");
        warningRenderer.sortingOrder = 26;
        warningRenderer.enabled = false;
    }

    void Update()
    {
        if (player == null) return;
        if (warning)
        {
            UpdateWarning();
            return;
        }
        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f) return;

        batsInPlay.RemoveAll(b => b == null);
        if (batsInPlay.Count >= maxBatsAlive)
        {
            return; // Try again next frame once a bat has left
        }
        StartWarning();
    }

    private void StartWarning()
    {
        pendingSide = Random.value < 0.5f ? -1 : 1;
        pendingStartY = GetFlightHeight();
        warning = true;
        warningTimer = warningTime;
        warningFlashTimer = 0f;
        warningRenderer.enabled = true;
        warningRenderer.color = new Color(1f, 0f, 0f, 0.6f);
    }

    private void UpdateWarning()
    {
        warningTimer -= Time.deltaTime;
        // Follow the camera so the marker stays where the bat will enter
        pendingStartY = ClampToCamera(pendingStartY);
        warningMarker.transform.position = new Vector3(pendingSide * Constants.batWarningX, pendingStartY, 0f);
        warningFlashTimer += Time.deltaTime;
        if (warningFlashTimer >= 0.1f)
        {
            warningFlashTimer = 0f;
            warningRenderer.color = warningRenderer.color.g > 0.5f ? new Color(1f, 0f, 0f, 0.6f) : new Color(1f, 1f, 1f, 0.6f);
        }
        if (warningTimer <= 0f)
        {
            warning = false;
            warningRenderer.enabled = false;
            SpawnBat(pendingSide, pendingStartY);
            spawnTimer = Random.Range(minTimeBetweenBats, maxTimeBetweenBats);
        }
    }

    private void SpawnBat(int side, float startY)
    {
        Vector3 start = new Vector3(side * Constants.batSpawnX, startY, 0f);
        Vector3 end = new Vector3(-side * Constants.batSpawnX, GetFlightHeight(), 0f);
        Vector3 target = player.transform.position;

        numberOfBats++;
        GameObject batObject = batPrefab != null ? Instantiate(batPrefab) : CreatePlaceholderBat();
        batObject.name = Constants.batNamePrefix + numberOfBats;
        BatController bat = batObject.GetComponent<BatController>();
        bat.Init(start, target, end, batSpeed);
        batsInPlay.Add(bat);
        AudioManager.instance()?.PlayBatScreech();
        GameManager.instance().Print("Spawned " + batObject.name + " from " + start + " through " + target + " to " + end, 1);
    }

    // Stand-in until there's bat art: a dark purple oval on the Bats layer
    private GameObject CreatePlaceholderBat()
    {
        GameObject batObject = new GameObject();
        int batLayer = LayerMask.NameToLayer(Constants.batLayerName);
        if (batLayer < 0)
        {
            Debug.LogWarning("No '" + Constants.batLayerName + "' layer found; add it in Tags and Layers so bats don't block raycasts");
        }
        else
        {
            batObject.layer = batLayer;
        }
        batObject.transform.localScale = new Vector3(0.9f, 0.5f, 0.75f);
        SpriteRenderer sr = batObject.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Circle");
        sr.color = new Color(0.35f, 0.1f, 0.45f, 1f);
        sr.sortingOrder = 26;
        Rigidbody2D rb = batObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        CircleCollider2D col = batObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        batObject.AddComponent<BatController>(); // Added last so its Awake finds the Rigidbody2D and SpriteRenderer
        return batObject;
    }

    private float GetFlightHeight()
    {
        return ClampToCamera(player.transform.position.y + Random.Range(minHeightAbovePlayer, maxHeightAbovePlayer));
    }

    private float ClampToCamera(float y)
    {
        float distance = Mathf.Abs(Camera.main.transform.position.z);
        float bottom = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, distance)).y;
        float top = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, distance)).y;
        return Mathf.Clamp(y, bottom + 1f, top - 1f);
    }
}
