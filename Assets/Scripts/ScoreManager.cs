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
        UpdateScoreText();
    }

    public void AddPoint()
    {
        score++;
        UpdateScoreText();

        Debug.Log("Очки: " + score);
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}