using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Shared presentation only: gameplay and menu actions remain in their controllers.
public static class OrbitalUI
{
    public static readonly Color Text = new Color(0.9f, 0.95f, 0.98f);
    public static readonly Color Muted = new Color(0.5f, 0.63f, 0.71f);
    public static readonly Color Cyan = new Color(0.35f, 0.88f, 0.96f);
    public static readonly Color Gold = new Color(1f, 0.72f, 0.35f);
    public static readonly Color Green = new Color(0.39f, 0.9f, 0.7f);
    public static readonly Color Red = new Color(1f, 0.37f, 0.39f);
    public static readonly Color Surface = new Color(0.025f, 0.055f, 0.085f, 0.96f);
    private static TMP_FontAsset uiFont;

    public static void ApplyTypography(TMP_Text text)
    {
        if (uiFont == null) uiFont = Resources.Load<TMP_FontAsset>("Fonts/OrbitalUI");
        if (uiFont == null)
        {
            Debug.LogError("OrbitalUI: UI font asset not found.");
            return;
        }

        // Use the baked family and its own material for both Latin and Cyrillic.
        text.font = uiFont;
        text.fontSharedMaterial = uiFont.material;
        text.extraPadding = true;
        text.characterSpacing = 0f;
    }

    public static void ConfigureCanvas(Canvas canvas)
    {
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null) scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
    }

    public static RectTransform Rect(string name, Transform parent, Vector2 size,
        Vector2 position, Vector2? anchor = null, Vector2? pivot = null)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        Place(rect, size, position, anchor ?? new Vector2(0.5f, 0.5f), pivot);
        return rect;
    }

    public static void Place(RectTransform rect, Vector2 size, Vector2 position,
        Vector2 anchor, Vector2? pivot = null)
    {
        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = pivot ?? new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
    }

    public static void Stretch(RectTransform rect, float inset = 0f)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.one * inset;
        rect.offsetMax = -Vector2.one * inset;
    }

    public static OrbitalPanelGraphic Panel(string name, Transform parent, Vector2 size,
        Vector2 position, Vector2? anchor = null, Vector2? pivot = null)
    {
        var rect = Rect(name, parent, size, position, anchor, pivot);
        var graphic = rect.gameObject.AddComponent<OrbitalPanelGraphic>();
        graphic.color = Surface;
        graphic.raycastTarget = false;
        return graphic;
    }

    public static Image Block(string name, Transform parent, Vector2 size,
        Vector2 position, Color color, Vector2? anchor = null, Vector2? pivot = null)
    {
        var rect = Rect(name, parent, size, position, anchor, pivot);
        var image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    public static TextMeshProUGUI Label(string name, Transform parent, string value,
        float fontSize, Color color, Vector2 size, Vector2 position,
        TextAlignmentOptions alignment = TextAlignmentOptions.MidlineLeft,
        Vector2? anchor = null, Vector2? pivot = null)
    {
        var rect = Rect(name, parent, size, position, anchor, pivot);
        var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        ApplyTypography(text);
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.raycastTarget = false;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.enableAutoSizing = true;
        text.fontSizeMin = fontSize * 0.8f;
        text.fontSizeMax = fontSize;
        return text;
    }

    public static void Caption(string name, Transform parent, string value,
        Vector2 size, Vector2 position, Color? color = null)
    {
        var text = Label(name, parent, value, 17f, color ?? Muted, size, position);
        text.characterSpacing = 1.5f;
    }

    public static Button Button(string name, Transform parent, string title,
        Vector2 size, Vector2 position, UnityAction action, bool primary = false)
    {
        var panel = Panel(name, parent, size, position);
        var button = panel.gameObject.AddComponent<Button>();
        StyleButton(button, primary);
        if (action != null) button.onClick.AddListener(action);
        var label = Label(name + "Text", panel.transform, title, 26f,
            primary ? new Color(0.02f, 0.08f, 0.11f) : Text,
            new Vector2(size.x - 92f, size.y), new Vector2(-6f, 0f));
        label.fontStyle = FontStyles.Bold;
        Label("Arrow", panel.transform, ">", 27f, primary ? Surface : Cyan,
            new Vector2(28f, size.y), new Vector2(size.x * 0.5f - 34f, 0f),
            TextAlignmentOptions.Center);
        return button;
    }

    public static void StyleButton(Button button, bool primary)
    {
        var graphic = button.targetGraphic as OrbitalPanelGraphic;
        if (graphic == null) graphic = button.GetComponent<OrbitalPanelGraphic>();
        if (graphic != null)
        {
            graphic.color = primary ? Cyan : new Color(0.035f, 0.075f, 0.11f, 0.95f);
            graphic.endColor = primary ? new Color(0.22f, 0.67f, 0.79f) : Surface;
            graphic.borderColor = primary ? new Color(0.62f, 0.95f, 1f) : new Color(0.3f, 0.57f, 0.67f, 0.45f);
            graphic.raycastTarget = true;
            button.targetGraphic = graphic;
        }
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.18f, 1.18f, 1.18f);
        colors.selectedColor = colors.highlightedColor;
        colors.pressedColor = new Color(0.72f, 0.82f, 0.88f);
        colors.disabledColor = new Color(0.4f, 0.45f, 0.5f, 0.5f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.14f;
        button.colors = colors;
    }

    public static void VolumeRow(Transform parent, Vector2 position, string title, string key)
    {
        var row = Rect(title + "Row", parent, new Vector2(456f, 88f), position);
        Label("Label", row, title, 23f, Text, new Vector2(300f, 34f), new Vector2(-78f, 24f));
        var percent = Label("Value", row, "", 22f, Cyan,
            new Vector2(100f, 34f), new Vector2(178f, 24f), TextAlignmentOptions.MidlineRight);
        var sliderRect = Rect("VolumeSlider", row, new Vector2(456f, 34f), new Vector2(0f, -18f));
        var slider = sliderRect.gameObject.AddComponent<Slider>();
        var hitArea = Block("SliderHitArea", sliderRect, new Vector2(456f, 34f), Vector2.zero, Color.clear);
        hitArea.raycastTarget = true;
        var track = Block("Track", sliderRect, new Vector2(456f, 4f), Vector2.zero,
            new Color(0.14f, 0.23f, 0.29f));
        var fillArea = Rect("Fill Area", sliderRect, new Vector2(-14f, 4f), Vector2.zero);
        fillArea.anchorMin = new Vector2(0f, 0.5f);
        fillArea.anchorMax = new Vector2(1f, 0.5f);
        var fill = Block("Fill", fillArea, Vector2.zero, Vector2.zero, Cyan);
        Stretch(fill.rectTransform);
        var handleArea = Rect("Handle Area", sliderRect, new Vector2(-14f, 26f), Vector2.zero);
        handleArea.anchorMin = new Vector2(0f, 0.5f);
        handleArea.anchorMax = new Vector2(1f, 0.5f);
        var handle = Block("Handle", handleArea, new Vector2(14f, 0f), Vector2.zero, Text);
        handle.raycastTarget = true;
        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        slider.targetGraphic = handle;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        float saved = Mathf.Clamp01(PlayerPrefs.GetFloat(key, 1f));
        slider.SetValueWithoutNotify(saved);
        percent.text = Mathf.RoundToInt(saved * 100f) + "%";
        slider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
            if (key == "MusicVolume" && AudioManager.Instance != null)
                AudioManager.Instance.SetMusicVolume(value);
            percent.text = Mathf.RoundToInt(value * 100f) + "%";
        });
    }

    public static void Settings(Transform parent, UnityAction back)
    {
        Caption("Eyebrow", parent, "АУДИО / ПАРАМЕТРЫ", new Vector2(456f, 28f), new Vector2(0f, 190f), Cyan);
        var title = Label("SettingsTitle", parent, "Настройки", 42f, Text,
            new Vector2(456f, 64f), new Vector2(0f, 133f));
        title.fontStyle = FontStyles.Bold;
        Block("Divider", parent, new Vector2(456f, 1f), new Vector2(0f, 82f), new Color(0.3f, 0.6f, 0.7f, 0.3f));
        VolumeRow(parent, new Vector2(0f, 20f), "МУЗЫКА", "MusicVolume");
        VolumeRow(parent, new Vector2(0f, -84f), "ЗВУКИ", "SfxVolume");
        Button("BackButton", parent, "НАЗАД", new Vector2(456f, 66f), new Vector2(0f, -194f), back);
    }
}
