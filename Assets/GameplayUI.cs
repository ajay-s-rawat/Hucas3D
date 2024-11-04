using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText; // Text to display the current score
    [SerializeField] private TextMeshProUGUI comboText; // Text to display the current combo streak
    [SerializeField] private TextMeshProUGUI movesText; // Text to display the total moves
    [SerializeField] private Button restartButton; // Restart button
    [SerializeField] private Button quitButton; // Quit Button

    private void Awake()
    {
        restartButton.onClick.AddListener(OnClickRestartGame); // Attach listener for the restart button
        quitButton.onClick.AddListener(OnClickQuitButton); // Attach listener for the restart button
    }

    private void Update()
    {
        UpdateUI(); // Update the UI elements every frame
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + ScoreManager.Instance.GetScore(); // Update the score display
        comboText.text = "Combo: " + ScoreManager.Instance.GetComboStreak(); // Update the combo display
        movesText.text = "Moves: " + ScoreManager.Instance.GetTotalMoves(); // Update the moves display
    }

    private void OnClickRestartGame()
    {
        // Notify GameManager to restart the game
        GameManager.Instance.RestartGame();
    }

    private void OnClickQuitButton()
    {

    }
}
