using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private GameObject mainPanel;
    private GameObject howToPlayPanel;
    private GameObject loadSavePanel;
    private GameObject saveFileListContainer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainPanel = GameObject.Find("MainPanel");
        howToPlayPanel = GameObject.Find("HowToPlayPanel");
        loadSavePanel = GameObject.Find("LoadSavePanel");
        
        var loadSaveBtn = GameObject.Find("LoadSaveButton");
        loadSaveBtn.GetComponent<Button>().onClick.AddListener(ShowLoadSave);

        var backFromLoadBtn = GameObject.Find("BackFromLoadButton");
        backFromLoadBtn.GetComponent<Button>().onClick.AddListener(HideLoadSave);

        saveFileListContainer = GameObject.Find("SaveFileListContainer");
        saveFileListContainer = SetupScrollView(saveFileListContainer);

        var playBtn = GameObject.Find("PlayButton");
        playBtn.GetComponent<Button>().onClick.AddListener(PlayGame);

        var howBtn = GameObject.Find("HowToPlayButton");
        howBtn.GetComponent<Button>().onClick.AddListener(ShowHowToPlay);

        var quitBtn = GameObject.Find("QuitButton");
        quitBtn.GetComponent<Button>().onClick.AddListener(QuitGame);

        var backBtn = GameObject.Find("BackFromHowToPlayButton");
        backBtn.GetComponent<Button>().onClick.AddListener(HideHowToPlay);

        howToPlayPanel.SetActive(false);
    }
    public void PlayGame() => SceneManager.LoadScene("SampleScene");
    public void QuitGame() => Application.Quit();
    public void ShowHowToPlay() { mainPanel.SetActive(false); howToPlayPanel.SetActive(true); }
    public void HideHowToPlay() { howToPlayPanel.SetActive(false); mainPanel.SetActive(true); }
    public void ShowLoadSave()
    {
        mainPanel.SetActive(false);
        loadSavePanel.SetActive(true);
        PopulateSaveFileList();
    }

    public void HideLoadSave()
    {
        loadSavePanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    private void PopulateSaveFileList()
    {
        // Clear any buttons from a previous showing
        foreach (Transform child in saveFileListContainer.transform)
            Destroy(child.gameObject);

        string[] files = System.IO.Directory.GetFiles(
            Assets.Scripts.Constants.saveFilePath, "*.json");

        if (files.Length == 0)
        {
            // Show a "no saves found" label
            var noSavesObj = new GameObject("NoSavesText");
            noSavesObj.transform.SetParent(saveFileListContainer.transform, false);
            var tmp = noSavesObj.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = "No save files found.";
            tmp.fontSize = 28;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            return;
        }

        // Sort newest-first (filenames contain timestamp so lexicographic = chronological)
        System.Array.Sort(files);
        System.Array.Reverse(files);

        float buttonHeight = 60f;

        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];
            string label = FormatSaveFileName(System.IO.Path.GetFileName(filePath));

            var btnObj = new GameObject("SaveBtn_" + i);
            btnObj.transform.SetParent(saveFileListContainer.transform, false);

            var img = btnObj.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(0.25f, 0.25f, 0.25f, 1f);

            var btn = btnObj.AddComponent<UnityEngine.UI.Button>();

            var le = btnObj.AddComponent<LayoutElement>();
            le.preferredHeight = buttonHeight;
            le.flexibleWidth = 1f;

            var textObj = new GameObject("Label");
            textObj.transform.SetParent(btnObj.transform, false);
            var tmp = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 28;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;

            // Capture path for closure
            string capturedPath = filePath;
            btn.onClick.AddListener(() => LoadFromSaveFile(capturedPath));
        }
    }

    private GameObject SetupScrollView(GameObject container)
    {
        var scrollRect = container.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 20f;

        // Viewport — clips the content so overflow is hidden
        var viewport = new GameObject("Viewport", typeof(RectTransform));
        viewport.transform.SetParent(container.transform, false);
        viewport.AddComponent<RectMask2D>();
        var viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = viewportRect.offsetMax = Vector2.zero;

        // Content — grows to fit all buttons; VerticalLayoutGroup stacks them
        var content = new GameObject("Content", typeof(RectTransform));
        content.transform.SetParent(viewport.transform, false);
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.offsetMin = contentRect.offsetMax = Vector2.zero;

        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 10f;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(5, 5, 5, 5);

        var csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;

        return content;
    }

    private string FormatSaveFileName(string filename)
    {
        // Input: "Save202603051500.json"  →  Output: "Save: 05/03/2026 15:00"
        try
        {
            // Strip prefix "-Save" and extension ".json"
            string stamp = filename.Replace("Save", "").Replace(".json", "");
            // stamp = "202603051500"
            int year = int.Parse(stamp.Substring(0, 4));
            int month = int.Parse(stamp.Substring(4, 2));
            int day = int.Parse(stamp.Substring(6, 2));
            int hour = int.Parse(stamp.Substring(8, 2));
            int min = int.Parse(stamp.Substring(10, 2));
            return $"Save: {day:D2}/{month:D2}/{year}  {hour:D2}:{min:D2}";
        }
        catch
        {
            return filename; // Fall back to raw name if parsing fails
        }
    }

    private void LoadFromSaveFile(string filePath)
    {
        PlayerPrefs.SetString("pendingSaveFile", filePath);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }
}
