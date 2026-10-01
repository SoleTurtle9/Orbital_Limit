using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private const string MenuSceneName = "Main_menu";
    private const string GameSceneName = "Game";
    private const string RootName = "RuntimeMainMenu";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const string HighScoreKey = "HighScore";
    private const string BackgroundResourcePath = "Menu/MainMenuBackground";

    private GameObject menuCard;
    private GameObject settingsCard;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneLoaded()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CreateMenu(scene.name);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateMenuForInitialScene()
    {
        CreateMenu(SceneManager.GetActiveScene().name);
    }

    private static void CreateMenu(string sceneName)
    {
        if (sceneName != MenuSceneName)
            return;

        if (GameObject.Find(RootName) != null)
            return;

        Time.timeScale = 1f;

        GameObject root = new GameObject(RootName);
        root.AddComponent<MainMenuController>().Build();
    }

    private void Build()
    {
        Camera camera = Camera.main;

        if (camera != null)
        {
            camera.backgroundColor = new Color(0.005f, 0.012f, 0.035f);
        }

        EnsureEventSystem();

        Canvas canvas = CreateCanvas();
        CreateBackground(canvas.transform);
        CreateTitle(canvas.transform);
        CreateMenuCard(canvas.transform);
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("MainMenuCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    private void CreateBackground(Transform parent)
    {
        GameObject background = CreateImage(
            "Background",
            parent,
            new Color(0.005f, 0.012f, 0.035f, 1f)
        );

        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        Image backgroundImage = background.GetComponent<Image>();
        Texture2D backgroundTexture = Resources.Load<Texture2D>(BackgroundResourcePath);

        if (backgroundTexture != null)
        {
            backgroundImage.sprite = Sprite.Create(
                backgroundTexture,
                new Rect(0f, 0f, backgroundTexture.width, backgroundTexture.height),
                new Vector2(0.5f, 0.5f)
            );
            backgroundImage.color = Color.white;
            backgroundImage.preserveAspect = false;
        }

        GameObject vignette = CreateImage(
            "Vignette",
            parent,
            new Color(0f, 0.005f, 0.02f, 0.22f)
        );

        RectTransform vignetteRect = vignette.GetComponent<RectTransform>();
        vignetteRect.anchorMin = Vector2.zero;
        vignetteRect.anchorMax = Vector2.one;
        vignetteRect.offsetMin = Vector2.zero;
        vignetteRect.offsetMax = Vector2.zero;
    }

    private void CreateStarField(Transform parent, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject star = CreateImage(
                "MenuStar",
                parent,
                new Color(0.82f, 0.92f, 1f, Random.Range(0.35f, 0.95f))
            );

            RectTransform rect = star.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(Random.value, Random.value);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            float size = Random.Range(2f, 6f);
            rect.sizeDelta = new Vector2(size, size);
        }
    }

    private void CreateTitle(Transform parent)
    {
        TextMeshProUGUI orbital = CreateText("TitleOrbital", parent);
        RectTransform orbitalRect = orbital.GetComponent<RectTransform>();
        orbitalRect.anchorMin = new Vector2(0.5f, 1f);
        orbitalRect.anchorMax = new Vector2(0.5f, 1f);
        orbitalRect.pivot = new Vector2(0.5f, 1f);
        orbitalRect.anchoredPosition = new Vector2(0f, -112f);
        orbitalRect.sizeDelta = new Vector2(1050f, 110f);

        orbital.text = "ORBITAL";
        orbital.fontSize = 92f;
        orbital.fontStyle = FontStyles.Bold;
        orbital.alignment = TextAlignmentOptions.Center;
        orbital.color = new Color(0.78f, 0.94f, 1f);
        AddShadow(orbital.gameObject, new Vector2(4f, -4f), 0.9f);

        TextMeshProUGUI limit = CreateText("TitleLimit", parent);
        RectTransform limitRect = limit.GetComponent<RectTransform>();
        limitRect.anchorMin = new Vector2(0.5f, 1f);
        limitRect.anchorMax = new Vector2(0.5f, 1f);
        limitRect.pivot = new Vector2(0.5f, 1f);
        limitRect.anchoredPosition = new Vector2(0f, -210f);
        limitRect.sizeDelta = new Vector2(820f, 98f);

        limit.text = "LIMIT";
        limit.fontSize = 86f;
        limit.fontStyle = FontStyles.Bold;
        limit.alignment = TextAlignmentOptions.Center;
        limit.color = new Color(1f, 0.28f, 0.32f);
        AddShadow(limit.gameObject, new Vector2(4f, -4f), 0.9f);
    }

    private void CreateSmallRecord(Transform parent)
    {
        TextMeshProUGUI record = CreateText("RecordText", parent);
        RectTransform recordRect = record.GetComponent<RectTransform>();
        recordRect.anchorMin = new Vector2(0.5f, 0.5f);
        recordRect.anchorMax = new Vector2(0.5f, 0.5f);
        recordRect.pivot = new Vector2(0.5f, 0.5f);
        recordRect.anchoredPosition = new Vector2(0f, -232f);
        recordRect.sizeDelta = new Vector2(480f, 44f);

        record.text = "BEST SCORE: " + PlayerPrefs.GetInt(HighScoreKey, 0);
        record.fontSize = 24f;
        record.fontStyle = FontStyles.Bold;
        record.alignment = TextAlignmentOptions.Center;
        record.color = new Color(0.62f, 0.9f, 1f, 0.9f);
        AddShadow(record.gameObject, new Vector2(2f, -2f), 0.75f);
    }

    private void CreateMenuCard(Transform parent)
    {
        GameObject card = new GameObject("MenuCard");
        card.transform.SetParent(parent, false);

        menuCard = card;

        RectTransform cardRect = card.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = new Vector2(0f, -32f);
        cardRect.sizeDelta = new Vector2(560f, 350f);

        CreateMenuButton(
            "StartButton",
            card.transform,
            new Vector2(0f, 88f),
            "ИГРАТЬ",
            StartGame
        );

        CreateMenuButton(
            "SettingsButton",
            card.transform,
            new Vector2(0f, -10f),
            "НАСТРОЙКИ",
            ShowSettings
        );

        CreateMenuButton(
            "ExitButton",
            card.transform,
            new Vector2(0f, -108f),
            "ВЫХОД",
            QuitGame
        );

        CreateSmallRecord(parent);
        CreateSettingsCard(parent);
    }

    private void CreateSettingsCard(Transform parent)
    {
        GameObject card = CreateImage(
            "SettingsCard",
            parent,
            new Color(0.01f, 0.055f, 0.105f, 0.94f)
        );

        settingsCard = card;
        settingsCard.SetActive(false);

        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = new Vector2(0f, -28f);
        cardRect.sizeDelta = new Vector2(620f, 450f);

        Outline outline = card.AddComponent<Outline>();
        outline.effectColor = new Color(0.12f, 0.85f, 1f, 0.65f);
        outline.effectDistance = new Vector2(3f, -3f);

        TextMeshProUGUI label = CreateText("SettingsLabel", card.transform);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.5f, 1f);
        labelRect.anchorMax = new Vector2(0.5f, 1f);
        labelRect.pivot = new Vector2(0.5f, 1f);
        labelRect.anchoredPosition = new Vector2(0f, -38f);
        labelRect.sizeDelta = new Vector2(500f, 52f);

        label.text = "НАСТРОЙКИ";
        label.fontSize = 34f;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = new Color(0.9f, 0.98f, 1f);

        CreateVolumeSlider(
            card.transform,
            new Vector2(0f, 82f),
            "МУЗЫКА",
            MusicVolumeKey
        );

        CreateVolumeSlider(
            card.transform,
            new Vector2(0f, -18f),
            "ЗВУКИ",
            SfxVolumeKey
        );

        CreateMenuButton(
            "BackButton",
            card.transform,
            new Vector2(0f, -138f),
            "НАЗАД",
            ShowMainMenu
        );
    }

    private void CreateVolumeSlider(
        Transform parent,
        Vector2 position,
        string labelText,
        string prefKey
    )
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
        sliderRect.anchoredPosition = new Vector2(0f, 0f);
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
        slider.wholeNumbers = false;

        GameObject background = CreateImage(
            "Background",
            sliderObject.transform,
            new Color(0.05f, 0.1f, 0.16f, 1f)
        );

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

        GameObject fill = CreateImage(
            "Fill",
            fillArea.transform,
            new Color(0.08f, 0.8f, 1f, 0.95f)
        );

        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        GameObject handle = CreateImage(
            "Handle",
            sliderObject.transform,
            new Color(0.9f, 0.99f, 1f, 1f)
        );

        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(28f, 28f);

        Outline handleOutline = handle.AddComponent<Outline>();
        handleOutline.effectColor = new Color(0.1f, 0.85f, 1f, 0.8f);
        handleOutline.effectDistance = new Vector2(2f, -2f);

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handle.GetComponent<Image>();

        return slider;
    }

    private void CreateMenuButton(
        string name,
        Transform parent,
        Vector2 position,
        string text,
        UnityEngine.Events.UnityAction action
    )
    {
        GameObject buttonObject = CreateImage(
            name,
            parent,
            new Color(0.01f, 0.08f, 0.16f, 0.82f)
        );

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(500f, 82f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.01f, 0.08f, 0.16f, 0.82f);
        colors.highlightedColor = new Color(0.02f, 0.34f, 0.62f, 0.95f);
        colors.pressedColor = new Color(0.01f, 0.16f, 0.34f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.2f, 0.24f, 0.28f, 0.55f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        Outline outline = buttonObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.8f, 1f, 1f, 0.65f);
        outline.effectDistance = new Vector2(3f, -3f);

        TextMeshProUGUI buttonText = CreateText(name + "Text", buttonObject.transform);
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        buttonText.text = text;
        buttonText.fontSize = 32f;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = new Color(0.92f, 0.99f, 1f);
        buttonText.raycastTarget = false;
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

    private void StartGame()
    {
        SceneManager.LoadScene(GameSceneName);
    }

    private void ShowSettings()
    {
        menuCard.SetActive(false);
        settingsCard.SetActive(true);
    }

    private void ShowMainMenu()
    {
        settingsCard.SetActive(false);
        menuCard.SetActive(true);
    }

    private void QuitGame()
    {
        Application.Quit();
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
