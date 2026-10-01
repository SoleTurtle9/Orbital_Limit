using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    private const string GameSceneName = "Game";
    private const string MainMenuSceneName = "Main_menu";
    private const string RootName = "RuntimePauseMenu";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";

    private GameObject rootPanel;
    private GameObject pauseCard;
    private GameObject settingsCard;
    private bool isPaused;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneLoaded()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != GameSceneName)
            return;

        if (GameObject.Find(RootName) != null)
            return;

        GameObject root = new GameObject(RootName);
        root.AddComponent<PauseMenuController>().Build();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void Build()
    {
        EnsureEventSystem();

        Canvas canvas = FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("PauseCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        CreateRootPanel(canvas.transform);
        CreatePauseCard(rootPanel.transform);
        CreateSettingsCard(rootPanel.transform);

        rootPanel.SetActive(false);
    }

    private void CreateRootPanel(Transform parent)
    {
        rootPanel = CreateImage("PauseOverlay", parent, new Color(0.005f, 0.012f, 0.03f, 0.78f));
        rootPanel.transform.SetAsLastSibling();

        RectTransform rect = rootPanel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void CreatePauseCard(Transform parent)
    {
        pauseCard = CreateCard("PauseCard", parent, new Vector2(540f, 430f));

        TextMeshProUGUI title = CreateText("PauseTitle", pauseCard.transform);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -42f);
        titleRect.sizeDelta = new Vector2(460f, 64f);

        title.text = "ПАУЗА";
        title.fontSize = 46f;
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.86f, 1f, 1f);
        AddShadow(title.gameObject, new Vector2(3f, -3f), 0.8f);

        CreateButton("ResumeButton", pauseCard.transform, new Vector2(0f, 74f), "ПРОДОЛЖИТЬ", ResumeGame);
        CreateButton("SettingsButton", pauseCard.transform, new Vector2(0f, -20f), "НАСТРОЙКИ", ShowSettings);
        CreateButton("MenuButton", pauseCard.transform, new Vector2(0f, -114f), "ВЫЙТИ В МЕНЮ", ExitToMainMenu);
    }

    private void CreateSettingsCard(Transform parent)
    {
        settingsCard = CreateCard("PauseSettingsCard", parent, new Vector2(620f, 450f));
        settingsCard.SetActive(false);

        TextMeshProUGUI title = CreateText("PauseSettingsTitle", settingsCard.transform);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -38f);
        titleRect.sizeDelta = new Vector2(500f, 58f);

        title.text = "НАСТРОЙКИ";
        title.fontSize = 36f;
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.9f, 0.98f, 1f);

        CreateVolumeSlider(settingsCard.transform, new Vector2(0f, 82f), "МУЗЫКА", MusicVolumeKey);
        CreateVolumeSlider(settingsCard.transform, new Vector2(0f, -18f), "ЗВУКИ", SfxVolumeKey);
        CreateButton("BackButton", settingsCard.transform, new Vector2(0f, -138f), "НАЗАД", ShowPauseMenu);
    }

    private GameObject CreateCard(string name, Transform parent, Vector2 size)
    {
        GameObject card = CreateImage(name, parent, new Color(0.01f, 0.055f, 0.105f, 0.95f));

        RectTransform rect = card.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;

        Outline outline = card.AddComponent<Outline>();
        outline.effectColor = new Color(0.12f, 0.85f, 1f, 0.65f);
        outline.effectDistance = new Vector2(3f, -3f);

        return card;
    }

    private void CreateButton(string name, Transform parent, Vector2 position, string text, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateImage(name, parent, new Color(0.02f, 0.62f, 0.9f, 0.96f));

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(360f, 72f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.02f, 0.62f, 0.9f, 0.96f);
        colors.highlightedColor = new Color(0.18f, 0.88f, 1f, 1f);
        colors.pressedColor = new Color(0.01f, 0.35f, 0.62f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.2f, 0.24f, 0.28f, 0.55f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        Outline outline = buttonObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.8f, 1f, 1f, 0.65f);
        outline.effectDistance = new Vector2(2f, -2f);

        TextMeshProUGUI buttonText = CreateText(name + "Text", buttonObject.transform);
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        buttonText.text = text;
        buttonText.fontSize = 27f;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = new Color(0.92f, 0.99f, 1f);
        buttonText.raycastTarget = false;
    }

    private void CreateVolumeSlider(Transform parent, Vector2 position, string labelText, string prefKey)
    {
        GameObject row = new GameObject(labelText + "Row");
        row.transform.SetParent(parent, false);

        RectTransform rowRect = row.AddComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(0.5f, 0.5f);
        rowRect.anchorMax = new Vector2(0.5f, 0.5f);
        rowRect.pivot = new Vector2(0.5f, 0.5f);
        rowRect.anchoredPosition = position;
        rowRect.sizeDelta = new Vector2(500f, 76f);

        TextMeshProUGUI label = CreateText(labelText + "Label", row.transform);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 0.5f);
        labelRect.anchorMax = new Vector2(0f, 0.5f);
        labelRect.pivot = new Vector2(0f, 0.5f);
        labelRect.anchoredPosition = new Vector2(0f, 18f);
        labelRect.sizeDelta = new Vector2(170f, 42f);

        label.text = labelText;
        label.fontSize = 26f;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Left;
        label.color = new Color(0.86f, 0.96f, 1f);

        TextMeshProUGUI valueText = CreateText(labelText + "Value", row.transform);
        RectTransform valueRect = valueText.GetComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(1f, 0.5f);
        valueRect.anchorMax = new Vector2(1f, 0.5f);
        valueRect.pivot = new Vector2(1f, 0.5f);
        valueRect.anchoredPosition = new Vector2(0f, 18f);
        valueRect.sizeDelta = new Vector2(90f, 42f);

        valueText.fontSize = 24f;
        valueText.fontStyle = FontStyles.Bold;
        valueText.alignment = TextAlignmentOptions.Right;
        valueText.color = new Color(0.5f, 0.92f, 1f);

        Slider slider = CreateSlider(row.transform);
        RectTransform sliderRect = slider.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0f, 0f);
        sliderRect.anchorMax = new Vector2(1f, 0f);
        sliderRect.pivot = new Vector2(0.5f, 0f);
        sliderRect.anchoredPosition = Vector2.zero;
        sliderRect.sizeDelta = new Vector2(0f, 28f);

        float savedValue = PlayerPrefs.GetFloat(prefKey, 1f);
        slider.value = savedValue;
        valueText.text = Mathf.RoundToInt(savedValue * 100f) + "%";

        slider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat(prefKey, value);
            PlayerPrefs.Save();
            valueText.text = Mathf.RoundToInt(value * 100f) + "%";
        });
    }

    private Slider CreateSlider(Transform parent)
    {
        GameObject sliderObject = new GameObject("VolumeSlider");
        sliderObject.transform.SetParent(parent, false);

        RectTransform sliderRect = sliderObject.AddComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(400f, 28f);

        Slider slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;

        GameObject background = CreateImage("Background", sliderObject.transform, new Color(0.05f, 0.1f, 0.16f, 1f));
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0f, 0.5f);
        backgroundRect.anchorMax = new Vector2(1f, 0.5f);
        backgroundRect.pivot = new Vector2(0.5f, 0.5f);
        backgroundRect.anchoredPosition = Vector2.zero;
        backgroundRect.sizeDelta = new Vector2(0f, 12f);

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObject.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f, 0.5f);
        fillAreaRect.anchorMax = new Vector2(1f, 0.5f);
        fillAreaRect.pivot = new Vector2(0.5f, 0.5f);
        fillAreaRect.anchoredPosition = Vector2.zero;
        fillAreaRect.sizeDelta = new Vector2(-22f, 12f);

        GameObject fill = CreateImage("Fill", fillArea.transform, new Color(0.08f, 0.8f, 1f, 0.95f));
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        GameObject handle = CreateImage("Handle", sliderObject.transform, new Color(0.9f, 0.99f, 1f, 1f));
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(28f, 28f);

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handle.GetComponent<Image>();

        return slider;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400f, 80f);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.raycastTarget = false;

        return text;
    }

    private GameObject CreateImage(string name, Transform parent, Color color)
    {
        GameObject imageObject = new GameObject(name);
        imageObject.transform.SetParent(parent, false);

        RectTransform rect = imageObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100f, 100f);

        Image image = imageObject.AddComponent<Image>();
        image.color = color;

        return imageObject;
    }

    private void PauseGame()
    {
        isPaused = true;
        rootPanel.SetActive(true);
        pauseCard.SetActive(true);
        settingsCard.SetActive(false);
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        isPaused = false;
        rootPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ShowSettings()
    {
        pauseCard.SetActive(false);
        settingsCard.SetActive(true);
    }

    private void ShowPauseMenu()
    {
        settingsCard.SetActive(false);
        pauseCard.SetActive(true);
    }

    private void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuSceneName);
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    private void AddShadow(GameObject target, Vector2 distance, float alpha)
    {
        Shadow shadow = target.GetComponent<Shadow>();

        if (shadow == null)
        {
            shadow = target.AddComponent<Shadow>();
        }

        shadow.effectColor = new Color(0f, 0f, 0f, alpha);
        shadow.effectDistance = distance;
    }
}
