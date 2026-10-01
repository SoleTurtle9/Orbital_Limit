using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    private const string HudObjectName = "RuntimeHUD";

    private PlayerController player;
    private AsteroidSpawner asteroidSpawner;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI waveText;
    private Image healthFill;
    private int displayedWave = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimeHUD()
    {
        if (FindFirstObjectByType<HUDController>() != null)
            return;

        GameObject hudObject = new GameObject(HudObjectName);
        hudObject.AddComponent<HUDController>();
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

        RectTransform scoreRect = scoreObject.GetComponent<RectTransform>();
        TextMeshProUGUI scoreText = scoreObject.GetComponent<TextMeshProUGUI>();

        if (scoreRect == null || scoreText == null)
            return;

        scoreRect.SetParent(canvas.transform, false);
        scoreRect.anchorMin = new Vector2(1f, 1f);
        scoreRect.anchorMax = new Vector2(1f, 1f);
        scoreRect.pivot = new Vector2(1f, 1f);
        scoreRect.anchoredPosition = new Vector2(-56f, -44f);
        scoreRect.sizeDelta = new Vector2(500f, 92f);

        scoreText.fontSize = 64f;
        scoreText.fontStyle = FontStyles.Bold;
        scoreText.alignment = TextAlignmentOptions.Right;
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

        GameObject waveObject = new GameObject("WaveText");
        waveObject.transform.SetParent(canvasTransform, false);

        RectTransform waveRect = waveObject.AddComponent<RectTransform>();
        waveRect.anchorMin = new Vector2(1f, 1f);
        waveRect.anchorMax = new Vector2(1f, 1f);
        waveRect.pivot = new Vector2(1f, 1f);
        waveRect.anchoredPosition = new Vector2(-56f, -126f);
        waveRect.sizeDelta = new Vector2(500f, 58f);

        waveText = waveObject.AddComponent<TextMeshProUGUI>();
        waveText.fontSize = 40f;
        waveText.fontStyle = FontStyles.Bold;
        waveText.alignment = TextAlignmentOptions.Right;
        waveText.color = new Color(0.45f, 0.82f, 1f);
        waveText.raycastTarget = false;

        AddTextShadow(waveObject);
    }

    private void CreateHealthPanel(Transform canvasTransform)
    {
        GameObject panel = new GameObject("HealthPanel");
        panel.transform.SetParent(canvasTransform, false);

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.anchoredPosition = new Vector2(56f, -44f);
        panelRect.sizeDelta = new Vector2(620f, 150f);

        Image panelBackground = panel.AddComponent<Image>();
        panelBackground.color = new Color(0.01f, 0.04f, 0.09f, 0.88f);

        healthText = CreateText(panel.transform);
        healthFill = CreateHealthBar(panel.transform);
    }

    private TextMeshProUGUI CreateText(Transform parent)
    {
        GameObject textObject = new GameObject("HealthText");
        textObject.transform.SetParent(parent, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 1f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.pivot = new Vector2(0.5f, 1f);
        textRect.anchoredPosition = new Vector2(0f, -18f);
        textRect.sizeDelta = new Vector2(-48f, 58f);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = 48f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Left;
        text.color = new Color(0.88f, 0.95f, 1f);
        text.raycastTarget = false;

        AddTextShadow(textObject);

        return text;
    }

    private Image CreateHealthBar(Transform parent)
    {
        GameObject barBackground = new GameObject("HealthBarBackground");
        barBackground.transform.SetParent(parent, false);

        RectTransform backgroundRect = barBackground.AddComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0f, 0f);
        backgroundRect.anchorMax = new Vector2(1f, 0f);
        backgroundRect.pivot = new Vector2(0.5f, 0f);
        backgroundRect.anchoredPosition = new Vector2(0f, 26f);
        backgroundRect.sizeDelta = new Vector2(-48f, 42f);

        Image background = barBackground.AddComponent<Image>();
        background.color = new Color(0.12f, 0.16f, 0.22f, 0.95f);

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

    private void UpdateHealthUI()
    {
        if (player == null || healthText == null || healthFill == null)
            return;

        int currentHealth = player.CurrentHealth;
        int maxHealth = player.MaxHealth;
        float healthPercent = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;

        healthText.text = "HP " + currentHealth + " / " + maxHealth;
        healthFill.fillAmount = healthPercent;
        healthFill.color = GetHealthColor(healthPercent);
    }

    private void UpdateWaveUI()
    {
        if (asteroidSpawner == null || waveText == null)
            return;

        int currentWave = asteroidSpawner.CurrentWave;

        if (currentWave == displayedWave)
            return;

        displayedWave = currentWave;
        waveText.text = "WAVE " + currentWave;
    }

    private Color GetHealthColor(float healthPercent)
    {
        if (healthPercent > 0.5f)
            return new Color(0.28f, 0.9f, 0.5f);

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
}
