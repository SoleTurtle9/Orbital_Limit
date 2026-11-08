using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIStyler : MonoBehaviour
{
    public void Initialize(GameObject panel)
    {
        if (panel == null || panel.transform.Find("GameOverCard") != null) return;
        var canvas = panel.GetComponentInParent<Canvas>();
        if (canvas != null) OrbitalUI.ConfigureCanvas(canvas);
        OrbitalUI.Stretch(panel.GetComponent<RectTransform>());
        var overlay = panel.GetComponent<Image>();
        if (overlay != null) overlay.color = new Color(0.003f, 0.009f, 0.02f, 0.9f);

        var texts = panel.GetComponentsInChildren<TextMeshProUGUI>(true);
        var button = panel.GetComponentInChildren<Button>(true);
        var card = OrbitalUI.Panel("GameOverCard", panel.transform, new Vector2(640f, 490f), Vector2.zero);
        card.transform.SetAsFirstSibling();
        card.borderColor = new Color(1f, 0.72f, 0.35f, 0.4f);
        OrbitalUI.Caption("GameOverCaption", card.transform, "СИГНАЛ ПОТЕРЯН", new Vector2(520f, 30f),
            new Vector2(0f, 180f), OrbitalUI.Gold);
        OrbitalUI.Block("GameOverDivider", card.transform, new Vector2(520f, 1f),
            new Vector2(0f, 51f), new Color(0.5f, 0.7f, 0.8f, 0.25f));

        foreach (var text in texts)
        {
            if (button != null && text.transform.IsChildOf(button.transform)) continue;
            OrbitalUI.ApplyTypography(text);
            text.transform.SetParent(card.transform, false);
            bool score = text.name == "FinalScoreText";
            OrbitalUI.Place(text.rectTransform, new Vector2(550f, score ? 100f : 76f),
                new Vector2(0f, score ? -16f : 111f), new Vector2(0.5f, 0.5f));
            if (!score) text.text = "ПОЛЁТ ЗАВЕРШЁН";
            text.fontSize = score ? 30f : 43f;
            text.enableAutoSizing = true;
            text.fontSizeMin = score ? 26f : 34f;
            text.fontSizeMax = score ? 30f : 43f;
            text.fontStyle = score ? FontStyles.Normal : FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.color = OrbitalUI.Text;
            text.raycastTarget = false;
        }

        if (button != null)
        {
            button.transform.SetParent(card.transform, false);
            OrbitalUI.Place(button.GetComponent<RectTransform>(), new Vector2(520f, 72f),
                new Vector2(0f, -139f), new Vector2(0.5f, 0.5f));
            var oldImage = button.GetComponent<Image>();
            if (oldImage != null) { oldImage.enabled = false; oldImage.raycastTarget = false; }
            var graphic = OrbitalUI.Panel("RestartBackground", button.transform, Vector2.zero, Vector2.zero);
            OrbitalUI.Stretch(graphic.rectTransform);
            graphic.transform.SetAsFirstSibling();
            button.targetGraphic = graphic;
            OrbitalUI.StyleButton(button, true);
            var label = button.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
            {
                OrbitalUI.ApplyTypography(label);
                OrbitalUI.Stretch(label.rectTransform, 8f);
                label.text = "НАЧАТЬ ЗАНОВО";
                label.fontSize = 26f;
                label.alignment = TextAlignmentOptions.Center;
                label.fontStyle = FontStyles.Bold;
                label.color = new Color(0.02f, 0.08f, 0.11f);
                label.raycastTarget = false;
            }
        }
    }
}
