using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel; // The Game Over UI panel
    [SerializeField] private TextMeshProUGUI finalScoreText; // Text to display the final score

    public void ShowGameOverUI(int finalScore)
    {
        finalScoreText.text = "Final Score: " + finalScore; // Update the score display
        gameOverPanel.SetActive(true); // Show the Game Over UI
    }

    public void HideGameOverUI()
    {
        gameOverPanel.SetActive(false); // Hide the Game Over UI
    }
}
