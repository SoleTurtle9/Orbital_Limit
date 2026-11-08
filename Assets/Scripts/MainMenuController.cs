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
    private const string BackgroundResourcePath = "Menu/MainMenuBackground";
    private GameObject menuCard;
    private GameObject settingsCard;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneLoaded()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => CreateMenu(scene.name);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateMenuForInitialScene() => CreateMenu(SceneManager.GetActiveScene().name);

    private static void CreateMenu(string sceneName)
    {
        if (sceneName != MenuSceneName || GameObject.Find(RootName) != null) return;
        Time.timeScale = 1f;
        new GameObject(RootName).AddComponent<MainMenuController>().Build();
    }

    private void Build()
    {
        EnsureEventSystem();
        var canvasRoot = OrbitalUI.Rect("MainMenuCanvas", transform, Vector2.zero, Vector2.zero);
        var canvas = canvasRoot.gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasRoot.gameObject.AddComponent<GraphicRaycaster>();
        OrbitalUI.ConfigureCanvas(canvas);
        CreateBackground(canvasRoot);
        OrbitalUI.Label("Brand", canvasRoot, "O / L", 26f, OrbitalUI.Text,
            new Vector2(160f, 40f), new Vector2(64f, -54f), TextAlignmentOptions.Left,
            new Vector2(0f, 1f), new Vector2(0f, 0.5f));
        OrbitalUI.Label("Genre", canvasRoot, "КОСМИЧЕСКАЯ АРКАДА", 17f, OrbitalUI.Muted,
            new Vector2(420f, 34f), new Vector2(-64f, -54f), TextAlignmentOptions.Right,
            Vector2.one, new Vector2(1f, 0.5f));
        OrbitalUI.Label("Controls", canvasRoot, "W / S   ТЯГА     A / D   ПОВОРОТ     SPACE   ОГОНЬ", 17f,
            new Color(0.65f, 0.76f, 0.84f), new Vector2(950f, 36f), new Vector2(64f, 44f),
            TextAlignmentOptions.Left, Vector2.zero, new Vector2(0f, 0.5f));
        CreateMenuCard(canvasRoot);
        CreateSettingsCard(canvasRoot);
    }

    private void CreateBackground(Transform parent)
    {
        var image = OrbitalUI.Rect("Background", parent, Vector2.zero, Vector2.zero).gameObject.AddComponent<RawImage>();
        image.raycastTarget = false;
        OrbitalUI.Stretch(image.rectTransform);
        var texture = Resources.Load<Texture2D>(BackgroundResourcePath);
        if (texture != null)
        {
            image.texture = texture;
            var aspect = image.gameObject.AddComponent<AspectRatioFitter>();
            aspect.aspectRatio = (float)texture.width / texture.height;
            aspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        }
        else image.color = OrbitalUI.Surface;

        var shade = OrbitalUI.Panel("ReadabilityGradient", parent, Vector2.zero, Vector2.zero);
        OrbitalUI.Stretch(shade.rectTransform);
        shade.corner = shade.borderWidth = 0f;
        shade.horizontalGradient = true;
        shade.color = new Color(0.005f, 0.012f, 0.025f, 0.12f);
        shade.endColor = new Color(0.005f, 0.012f, 0.025f, 0.96f);
        var top = OrbitalUI.Panel("TopShade", parent, new Vector2(0f, 180f), Vector2.zero, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
        top.rectTransform.anchorMin = new Vector2(0f, 1f);
        top.rectTransform.anchorMax = Vector2.one;
        top.corner = top.borderWidth = 0f;
        top.color = new Color(0.005f, 0.012f, 0.025f, 0.8f);
        top.endColor = Color.clear;
        var line = OrbitalUI.Block("HeaderLine", parent, new Vector2(-128f, 1f), new Vector2(0f, -92f),
            new Color(0.5f, 0.8f, 0.9f, 0.18f), new Vector2(0.5f, 1f));
        line.rectTransform.anchorMin = new Vector2(0f, 1f);
        line.rectTransform.anchorMax = Vector2.one;
    }

    private void CreateMenuCard(Transform parent)
    {
        var rect = OrbitalUI.Rect("MenuCard", parent, new Vector2(580f, 790f),
            new Vector2(-105f, 0f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
        menuCard = rect.gameObject;
        OrbitalUI.Caption("Eyebrow", rect, "ЗА ПРЕДЕЛАМИ ОРБИТЫ", new Vector2(520f, 30f), new Vector2(0f, 303f), OrbitalUI.Cyan);
        var orbital = OrbitalUI.Label("TitleOrbital", rect, "ORBITAL", 88f, OrbitalUI.Text,
            new Vector2(540f, 110f), new Vector2(0f, 226f));
        orbital.fontStyle = FontStyles.Bold;
        orbital.characterSpacing = 3f;
        var limit = OrbitalUI.Label("TitleLimit", rect, "LIMIT", 112f, OrbitalUI.Gold,
            new Vector2(540f, 132f), new Vector2(0f, 117f));
        limit.fontStyle = FontStyles.Bold;
        limit.characterSpacing = 5f;
        OrbitalUI.Label("Subtitle", rect, "Один корабль. Бесконечный космос.", 23f, OrbitalUI.Muted,
            new Vector2(520f, 40f), new Vector2(0f, 19f));
        OrbitalUI.Button("StartButton", rect, "ИГРАТЬ", new Vector2(520f, 76f), new Vector2(0f, -74f), StartGame, true);
        OrbitalUI.Button("SettingsButton", rect, "НАСТРОЙКИ", new Vector2(520f, 66f), new Vector2(0f, -159f), ShowSettings);
        OrbitalUI.Button("ExitButton", rect, "ВЫХОД", new Vector2(520f, 66f), new Vector2(0f, -234f), QuitGame);
        OrbitalUI.Block("RecordDivider", rect, new Vector2(520f, 1f), new Vector2(0f, -296f), new Color(0.5f, 0.8f, 0.9f, 0.2f));
        OrbitalUI.Caption("RecordLabel", rect, "ЛИЧНЫЙ РЕКОРД", new Vector2(340f, 30f), new Vector2(-90f, -335f));
        OrbitalUI.Label("RecordText", rect, PlayerPrefs.GetInt("HighScore", 0).ToString("N0"), 30f, OrbitalUI.Gold,
            new Vector2(160f, 40f), new Vector2(180f, -335f), TextAlignmentOptions.MidlineRight);
    }

    private void CreateSettingsCard(Transform parent)
    {
        var panel = OrbitalUI.Panel("SettingsCard", parent, new Vector2(580f, 550f),
            new Vector2(-105f, 0f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
        settingsCard = panel.gameObject;
        OrbitalUI.Settings(panel.transform, ShowMainMenu);
        settingsCard.SetActive(false);
    }

    private void StartGame() => SceneManager.LoadScene(GameSceneName);
    private void ShowSettings() { menuCard.SetActive(false); settingsCard.SetActive(true); }
    private void ShowMainMenu() { settingsCard.SetActive(false); menuCard.SetActive(true); }
    private void QuitGame() => Application.Quit();

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }
}
