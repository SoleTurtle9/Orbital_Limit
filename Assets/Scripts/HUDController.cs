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
    private TextMeshProUGUI healthValueText;
    private TextMeshProUGUI waveText;
    private TextMeshProUGUI notificationText;
    private CanvasGroup notificationGroup;
    private Image healthFill;
    private Image healthIcon;
    private int displayedWave = -1;
    private Coroutine notificationRoutine;
    private RectTransform hudRoot;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneLoaded()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => CreateRuntimeHUD(scene.name);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimeHUDForInitialScene() => CreateRuntimeHUD(SceneManager.GetActiveScene().name);

    private static void CreateRuntimeHUD(string sceneName)
    {
        if (sceneName != GameSceneName || FindFirstObjectByType<HUDController>() != null) return;
        new GameObject(HudObjectName).AddComponent<HUDController>();
    }

    private void Awake() => Instance = this;

    private void Start()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        player = FindFirstObjectByType<PlayerController>();
        asteroidSpawner = FindFirstObjectByType<AsteroidSpawner>();
        if (canvas == null || player == null)
        {
            Debug.LogWarning("HUDController: Canvas or PlayerController not found.");
            enabled = false;
            return;
        }

        OrbitalUI.ConfigureCanvas(canvas);
        hudRoot = OrbitalUI.Rect("GameplayHUD", canvas.transform, Vector2.zero, Vector2.zero);
        OrbitalUI.Stretch(hudRoot);
        CreateHealthPanel(hudRoot);
        SetupScoreText(hudRoot);
        CreateWaveText(hudRoot);
        CreateNotificationPanel(hudRoot);
        OrbitalUI.Label("ControlsHint", hudRoot,
            "W / S  ТЯГА     A / D  ПОВОРОТ     SPACE  ОГОНЬ     ESC  ПАУЗА",
            17f, new Color(0.66f, 0.78f, 0.84f, 0.7f), new Vector2(940f, 32f),
            new Vector2(0f, 24f), TextAlignmentOptions.Center, new Vector2(0.5f, 0f));
        UpdateHealthUI();
        UpdateWaveUI();
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) Hide();
    }

    public void Hide()
    {
        StopAllCoroutines();
        notificationRoutine = null;
        if (hudRoot != null) hudRoot.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateHealthUI();
        UpdateWaveUI();
    }

    private void CreateHealthPanel(Transform parent)
    {
        var panel = OrbitalUI.Panel("HealthPanel", parent, new Vector2(358f, 102f),
            new Vector2(36f, -30f), new Vector2(0f, 1f), new Vector2(0f, 1f));
        OrbitalUI.Caption("HealthLabel", panel.transform, "КОРПУС", new Vector2(170f, 26f), new Vector2(-36f, 24f));
        healthValueText = OrbitalUI.Label("HealthValueText", panel.transform, "", 30f,
            OrbitalUI.Text, new Vector2(170f, 42f), new Vector2(68f, 20f), TextAlignmentOptions.MidlineRight);
        healthValueText.fontStyle = FontStyles.Bold;

        healthIcon = OrbitalUI.Block("HealthCross", panel.transform, new Vector2(16f, 5f),
            new Vector2(-146f, 24f), OrbitalUI.Green);
        OrbitalUI.Block("CrossVertical", healthIcon.transform, new Vector2(5f, 16f), Vector2.zero, OrbitalUI.Green);
        var track = OrbitalUI.Block("HealthBarBackground", panel.transform, new Vector2(306f, 8f),
            new Vector2(0f, -22f), new Color(0.12f, 0.2f, 0.25f));
        healthFill = OrbitalUI.Block("HealthBarFill", track.transform, Vector2.zero, Vector2.zero, OrbitalUI.Green);
        OrbitalUI.Stretch(healthFill.rectTransform);
        healthFill.rectTransform.pivot = new Vector2(0f, 0.5f);
        for (int i = 1; i < 10; i++)
            OrbitalUI.Block("HealthTick", track.transform, new Vector2(2f, 8f),
                new Vector2(i * 30.6f - 153f, 0f), OrbitalUI.Surface);
    }

    private void SetupScoreText(Transform parent)
    {
        var scoreObject = GameObject.Find("ScoreText");
        if (scoreObject == null) return;
        var scoreText = scoreObject.GetComponent<TextMeshProUGUI>();
        if (scoreText == null) return;
        BuildScorePanel(parent, scoreText);
    }

    private void BuildScorePanel(Transform parent, TextMeshProUGUI scoreText)
    {
        OrbitalUI.ApplyTypography(scoreText);
        var panel = OrbitalUI.Panel("ScorePanel", parent, new Vector2(294f, 102f),
            new Vector2(-36f, -30f), Vector2.one, Vector2.one);
        var starRect = OrbitalUI.Rect("ScoreIcon", panel.transform, new Vector2(29f, 29f), new Vector2(-110f, 0f));
        var star = starRect.gameObject.AddComponent<ScoreIconGraphic>();
        star.color = OrbitalUI.Gold;
        star.raycastTarget = false;
        var rect = scoreText.rectTransform;
        rect.SetParent(panel.transform, false);
        OrbitalUI.Place(rect, new Vector2(210f, 62f), new Vector2(30f, 0f), new Vector2(0.5f, 0.5f));
        scoreText.fontSize = 31f;
        scoreText.enableAutoSizing = true;
        scoreText.fontSizeMin = 22f;
        scoreText.fontSizeMax = 31f;
        scoreText.fontStyle = FontStyles.Bold;
        scoreText.alignment = TextAlignmentOptions.MidlineLeft;
        scoreText.color = OrbitalUI.Text;
        scoreText.raycastTarget = false;
    }

    private void CreateWaveText(Transform parent)
    {
        if (asteroidSpawner == null) return;
        var panel = OrbitalUI.Panel("WavePanel", parent, new Vector2(224f, 72f),
            new Vector2(0f, -30f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
        for (int i = 0; i < 3; i++)
            OrbitalUI.Block("WaveSignal", panel.transform, new Vector2(4f, 9f + i * 5f),
                new Vector2(-82f + i * 9f, -5f + i * 2.5f), OrbitalUI.Cyan);
        waveText = OrbitalUI.Label("WaveText", panel.transform, "", 25f, OrbitalUI.Text,
            new Vector2(145f, 48f), new Vector2(28f, 0f));
        waveText.fontStyle = FontStyles.Bold;
    }

    private void CreateNotificationPanel(Transform parent)
    {
        var panel = OrbitalUI.Panel("BonusNotification", parent, new Vector2(560f, 56f),
            new Vector2(0f, -150f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
        panel.borderColor = new Color(0.39f, 0.9f, 0.7f, 0.45f);
        notificationGroup = panel.gameObject.AddComponent<CanvasGroup>();
        notificationGroup.alpha = 0f;
        notificationGroup.blocksRaycasts = false;
        notificationGroup.interactable = false;
        OrbitalUI.Block("BonusAccent", panel.transform, new Vector2(3f, 26f), new Vector2(-258f, 0f), OrbitalUI.Green);
        notificationText = OrbitalUI.Label("NotificationText", panel.transform, "", 23f,
            OrbitalUI.Text, new Vector2(500f, 48f), new Vector2(6f, 0f), TextAlignmentOptions.Center);
    }

    public void ShowBonusNotification(string message)
    {
        if (!isActiveAndEnabled || notificationText == null || notificationGroup == null) return;
        if (notificationRoutine != null) StopCoroutine(notificationRoutine);
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
            notificationGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(timer / duration));
            yield return null;
        }
        notificationGroup.alpha = to;
    }

    private void UpdateHealthUI()
    {
        if (player == null || healthValueText == null || healthFill == null) return;
        float percent = player.MaxHealth > 0 ? (float)player.CurrentHealth / player.MaxHealth : 0f;
        healthValueText.text = player.CurrentHealth + " / " + player.MaxHealth;
        healthFill.rectTransform.anchorMax = new Vector2(percent, 1f);
        Color color = percent > 0.5f ? OrbitalUI.Green : percent > 0.25f ? OrbitalUI.Gold : OrbitalUI.Red;
        healthFill.color = color;
        healthIcon.color = color;
        healthIcon.transform.GetChild(0).GetComponent<Image>().color = color;
    }

    private void UpdateWaveUI()
    {
        if (asteroidSpawner == null || waveText == null || displayedWave == asteroidSpawner.CurrentWave) return;
        displayedWave = asteroidSpawner.CurrentWave;
        waveText.text = "ВОЛНА  " + displayedWave;
    }
}
