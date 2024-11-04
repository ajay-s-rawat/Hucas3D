using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public GameObject gameOverPanel; // Game Over panel

    private int currentScore = 0;

    [SerializeField] private CardManager cardManager;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private GameplayUI gameplayUI;

    protected override void Awake()
    {
        base.Awake(); // Call the base Awake to handle the singleton logic
    }

    void Start()
    {
        if (cardManager == null) cardManager = FindObjectOfType<CardManager>();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);  // Hide Game Over panel at start

        StartGame();                     // Initialize the game
    }

    public void StartGame()
    {
        // Initialize game settings, reset scores, etc.
        ScoreManager.Instance.ResetScore();
        gameplayUI.gameObject.SetActive(true); // Show gameplay UI
    }

    public void EndGame()
    {
        gameOverUI.gameObject.SetActive(true); // Hide gameplay UI
        gameOverUI.ShowGameOverUI(ScoreManager.Instance.GetScore()); // Show Game Over UI
    }

    public void RestartGame()
    {
        gameOverPanel.SetActive(false);  // Hide Game Over panel
        StartGame();
    }
}
