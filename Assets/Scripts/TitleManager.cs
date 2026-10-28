using TMPro;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public GameObject ClickToContinue;
    float timeBeforeClickToContinueAppears = 2f;
    bool appearing = false;
    bool hasAppeared = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TMP_Text text = ClickToContinue.GetComponent<TMP_Text>();
        if (!appearing && !hasAppeared && Time.timeSinceLevelLoad > timeBeforeClickToContinueAppears)
        {
            appearing = true;
        }
        else if (appearing && !hasAppeared && text.color.a < 1f)
        {
            Color color = text.color;
            color.a += Time.deltaTime;
            text.color = color;
        }
        else if (appearing && !hasAppeared && text.color.a >= 1f)
        {
            appearing = false;
            hasAppeared = true;
        }
        else if (!appearing && hasAppeared && Input.anyKeyDown)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
