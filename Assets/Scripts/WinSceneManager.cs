using TMPro;
using UnityEngine;

public class WinSceneManager : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject timeLabel = GameObject.Find("TimeLabel");
        if (timeLabel != null)
        {
            timeLabel.GetComponent<TextMeshProUGUI>().text = "You Took\n" + TimeManager.Instance.GetTimeInGame().Split(":")[0] + " minutes and " + TimeManager.Instance.GetTimeInGame().Split(":")[1] + " seconds\n to win!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        TimeManager.Instance.ResetTime();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    public void ResetGame()
    {
        Time.timeScale = 1f;
        TimeManager.Instance.ResetTime();
        PlayerPrefs.SetInt("PlayerLivesFromLevel1", 3);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
    }
}
