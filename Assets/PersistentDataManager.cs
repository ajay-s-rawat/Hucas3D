using UnityEngine;
using System.Collections.Generic;

public class PersistentDataManager : Singleton<PersistentDataManager>
{
    private const string MatchesFoundKey = "MatchesFound";
    private const string TotalPairsKey = "TotalPairs";
    private const string MovesMadeKey = "MovesMade";
    private const string ScoreKey = "Score";
    private const string ComboKey = "Combo";
    private const string GameSavedKey = "GameSaved";

    [SerializeField] private CardManager cardManager;

    protected override void Awake()
    {
        base.Awake();
    }

    // Saves the current game state, including card positions and face-up states
    public void SaveGame(GameData gameData)
    {
        PlayerPrefs.SetInt(MatchesFoundKey, gameData.MatchesFound);
        PlayerPrefs.SetInt(TotalPairsKey, gameData.TotalPairs);
        PlayerPrefs.SetInt(MovesMadeKey, gameData.MovesMade);
        PlayerPrefs.SetInt(ScoreKey, gameData.Score);
        PlayerPrefs.SetInt(ComboKey, gameData.Combo);

       
        // Save card data
        for (int i = 0; i < gameData.CardStates.Length; i++)
        {
            PlayerPrefs.SetInt("Card_" + i + "_ID", gameData.CardStates[i].CardID);
            PlayerPrefs.SetInt("Card_" + i + "_IsFaceUp", gameData.CardStates[i].IsFaceUp ? 1 : 0);
            Debug.Log(i);
        }

        PlayerPrefs.SetInt(GameSavedKey, 1);  // Flag to indicate saved game data exists
        PlayerPrefs.Save();
        Debug.Log("Game Saved!");
    }

    // Loads the game state and returns relevant data
    public bool LoadGame(out GameData gameData)
    {
        if (PlayerPrefs.HasKey(GameSavedKey))
        {
            gameData = new GameData
            {
                MatchesFound = PlayerPrefs.GetInt(MatchesFoundKey, 0),
                TotalPairs = PlayerPrefs.GetInt(TotalPairsKey, 0),
                MovesMade = PlayerPrefs.GetInt(MovesMadeKey, 0),
                Score = PlayerPrefs.GetInt(ScoreKey, 0),
                Combo = PlayerPrefs.GetInt(ComboKey, 0),
                CardStates = new CardData[PlayerPrefs.GetInt(TotalPairsKey, 0) * 2]  // Initialize the card states array
            };

            // Load card data
            for (int i = 0; i < gameData.TotalPairs * 2; i++)
            {
                int cardID = PlayerPrefs.GetInt("Card_" + i + "_ID", -1);
                int isFaceUp = PlayerPrefs.GetInt("Card_" + i + "_IsFaceUp", 0);

                if (cardID != -1)
                {
                    gameData.CardStates[i] = new CardData
                    {
                        CardID = cardID,
                        IsFaceUp = isFaceUp == 1
                    };
                    Debug.Log(i);
                }
            }

            Debug.Log("Game Loaded!");
            return true;
        }

        gameData = null; // Return null if no game data exists
        return false;
    }

    // Clears saved game data
    public void ClearSavedGame()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Saved game data cleared!");
    }
}
