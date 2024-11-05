using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public GameObject gameOverPanel; // Game Over panel

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

        if (PersistentDataManager.Instance.LoadGame(out GameData gameData))
        {
            LoadGameIfExist(gameData);
        }
        else
        {
            cardManager.SetupGridAndCards();
        }

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
        cardManager.ResetCards();
        StartGame();
    }

    private void OnApplicationQuit()
    {
        CardData[] cardDataList = new CardData[cardManager.TotalPairs * 2];
        Debug.Log(" TotalPairs : " + cardManager.TotalPairs);
        for(int i = 0; i < cardManager.TotalPairs * 2; i++)
        {
            cardDataList[i] = new CardData
            {
                CardID = cardManager.Cards[i].cardID,
                IsFaceUp = cardManager.Cards[i].isMatched && cardManager.Cards[i].isFaceUp
            };
        }
        Debug.Log(" cardDataList : " + cardDataList.Length);
        GameData gameData = new()
        {
            MatchesFound = cardManager.MatchesFound,
            TotalPairs = cardManager.TotalPairs,
            MovesMade = ScoreManager.Instance.GetTotalMoves(),
            Score = ScoreManager.Instance.Score,
            Combo = ScoreManager.Instance.GetComboStreak(),
            CardStates = cardDataList,
        };
        PersistentDataManager.Instance.SaveGame(gameData);
    }

    //Load existing game if any.
    private void LoadGameIfExist(GameData gameData)
    {
        // Load game data from PersistentDataManager
       
            cardManager.RestoreSavedCards(gameData.TotalPairs, gameData.MatchesFound, gameData.CardStates);
            ScoreManager.Instance.SetData(gameData.Score, gameData.MovesMade, gameData.Combo);
          
            Debug.Log("Game Loaded Successfully!");
             
    }

}
