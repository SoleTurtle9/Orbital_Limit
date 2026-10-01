using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const string HighScoreKey = "HighScore";

    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameOverUIStyler styler = gameObject.AddComponent<GameOverUIStyler>();
        styler.Initialize(gameOverPanel);

        gameOverPanel.SetActive(false);
    }

    public void GameOver()
    {
        if (IsGameOver)
            return;

        IsGameOver = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOver();
        }

        HUDController hud = FindFirstObjectByType<HUDController>();

        if (hud != null)
        {
            hud.gameObject.SetActive(false);
        }

        int score = ScoreManager.Instance.GetScore();
        int bestScore = Mathf.Max(PlayerPrefs.GetInt(HighScoreKey, 0), score);

        PlayerPrefs.SetInt(HighScoreKey, bestScore);
        PlayerPrefs.Save();

        finalScoreText.text = "Счёт: " + score + "\nРекорд: " + bestScore;

        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
