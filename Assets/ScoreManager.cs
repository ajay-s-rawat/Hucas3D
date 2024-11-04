using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    public int Score { get; private set; }
    private int comboStreak;  // Track the current streak of matches
    private int totalMoves;    // Track the total moves made by the player

    // Points configuration (modifiable in Inspector)
    [SerializeField] private int pointsPerMatch = 10; // Points for each match found
    [SerializeField] private int baseBonusPerStreak = 5; // Base bonus per streak

    protected override void Awake()
    {
        base.Awake();
        Score = 0;
        comboStreak = 0;
        totalMoves = 0; // Initialize total moves
    }

    // Method to reset the score and combo
    public void ResetScore()
    {
        Score = 0;
        comboStreak = 0; // Reset combo streak
        totalMoves = 0;  // Reset total moves
        Debug.Log("Scoreboard have been reset.");
    }

    // Method to add points to the score
    public void AddScore(int amount)
    {
        if (amount > 0)
        {
            Score += amount;
            Debug.Log($"Score increased by {amount}. Total Score: {Score}");
        }
        else
        {
            Debug.LogWarning("Attempted to add a non-positive score.");
        }
    }

    // Method to increase the combo streak
    public void IncreaseCombo()
    {
        comboStreak++;
        int bonusPoints = CalculateBonusPoints(comboStreak);
        AddScore(bonusPoints); // Add bonus points for the combo
        Debug.Log($"Combo streak: {comboStreak}. Bonus Points: {bonusPoints}");
    }

    // Calculate bonus points based on the streak
    private int CalculateBonusPoints(int streak)
    {
        return streak * baseBonusPerStreak; // Example: Use base bonus from Inspector
    }

    // Method to track moves
    public void IncrementMoves()
    {
        totalMoves++;
        Debug.Log($"Total Moves: {totalMoves}");
    }

    // Method to reward points for a match found
    public void RewardPointsForMatch()
    {
        AddScore(pointsPerMatch); // Add points for match found
        Debug.Log($"Points awarded for match: {pointsPerMatch}");
    }

    // Method to get the current score
    public int GetScore()
    {
        return Score;
    }

    // Method to get the current combo streak
    public int GetComboStreak()
    {
        return comboStreak;
    }

    // Method to get the total moves
    public int GetTotalMoves()
    {
        return totalMoves;
    }

    // Method to reset the combo streak
    public void ResetCombo()
    {
        comboStreak = 0; // Reset combo streak
        Debug.Log("Combo streak has been reset.");
    }
}
