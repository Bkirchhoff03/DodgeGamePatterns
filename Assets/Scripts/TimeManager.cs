using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    private float timeInGame = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public string GetTimeInGame()
    {
        int minutes = Mathf.FloorToInt(timeInGame / 60F);
        int seconds = Mathf.FloorToInt(timeInGame - minutes * 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
    public void ResetTime()
    {
        timeInGame = 0f;
    }
    public void UpdateTime(float time)
    {
        timeInGame = time;
    }
    public void UpdateTime(string timeString)
    {
        timeInGame = GetTimeFromString(timeString);
    }
    public float GetTimeFromString(string timeString)
    {
        string[] timeParts = timeString.Split(':');
        if (timeParts.Length == 2)
        {
            int minutes = int.Parse(timeParts[0]);
            int seconds = int.Parse(timeParts[1]);
            return minutes * 60 + seconds;
        }
        return 0f; // Default to 0 if the format is incorrect
    }
    // Update is called once per frame
    void Update()
    {
        timeInGame += Time.deltaTime;
    }
}
