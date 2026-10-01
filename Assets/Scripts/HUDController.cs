using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    private const string HudObjectName = "RuntimeHUD";
    private const string GameSceneName = "Game";

    public static HUDController Instance { get; private set; }

    private PlayerController player;
    private AsteroidSpawner asteroidSpawner;
    private TextMeshProUGUI heartText;
    private TextMeshProUGUI healthValueText;
    private TextMeshProUGUI waveText;
    private TextMeshProUGUI notificationText;
    private CanvasGroup notificationGroup;
    private Image healthFill;
    private int displayedWave = -1;
    private Coroutine notificationRoutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneLoaded()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CreateRuntimeHUD(scene.name);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimeHUDForInitialScene()
    {
        CreateRuntimeHUD(SceneManager.GetActiveScene().name);
    }

    private static void CreateRuntimeHUD(string sceneName)
    {
        if (sceneName != GameSceneName)
            return;

        if (FindFirstObjectByType<HUDController>() != null)
            return;

        GameObject hudObject = new GameObject(HudObjectName);
        hudObject.AddComponent<HUDController>();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            Debug.LogWarning("HUDController: Canvas not found.");
            enabled = false;
            return;
        }

        player = FindFirstObjectByType<PlayerController>();
        asteroidSpawner = FindFirstObjectByType<AsteroidSpawner>();

        if (player == null)
        {
            Debug.LogWarning("HUDController: PlayerController not found.");
            enabled = false;
            return;
        }

        SetupScoreText(canvas);
        CreateWaveText(canvas.transform);
        CreateHealthPanel(canvas.transform);
        CreateNotificationPanel(canvas.transform);
        UpdateHealthUI();
        UpdateWaveUI();
    }

    private void Update()
    {
        UpdateHealthUI();
        UpdateWaveUI();
    }

    private void SetupScoreText(Canvas canvas)
    {
        GameObject scoreObject = GameObject.Find("ScoreText");

        if (scoreObject == null)
            return;

        TextMeshProUGUI scoreText = scoreObject.GetComponent<TextMeshProUGUI>();

        if (scoreText == null)
            return;

        GameObject panel = CreateHudPanel(
            "ScorePanel",
            canvas.transform,
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(-28f, -28f),
            new Vector2(420f, 86f)
        );

        TextMeshProUGUI trophy = CreatePanelIcon(
            "ScoreIcon",
            panel.transform,
            "★",
            new Color(1f, 0.73f, 0.1f),
            new Vector2(54f, 0f)
        );
        trophy.fontSize = 48f;

        RectTransform scoreRect = scoreObject.GetComponent<RectTransform>();

        scoreRect.SetParent(panel.transform, false);
        scoreRect.anchorMin = Vector2.zero;
        scoreRect.anchorMax = Vector2.one;
        scoreRect.pivot = new Vector2(0.5f, 0.5f);
        scoreRect.offsetMin = new Vector2(104f, 0f);
        scoreRect.offsetMax = new Vector2(-24f, 0f);

        scoreText.fontSize = 36f;
        scoreText.fontStyle = FontStyles.Bold;
        scoreText.alignment = TextAlignmentOptions.MidlineLeft;
        scoreText.color = new Color(0.88f, 0.95f, 1f);

        AddTextShadow(scoreObject);
    }

    private void CreateWaveText(Transform canvasTransform)
    {
        if (asteroidSpawner == null)
        {
            Debug.LogWarning("HUDController: AsteroidSpawner not found.");
            return;
        }

        GameObject panel = CreateHudPanel(
            "WavePanel",
            canvasTransform,
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(-28f, -126f),
            new Vector2(338f, 82f)
        );

        TextMeshProUGUI icon = CreatePanelIcon(
            "WaveIcon",
            panel.transform,
            "▲",
            new Color(0.12f, 0.85f, 1f),
            new Vector2(54f, 0f)
        );
        icon.fontSize = 48f;

        GameObject waveObject = new GameObject("WaveText");
        waveObject.transform.SetParent(panel.transform, false);

        RectTransform waveRect = waveObject.AddComponent<RectTransform>();
        waveRect.anchorMin = Vector2.zero;
        waveRect.anchorMax = Vector2.one;
        waveRect.pivot = new Vector2(0.5f, 0.5f);
        waveRect.offsetMin = new Vector2(108f, 0f);
        waveRect.offsetMax = new Vector2(-24f, 0f);

        waveText = waveObject.AddComponent<TextMeshProUGUI>();
        waveText.fontSize = 34f;
        waveText.fontStyle = FontStyles.Bold;
        waveText.alignment = TextAlignmentOptions.MidlineLeft;
        waveText.color = new Color(0.55f, 0.9f, 1f);
        waveText.raycastTarget = false;

        AddTextShadow(waveObject);
    }

    private void CreateHealthPanel(Transform canvasTransform)
    {
        GameObject panel = CreateHudPanel(
            "HealthPanel",
            canvasTransform,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(28f, -28f),
            new Vector2(690f, 132f)
        );

        heartText = CreateHeartText(panel.transform);
        healthValueText = CreateHealthValueText(panel.transform);
        healthFill = CreateHealthBar(panel.transform);
    }

    private TextMeshProUGUI CreateHeartText(Transform parent)
    {
        GameObject textObject = new GameObject("HealthHeart");
        textObject.transform.SetParent(parent, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0.5f);
        textRect.anchorMax = new Vector2(0f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(66f, 0f);
        textRect.sizeDelta = new Vector2(100f, 100f);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = "♥";
        text.fontSize = 70f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.3f, 1f, 0.58f);
        text.raycastTarget = false;

        AddTextShadow(textObject);

        return text;
    }

    private TextMeshProUGUI CreateHealthValueText(Transform parent)
    {
        GameObject textObject = new GameObject("HealthValueText");
        textObject.transform.SetParent(parent, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 1f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.pivot = new Vector2(0.5f, 1f);
        textRect.anchoredPosition = new Vector2(80f, -18f);
        textRect.sizeDelta = new Vector2(-190f, 42f);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = 30f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Left;
        text.color = new Color(0.9f, 0.98f, 1f);
        text.raycastTarget = false;

        AddTextShadow(textObject);

        return text;
    }

    private Image CreateHealthBar(Transform parent)
    {
        GameObject barBackground = new GameObject("HealthBarBackground");
        barBackground.transform.SetParent(parent, false);

        RectTransform backgroundRect = barBackground.AddComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0f, 0.5f);
        backgroundRect.anchorMax = new Vector2(1f, 0.5f);
        backgroundRect.pivot = new Vector2(0.5f, 0f);
        backgroundRect.anchoredPosition = new Vector2(82f, -38f);
        backgroundRect.sizeDelta = new Vector2(-192f, 32f);

        Image background = barBackground.AddComponent<Image>();
        background.color = new Color(0.02f, 0.08f, 0.13f, 0.98f);

        Outline backgroundOutline = barBackground.AddComponent<Outline>();
        backgroundOutline.effectColor = new Color(0.12f, 0.9f, 1f, 0.75f);
        backgroundOutline.effectDistance = new Vector2(2f, -2f);

        GameObject fillObject = new GameObject("HealthBarFill");
        fillObject.transform.SetParent(barBackground.transform, false);

        RectTransform fillRect = fillObject.AddComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.pivot = new Vector2(0f, 0.5f);
        fillRect.anchoredPosition = Vector2.zero;
        fillRect.sizeDelta = Vector2.zero;

        Image fill = fillObject.AddComponent<Image>();
        fill.color = new Color(0.28f, 0.9f, 0.5f);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        fill.fillAmount = 1f;

        return fill;
    }

    private void CreateNotificationPanel(Transform canvasTransform)
    {
        GameObject panel = new GameObject("BonusNotification");
        panel.transform.SetParent(canvasTransform, false);

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0f, -42f);
        panelRect.sizeDelta = new Vector2(620f, 54f);

        notificationGroup = panel.AddComponent<CanvasGroup>();
        notificationGroup.alpha = 0f;
        notificationGroup.blocksRaycasts = false;
        notificationGroup.interactable = false;

        Image background = panel.AddComponent<Image>();
        background.color = new Color(0.02f, 0.09f, 0.14f, 0.86f);
        background.raycastTarget = false;

        GameObject textObject = new GameObject("NotificationText");
        textObject.transform.SetParent(panel.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(-24f, 0f);

        notificationText = textObject.AddComponent<TextMeshProUGUI>();
        notificationText.fontSize = 30f;
        notificationText.fontStyle = FontStyles.Bold;
        notificationText.alignment = TextAlignmentOptions.Center;
        notificationText.color = new Color(0.83f, 1f, 0.96f);
        notificationText.raycastTarget = false;

        AddTextShadow(textObject);
    }

    public void ShowBonusNotification(string message)
    {
        if (notificationText == null || notificationGroup == null)
            return;

        if (notificationRoutine != null)
        {
            StopCoroutine(notificationRoutine);
        }

        notificationRoutine = StartCoroutine(ShowNotificationRoutine(message));
    }

    private IEnumerator ShowNotificationRoutine(string message)
    {
        notificationText.text = message;

        yield return FadeNotification(0f, 1f, 0.12f);
        yield return new WaitForSeconds(1.05f);
        yield return FadeNotification(1f, 0f, 0.28f);

        notificationRoutine = null;
    }

    private IEnumerator FadeNotification(float from, float to, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);
            notificationGroup.alpha = Mathf.Lerp(from, to, progress);
            yield return null;
        }

        notificationGroup.alpha = to;
    }

    private void UpdateHealthUI()
    {
        if (player == null || healthValueText == null || healthFill == null)
            return;

        int currentHealth = player.CurrentHealth;
        int maxHealth = player.MaxHealth;
        float healthPercent = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;

        healthValueText.text = "Здоровье: " + currentHealth + " / " + maxHealth;
        healthFill.fillAmount = healthPercent;
        healthFill.color = GetHealthColor(healthPercent);

        if (heartText != null)
        {
            heartText.color = GetHealthColor(healthPercent);
        }
    }

    private void UpdateWaveUI()
    {
        if (asteroidSpawner == null || waveText == null)
            return;

        int currentWave = asteroidSpawner.CurrentWave;

        if (currentWave == displayedWave)
            return;

        displayedWave = currentWave;
        waveText.text = "ВОЛНА  " + currentWave;
    }

    private Color GetHealthColor(float healthPercent)
    {
        if (healthPercent > 0.5f)
            return new Color(0.25f, 1f, 0.55f);

        if (healthPercent > 0.25f)
            return new Color(1f, 0.78f, 0.25f);

        return new Color(1f, 0.25f, 0.25f);
    }

    private void AddTextShadow(GameObject textObject)
    {
        Shadow shadow = textObject.GetComponent<Shadow>();

        if (shadow == null)
        {
            shadow = textObject.AddComponent<Shadow>();
        }

        shadow.effectColor = new Color(0f, 0f, 0f, 0.75f);
        shadow.effectDistance = new Vector2(3f, -3f);
    }

    private GameObject CreateHudPanel(
        string name,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition,
        Vector2 size
    )
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        Image background = panel.AddComponent<Image>();
        background.color = new Color(0.01f, 0.055f, 0.105f, 0.88f);
        background.raycastTarget = false;

        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.18f, 0.82f, 1f, 0.72f);
        outline.effectDistance = new Vector2(3f, -3f);

        return panel;
    }

    private TextMeshProUGUI CreatePanelIcon(
        string name,
        Transform parent,
        string value,
        Color color,
        Vector2 position
    )
    {
        GameObject iconObject = new GameObject(name);
        iconObject.transform.SetParent(parent, false);

        RectTransform rect = iconObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(80f, 70f);

        TextMeshProUGUI icon = iconObject.AddComponent<TextMeshProUGUI>();
        icon.text = value;
        icon.fontStyle = FontStyles.Bold;
        icon.alignment = TextAlignmentOptions.Center;
        icon.color = color;
        icon.raycastTarget = false;

        AddTextShadow(iconObject);

        return icon;
    }
}
