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
        if (scene.name != GameSceneName || GameObject.Find(RootName) != null) return;
        new GameObject(RootName).AddComponent<PauseMenuController>().Build();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    private void Build()
    {
        EnsureEventSystem();
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            var canvasObject = new GameObject("PauseCanvas", typeof(Canvas), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        OrbitalUI.ConfigureCanvas(canvas);
        var overlay = OrbitalUI.Block("PauseOverlay", canvas.transform, Vector2.zero, Vector2.zero,
            new Color(0.003f, 0.009f, 0.02f, 0.88f));
        rootPanel = overlay.gameObject;
        OrbitalUI.Stretch(overlay.rectTransform);
        overlay.raycastTarget = true;
        rootPanel.transform.SetAsLastSibling();
        CreatePauseCard(rootPanel.transform);
        CreateSettingsCard(rootPanel.transform);
        rootPanel.SetActive(false);
    }

    private void CreatePauseCard(Transform parent)
    {
        var panel = OrbitalUI.Panel("PauseCard", parent, new Vector2(580f, 584f), Vector2.zero);
        pauseCard = panel.gameObject;
        OrbitalUI.Caption("PauseCaption", panel.transform, "ORBITAL LIMIT / ПОЛЁТ", new Vector2(456f, 30f),
            new Vector2(0f, 226f), OrbitalUI.Cyan);
        var title = OrbitalUI.Label("PauseTitle", panel.transform, "Пауза", 54f, OrbitalUI.Text,
            new Vector2(456f, 80f), new Vector2(0f, 161f));
        title.fontStyle = FontStyles.Bold;
        OrbitalUI.Label("PauseSubtitle", panel.transform, "Космос подождёт.", 22f, OrbitalUI.Muted,
            new Vector2(456f, 36f), new Vector2(0f, 101f));
        OrbitalUI.Button("ResumeButton", panel.transform, "ПРОДОЛЖИТЬ", new Vector2(456f, 72f),
            new Vector2(0f, 16f), ResumeGame, true);
        OrbitalUI.Button("SettingsButton", panel.transform, "НАСТРОЙКИ", new Vector2(456f, 66f),
            new Vector2(0f, -68f), ShowSettings);
        OrbitalUI.Button("MenuButton", panel.transform, "В ГЛАВНОЕ МЕНЮ", new Vector2(456f, 66f),
            new Vector2(0f, -146f), ExitToMainMenu);
        OrbitalUI.Label("PauseHint", panel.transform, "ESC  /  ПРОДОЛЖИТЬ ПОЛЁТ", 16f, OrbitalUI.Muted,
            new Vector2(456f, 30f), new Vector2(0f, -234f), TextAlignmentOptions.Center);
    }

    private void CreateSettingsCard(Transform parent)
    {
        var panel = OrbitalUI.Panel("PauseSettingsCard", parent, new Vector2(580f, 550f), Vector2.zero);
        settingsCard = panel.gameObject;
        OrbitalUI.Settings(panel.transform, ShowPauseMenu);
        settingsCard.SetActive(false);
    }

    private void PauseGame()
    {
        isPaused = true;
        rootPanel.SetActive(true);
        rootPanel.transform.SetAsLastSibling();
        pauseCard.SetActive(true);
        settingsCard.SetActive(false);
        Time.timeScale = 0f;
        if (AudioManager.Instance != null) AudioManager.Instance.PauseMusic();
    }

    private void ResumeGame()
    {
        isPaused = false;
        rootPanel.SetActive(false);
        Time.timeScale = 1f;
        if (AudioManager.Instance != null) AudioManager.Instance.ResumeMusic();
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
        if (FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }
}
