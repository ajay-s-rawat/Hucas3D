using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;          // UI Text to display score
    public GameObject gameOverPanel; // Game Over panel
    public Button restartButton;     // Restart button

    private int currentScore = 0;

    [SerializeField] private CardManager cardManager;

    void Start()
    {
        if(cardManager == null) cardManager = FindObjectOfType<CardManager>();
        if(gameOverPanel != null) gameOverPanel.SetActive(false);  // Hide Game Over panel at start
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);

        StartGame();                     // Initialize the game
    }

    private void StartGame()
    {
        currentScore = 0;
        UpdateScore(0);                  // Reset score
    }

    public void UpdateScore(int points)
    {
        currentScore += points;
        scoreText.text = "Score: " + currentScore;
    }

    public void OnGameOver()
    {
        gameOverPanel.SetActive(true);   // Show Game Over panel
    }

    private void RestartGame()
    {
        gameOverPanel.SetActive(false);  // Hide Game Over panel
        StartGame();
    }
}
