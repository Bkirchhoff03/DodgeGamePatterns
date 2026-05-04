using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private GameObject mainPanel;
    private GameObject howToPlayPanel;
    private GameObject loadSavePanel;
    private GameObject runAsTesterPanel;
    private GameObject saveFileListContainer;
    public bool runAsTester = false;
    private GameObject clickToEditKeybind = null;
    private float flashDuration = 0.5f;
    private Sprite[] keyboardSpriteSheet;
    private Vector4 anchors;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainPanel = GameObject.Find("MainPanel");
        howToPlayPanel = GameObject.Find("HowToPlayPanel");
        loadSavePanel = GameObject.Find("LoadSavePanel");
        runAsTesterPanel = GameObject.Find("RunAsTesterPanel");
        
        var loadSaveBtn = GameObject.Find("LoadSaveButton");
        loadSaveBtn.GetComponent<Button>().onClick.AddListener(ShowLoadSave);

        var backFromLoadBtn = GameObject.Find("BackFromLoadButton");
        backFromLoadBtn.GetComponent<Button>().onClick.AddListener(HideLoadSave);

        saveFileListContainer = GameObject.Find("SaveFileListContainer");
        saveFileListContainer = SetupScrollView(saveFileListContainer);

        var playBtn = mainPanel.transform.Find("PlayButton");
        if(runAsTester)
        {
            playBtn.GetComponent<Button>().onClick.AddListener(OpenRunAsTesterPanel);
        }
        else
        {
            playBtn.GetComponent<Button>().onClick.AddListener(PlayGame);
        }

        var howBtn = GameObject.Find("HowToPlayButton");
        howBtn.GetComponent<Button>().onClick.AddListener(ShowHowToPlay);

        var quitBtn = GameObject.Find("QuitButton");
        quitBtn.GetComponent<Button>().onClick.AddListener(QuitGame);

        var backBtn = GameObject.Find("BackFromHowToPlayButton");
        backBtn.GetComponent<Button>().onClick.AddListener(HideHowToPlay);

        var backTestBtn = runAsTesterPanel.transform.Find("CancelButton");
        if (backTestBtn != null)
        {
            backTestBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => runAsTesterPanel.SetActive(false));
        }
        
        var clickToSpawnToggle = runAsTesterPanel.transform.Find("ClickToSpawnToggle");
        if (clickToSpawnToggle != null)
        {
            clickToSpawnToggle.GetComponent<Toggle>().onValueChanged.AddListener((isOn) => {
                SetClickToSpawn(isOn);
            });
        }
        var unlimitedLivesToggle = runAsTesterPanel.transform.Find("UnlimitedLivesToggle");
        if (unlimitedLivesToggle != null)
        {
            unlimitedLivesToggle.GetComponent<Toggle>().onValueChanged.AddListener((isOn) => {
                SetUnlimitedLives(isOn);
            });
        }
        PlayerPrefs.SetInt("clickToSpawnTester", 0);
        PlayerPrefs.SetInt("unlimitedLivesTester", 0);
        howToPlayPanel.SetActive(false);
        runAsTesterPanel.SetActive(false);
        keyboardSpriteSheet = Resources.LoadAll<Sprite>("KeysAndMouseSpriteSheet");

        setKeyBinds();

    }
    public void Update()
    {
        if(clickToEditKeybind != null)
        {
            flashDuration -= Time.unscaledDeltaTime;
            if (flashDuration <= 0f)
            {
                flashDuration = 0.25f; // Reset flash duration
                
                clickToEditKeybind.GetComponent<UnityEngine.UI.Image>().color = clickToEditKeybind.GetComponent<UnityEngine.UI.Image>().color == Color.red ? Color.white : Color.red;
            }
            if (Input.anyKeyDown)
            {
                foreach(KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    if(Input.GetKeyDown(kc))
                    {
                        string keyBind = GetShortKeyName(kc);
                        clickToEditKeybind.transform.Find("KeyBindLabel").GetComponent<TMPro.TextMeshProUGUI>().text = keyBind;
                        PlayerPrefs.SetString(clickToEditKeybind.gameObject.name + "_Key", kc.ToString());
                        Debug.Log("Set " + clickToEditKeybind.gameObject.name + "_Key to " + keyBind + " length: " + keyBind.Length);

                        justifySize(clickToEditKeybind.transform, keyBind);
                        clickToEditKeybind.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                        clickToEditKeybind = null;
                        break;
                    }
                }
            }
        }
    }
    private void setKeyBinds()
    {
        Transform moveLeft = howToPlayPanel.transform.Find("MoveLeft");
        string mlkeyBind = GetShortKeyName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MoveLeft_Key", KeyCode.A.ToString())));
        moveLeft.Find("KeyBindLabel").GetComponent<TMPro.TextMeshProUGUI>().text = mlkeyBind;
        justifySize(moveLeft, mlkeyBind);

        Transform moveRight = howToPlayPanel.transform.Find("MoveRight");
        string mrkeyBind = GetShortKeyName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MoveRight_Key", KeyCode.D.ToString())));
        moveRight.Find("KeyBindLabel").GetComponent<TMPro.TextMeshProUGUI>().text = mrkeyBind;
        justifySize(moveRight, mrkeyBind);

        Transform jump = howToPlayPanel.transform.Find("Jump");
        string jumpKeyBind = GetShortKeyName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Jump_Key", KeyCode.Space.ToString())));
        jump.Find("KeyBindLabel").GetComponent<TMPro.TextMeshProUGUI>().text = jumpKeyBind;
        justifySize(jump, jumpKeyBind);

        Transform pause = howToPlayPanel.transform.Find("Pause");
        string pauseKeyBind = GetShortKeyName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pause_Key", KeyCode.Escape.ToString())));
        pause.Find("KeyBindLabel").GetComponent<TMPro.TextMeshProUGUI>().text = pauseKeyBind;
        justifySize(pause, pauseKeyBind);

        Transform emt = howToPlayPanel.transform.Find("EMT");
        string emtKeyBind = GetShortKeyName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("EMT_Key", KeyCode.M.ToString())));
        emt.Find("KeyBindLabel").GetComponent<TMPro.TextMeshProUGUI>().text = emtKeyBind;
        justifySize(emt, emtKeyBind);

    }
    private void justifySize(Transform keyTransform, string keyBind)
    {
        anchors = new Vector4(keyTransform.GetComponent<RectTransform>().anchorMin.x, keyTransform.GetComponent<RectTransform>().anchorMin.y, keyTransform.GetComponent<RectTransform>().anchorMax.x, keyTransform.GetComponent<RectTransform>().anchorMax.y);

        if (keyBind.Length > 3)
        {

            keyTransform.GetComponent<UnityEngine.UI.Image>().sprite = System.Array.Find(keyboardSpriteSheet, s => s.name == "BlankSpaceKey");
            if (anchors.z - anchors.x < 0.11f)
            {
                float middleX = (anchors.x + anchors.z) / 2f;
                float halfWidth = 0.11374f / 2f;
                keyTransform.GetComponent<RectTransform>().anchorMin = new Vector2(middleX - halfWidth, anchors.y);
                keyTransform.GetComponent<RectTransform>().anchorMax = new Vector2(middleX + halfWidth, anchors.w);
                //.11374
                Transform text = keyTransform.Find(keyTransform.name + "Text");
                Vector2 offMax = text.GetComponent<RectTransform>().offsetMax;
                Vector2 offMin = text.GetComponent<RectTransform>().offsetMin;
                text.GetComponent<RectTransform>().offsetMax = new Vector2(offMax.x - 50f, offMax.y);
                text.GetComponent<RectTransform>().offsetMin = new Vector2(offMin.x + 50f, offMin.y);
            }
        }
        else
        {
            keyTransform.GetComponent<UnityEngine.UI.Image>().sprite = System.Array.Find(keyboardSpriteSheet, s => s.name == "BlankKey");
            if (anchors.z - anchors.x > 0.055f)
            {
                float middleX = (anchors.x + anchors.z) / 2f;
                float halfWidth = 0.05f / 2f;
                keyTransform.GetComponent<RectTransform>().anchorMin = new Vector2(middleX - halfWidth, anchors.y);
                keyTransform.GetComponent<RectTransform>().anchorMax = new Vector2(middleX + halfWidth, anchors.w);
                Transform text = keyTransform.Find(keyTransform.name + "Text");
                Vector2 offMax = text.GetComponent<RectTransform>().offsetMax;
                Vector2 offMin = text.GetComponent<RectTransform>().offsetMin;
                text.GetComponent<RectTransform>().offsetMax = new Vector2(offMax.x + 50f, offMax.y);
                text.GetComponent<RectTransform>().offsetMin = new Vector2(offMin.x - 50f, offMin.y);
            }
        }
    }
    private static readonly Dictionary<KeyCode, string> KeyShortNames = new Dictionary<KeyCode, string>
    {
        { KeyCode.Alpha0, "0" },
        { KeyCode.Alpha1, "1" },
        { KeyCode.Alpha2, "2" },
        { KeyCode.Alpha3, "3" },
        { KeyCode.Alpha4, "4" },
        { KeyCode.Alpha5, "5" },
        { KeyCode.Alpha6, "6" },
        { KeyCode.Alpha7, "7" },
        { KeyCode.Alpha8, "8" },
        { KeyCode.Alpha9, "9" },
        { KeyCode.UpArrow, "UP" },
        { KeyCode.DownArrow, "DOWN" },
        { KeyCode.LeftArrow, "LEFT" },
        { KeyCode.RightArrow, "RIGHT" },
        { KeyCode.Escape, "ESC" },
        { KeyCode.Backspace, "BCK" },
        { KeyCode.Return, "RETURN" },
        { KeyCode.Space, "SPACE" },
        { KeyCode.LeftControl, "LCTRL" },
        { KeyCode.RightControl, "RCTRL" },
        { KeyCode.LeftShift, "LSHIFT" },
        { KeyCode.RightShift, "RSHIFT" }
    };

    public string GetShortKeyName(KeyCode key)
    {
        if (KeyShortNames.TryGetValue(key, out string shortName))
            return shortName;

        return key.ToString(); // Fallback to default
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
        PlayerPrefs.SetString("SessionSaveFile", "Session_" + DateTime.Now.ToString("yyyyMMddHHmm")); 
        
    }
    public void PlayLevel2() => SceneManager.LoadScene("Level2");
    public void QuitGame() => Application.Quit();
    public void ShowHowToPlay() 
    { 
        mainPanel.SetActive(false); 
        howToPlayPanel.SetActive(true);
        
    }

    public void HideHowToPlay() { howToPlayPanel.SetActive(false); mainPanel.SetActive(true); }
    public void ShowLoadSave()
    {
        mainPanel.SetActive(false);
        loadSavePanel.SetActive(true);
        PopulateSaveFileList();
    }
    public void ChangeKeybind(GameObject btn)
    {
        RectTransform rt = btn.GetComponent<RectTransform>();
        anchors = new Vector4(rt.anchorMin.x, rt.anchorMin.y, rt.anchorMax.x, rt.anchorMax.y);
        clickToEditKeybind = btn;
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

        string savePath = Assets.Scripts.Constants.saveFilePath;
        if (!System.IO.Directory.Exists(savePath))
            System.IO.Directory.CreateDirectory(savePath);
        string[] files = System.IO.Directory.GetFiles(savePath, "*.json");

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

        // Sort newest-first by actual file write time
        System.Array.Sort(files, (a, b) =>
            System.IO.File.GetLastWriteTime(b).CompareTo(System.IO.File.GetLastWriteTime(a)));

        float buttonHeight = 60f;

        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];
            string label = FormatSaveFileName(filePath);

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


            GameObject PlayButton = makeButton(btnObj, buttonHeight, "PlayButton", "Play File", Vector2.zero, new Vector2(0.33f, 1f), new Color(0.25f, 1f, 0.25f, 1f));
            GameObject DeleteButton = makeButton(btnObj, buttonHeight, "DeleteButton", "Delete File", new Vector2(0.33f, 0f), new Vector2(0.66f, 1f), new Color(1f, 0.25f, 0.25f, 1f));
            GameObject CancelPlayButton = makeButton(btnObj, buttonHeight, "CancelPlayButton", "Cancel", new Vector2(0.66f, 0f), Vector2.one, new Color(.25f, .25f, .25f, 1f));
            GameObject DeleteConfirmationButton = makeButton(btnObj, buttonHeight, "ConfirmDeleteButton", "Confirm Delete", Vector2.zero, new Vector2(0.5f, 1f), new Color(1f, 0.1f, 0.1f, 1f));
            GameObject CancelDeleteButton = makeButton(btnObj, buttonHeight, "CancelDeleteButton", "Cancel", new Vector2(0.5f, 0f), Vector2.one, new Color(.25f, .25f, .25f, 1f));
            if (runAsTester)
            {
                PlayButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OpenRunAsTesterPanelFromFile(capturedPath));
            }
            else
            {
                PlayButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => LoadFromSaveFile(capturedPath));
            }
            DeleteButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
                DeleteButton.gameObject.transform.parent.Find("ConfirmDeleteButton").gameObject.SetActive(true);
                DeleteButton.gameObject.transform.parent.Find("CancelDeleteButton").gameObject.SetActive(true);
            });
            CancelPlayButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                CancelPlayButton.gameObject.transform.parent.Find("PlayButton").gameObject.SetActive(false);
                CancelPlayButton.gameObject.transform.parent.Find("DeleteButton").gameObject.SetActive(false);
                CancelPlayButton.gameObject.SetActive(false);
            });
            DeleteConfirmationButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
                ClearSaveFile(System.IO.Path.GetFileNameWithoutExtension(capturedPath).Substring(5)); // Remove "Save_" prefix
                PopulateSaveFileList(); // Refresh the list after deletion
            });
            CancelDeleteButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                CancelDeleteButton.gameObject.transform.parent.Find("ConfirmDeleteButton").gameObject.SetActive(false);
                CancelDeleteButton.gameObject.SetActive(false);
            });
            DeleteButton.SetActive(false);
            DeleteConfirmationButton.SetActive(false);
            CancelDeleteButton.SetActive(false);
            PlayButton.SetActive(false);
            CancelPlayButton.SetActive(false);

            btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
                // Show play and delete options for this save
                btn.gameObject.transform.Find("DeleteButton").gameObject.SetActive(true);
                btn.gameObject.transform.Find("PlayButton").gameObject.SetActive(true);
                btn.gameObject.transform.Find("CancelPlayButton").gameObject.SetActive(true);
            });
        }
    }
    
    private GameObject makeButton(GameObject parent, float buttonHeight, string objName ,string label, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject delBtnObj = new GameObject(objName);
        delBtnObj.transform.SetParent(parent.transform, false);
        var delBtnRect = delBtnObj.AddComponent<RectTransform>();
        delBtnRect.anchorMin = anchorMin;
        delBtnRect.anchorMax = anchorMax;
        delBtnRect.offsetMin = delBtnRect.offsetMax = Vector2.zero;

        var img = delBtnObj.AddComponent<UnityEngine.UI.Image>();
        img.color = color;
        var btn = delBtnObj.AddComponent<UnityEngine.UI.Button>();

        var le = delBtnObj.AddComponent<LayoutElement>();
        le.preferredHeight = buttonHeight;
        le.flexibleWidth = 1f;

        var textObj = new GameObject("Label");
        textObj.transform.SetParent(delBtnObj.transform, false);
        var tmp = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 28;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        // Add confirmation dialog or similar here if desired before actually deleting
        return delBtnObj;

    }
    private GameObject SetupScrollView(GameObject container)
    {
        var scrollRect = container.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 5f;

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

    private string FormatSaveFileName(string filePath)
    {
        string name = System.IO.Path.GetFileNameWithoutExtension(filePath);
        // Strip "Save_" prefix added by the named-save system
        if (name.StartsWith("Save_"))
            name = name.Substring(5);
        string saved = System.IO.File.GetLastWriteTime(filePath).ToString("MM/dd/yyyy  HH:mm");
        return $"{name}  |  {saved}";
    }
    public void ClearSaveFile(string saveName)
    {
        string savePath = Constants.saveFilePath + "Save_" + saveName + ".json";
        string playerSavePath = Constants.playerDataSavePath + "PlayerSave_" + saveName + ".json";
        string fallerSavePath = Constants.fallerDataSavePath + "FallerSave_" + saveName + ".json";
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        if (File.Exists(playerSavePath))
        {
            File.Delete(playerSavePath);
        }
        if (File.Exists(fallerSavePath))
        {
            File.Delete(fallerSavePath);
        }


    }
    private void OpenRunAsTesterPanelFromFile(string filePath)
    {
        runAsTesterPanel.SetActive(true);
        var loadBtn = runAsTesterPanel.transform.Find("PlayButton");
        if (loadBtn != null)
        {
            loadBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => LoadFromSaveFileForTester(filePath));
        }
    }
    private void OpenRunAsTesterPanel()
    {
        runAsTesterPanel.SetActive(true);
        var loadBtn = runAsTesterPanel.transform.Find("PlayButton");
        if (loadBtn != null)
        {
            loadBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => PlayGame());
        }
    }
    public void SetClickToSpawn(bool value)
    {
        PlayerPrefs.SetInt("clickToSpawnTester", value ? 1 : 0);
    }
    public void SetUnlimitedLives(bool value)
    {
        PlayerPrefs.SetInt("unlimitedLivesTester", value ? 1 : 0);
    }
    private void LoadFromSaveFileForTester(string filePath)
    {
        LoadFromSaveFile(filePath);
    }
    private void LoadFromSaveFile(string filePath)
    {
        string scene = "Level1"; // Default scene to load; could be encoded in the save file name or contents if needed
        try
        {
            string json = System.IO.File.ReadAllText(filePath);
            FallerManager.SaveData saveData = JsonUtility.FromJson<FallerManager.SaveData>(json);
            if(!string.IsNullOrEmpty(saveData.levelScene)){
                scene = saveData.levelScene;
            }
        }
        catch
        {
            // Ignore errors and use default scene
        }
        PlayerPrefs.SetString("pendingSaveFile", filePath);
        string sessionSaveName = Path.GetFileNameWithoutExtension(filePath);
        if(sessionSaveName.StartsWith("Save_"))
            sessionSaveName = sessionSaveName.Substring(5);
        PlayerPrefs.SetString("SessionSaveFile", sessionSaveName);
        PlayerPrefs.Save();
        SceneManager.LoadScene(scene);
    }
}
