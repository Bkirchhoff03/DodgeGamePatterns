using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private GameObject mainPanel;
    private GameObject howToPlayPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainPanel = GameObject.Find("MainPanel");
        howToPlayPanel = GameObject.Find("HowToPlayPanel");
        var playBtn = GameObject.Find("PlayButton");
        playBtn.GetComponent<Button>().onClick.AddListener(PlayGame);

        var howBtn = GameObject.Find("HowToPlayButton");
        howBtn.GetComponent<Button>().onClick.AddListener(ShowHowToPlay);

        var quitBtn = GameObject.Find("QuitButton");
        quitBtn.GetComponent<Button>().onClick.AddListener(QuitGame);

        var backBtn = GameObject.Find("BackButton");
        backBtn.GetComponent<Button>().onClick.AddListener(HideHowToPlay);

        howToPlayPanel.SetActive(false);
    }
    public void PlayGame() => SceneManager.LoadScene("SampleScene");
    public void QuitGame() => Application.Quit();
    public void ShowHowToPlay() { mainPanel.SetActive(false); howToPlayPanel.SetActive(true); }
    public void HideHowToPlay() { howToPlayPanel.SetActive(false); mainPanel.SetActive(true); }
    // Update is called once per frame
    void Update()
    {
        
    }
}
