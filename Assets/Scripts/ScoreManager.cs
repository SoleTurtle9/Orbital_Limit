using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;

    private int score = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResolveScoreText();
        UpdateScoreText();
    }

    public void AddPoint()
    {
        AddScore(1);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();

        Debug.Log("Очки: " + score);
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreText()
    {
        if (scoreText == null)
        {
            ResolveScoreText();
        }

        if (scoreText == null)
            return;

        scoreText.text = "СЧЁТ: " + score;
    }

    private void ResolveScoreText()
    {
        GameObject scoreObject = GameObject.Find("ScoreText");

        if (scoreObject == null)
            return;

        scoreText = scoreObject.GetComponent<TextMeshProUGUI>();
    }
}
