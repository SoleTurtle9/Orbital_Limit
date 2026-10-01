using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIStyler : MonoBehaviour
{
    private GameObject gameOverPanel;

    public void Initialize(GameObject panel)
    {
        gameOverPanel = panel;

        if (gameOverPanel == null)
            return;

        StylePanel();
        StyleTexts();
        StyleRestartButton();
    }

    private void StylePanel()
    {
        if (gameOverPanel.transform.Find("GameOverCard") != null)
            return;

        RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();

        if (panelRect != null)
        {
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = Vector2.zero;
        }

        Image overlay = gameOverPanel.GetComponent<Image>();

        if (overlay != null)
        {
            overlay.color = new Color(0.005f, 0.012f, 0.03f, 0.78f);
        }

        GameObject card = new GameObject("GameOverCard");
        card.transform.SetParent(gameOverPanel.transform, false);
        card.transform.SetAsFirstSibling();

        RectTransform cardRect = card.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = new Vector2(0f, -8f);
        cardRect.sizeDelta = new Vector2(600f, 360f);

        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(0.01f, 0.055f, 0.105f, 0.96f);
        cardImage.raycastTarget = false;

        Outline cardOutline = card.AddComponent<Outline>();
        cardOutline.effectColor = new Color(0.12f, 0.85f, 1f, 0.65f);
        cardOutline.effectDistance = new Vector2(3f, -3f);
    }

    private void StyleTexts()
    {
        TextMeshProUGUI[] texts = gameOverPanel.GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI text in texts)
        {
            if (text.gameObject.name == "FinalScoreText")
            {
                RectTransform rect = text.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0f, -4f);
                rect.sizeDelta = new Vector2(520f, 96f);

                text.fontSize = 34f;
                text.fontStyle = FontStyles.Bold;
                text.alignment = TextAlignmentOptions.Center;
                text.color = new Color(0.88f, 0.96f, 1f);
                AddShadow(text.gameObject, new Vector2(2f, -2f), 0.7f);

                continue;
            }

            if (text.text.Contains("Игра"))
            {
                RectTransform rect = text.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0f, 102f);
                rect.sizeDelta = new Vector2(560f, 86f);

                text.text = "ИГРА ОКОНЧЕНА";
                text.fontSize = 54f;
                text.fontStyle = FontStyles.Bold;
                text.alignment = TextAlignmentOptions.Center;
                text.color = new Color(1f, 0.22f, 0.32f);
                AddShadow(text.gameObject, new Vector2(3f, -3f), 0.85f);
            }
        }
    }

    private void StyleRestartButton()
    {
        Button button = gameOverPanel.GetComponentInChildren<Button>(true);

        if (button == null)
            return;

        RectTransform buttonRect = button.GetComponent<RectTransform>();
        buttonRect.anchoredPosition = new Vector2(0f, -112f);
        buttonRect.sizeDelta = new Vector2(360f, 72f);

        CreateButtonGlow(button.transform);

        Image buttonImage = button.GetComponent<Image>();

        if (buttonImage != null)
        {
            buttonImage.color = new Color(0.02f, 0.62f, 0.9f, 0.96f);
        }

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.02f, 0.62f, 0.9f, 0.96f);
        colors.highlightedColor = new Color(0.18f, 0.88f, 1f, 1f);
        colors.pressedColor = new Color(0.01f, 0.35f, 0.62f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.2f, 0.24f, 0.28f, 0.55f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        Outline outline = button.gameObject.GetComponent<Outline>();

        if (outline == null)
        {
            outline = button.gameObject.AddComponent<Outline>();
        }

        outline.effectColor = new Color(0.8f, 1f, 1f, 0.7f);
        outline.effectDistance = new Vector2(2f, -2f);

        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>(true);

        if (buttonText == null)
            return;

        buttonText.text = "НАЧАТЬ ЗАНОВО";
        buttonText.fontSize = 27f;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = new Color(0.92f, 0.99f, 1f);
        buttonText.raycastTarget = false;
    }

    private void CreateButtonGlow(Transform buttonTransform)
    {
        if (buttonTransform.Find("ButtonGlow") != null)
            return;

        GameObject glow = new GameObject("ButtonGlow");
        glow.transform.SetParent(buttonTransform, false);
        glow.transform.SetAsFirstSibling();

        RectTransform glowRect = glow.AddComponent<RectTransform>();
        glowRect.anchorMin = new Vector2(0.5f, 0.5f);
        glowRect.anchorMax = new Vector2(0.5f, 0.5f);
        glowRect.pivot = new Vector2(0.5f, 0.5f);
        glowRect.anchoredPosition = Vector2.zero;
        glowRect.sizeDelta = new Vector2(382f, 86f);

        Image glowImage = glow.AddComponent<Image>();
        glowImage.color = new Color(0.04f, 0.55f, 0.78f, 0.16f);
        glowImage.raycastTarget = false;
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
