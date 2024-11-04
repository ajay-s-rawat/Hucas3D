using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public Sprite[] cardFaces;        // Array of card face sprites
    public Sprite cardBack;           // Sprite for card back
    [SerializeField] private GridLayoutGroup gridLayoutGroup;  // Reference to GridLayoutGroup
    [SerializeField] private DynamicGridLayout dynamicGridLayout; // Reference to DynamicGridLayout
    public GameObject cardPrefab;     // Prefab for the card

    private List<CardScript> cards = new List<CardScript>();    // List to hold instantiated cards
    private CardScript firstSelectedCard;
    private CardScript secondSelectedCard;
    private int totalPairs;
    private int matchesFound = 0;

    void Start()
    {
        SetupGridAndCards();
    }

    private void SetupGridAndCards()
    {
        if (dynamicGridLayout != null)
        {
            dynamicGridLayout.UpdateGridLayout();  // Ensure grid layout is updated before card generation
        }
        //ShowPossibleCombinations(cardFaces.Length);

        // Check for minimum and maximum rows and columns
        if (!IsWithinValidRange(dynamicGridLayout.rows, dynamicGridLayout.columns))
        {
            ShowMessage("Please set rows and columns between 2 and 6.");
            return;
        }

        // Check if the total number of cards is even
        if (!IsEvenTotalCards(dynamicGridLayout.rows, dynamicGridLayout.columns))
        {
            ShowMessage("The total number of cards must be even for pairs.");
            return;
        }

        // Check if there are enough sprites for the required number of pairs
        if (!HasEnoughSprites(dynamicGridLayout.rows, dynamicGridLayout.columns))
        {
            ShowMessage("Not enough unique card faces available for the number of cards. Available: " + cardFaces.Length);
            return;
        }

        // Show possible combinations message
        int totalCards = dynamicGridLayout.rows * dynamicGridLayout.columns;
        totalPairs = totalCards / 2;
        ShowMessage($"Generating {totalPairs} pairs of cards.");

        GenerateCardLayout();
    }

    private bool IsWithinValidRange(int rows, int columns)
    {
        return rows >= 2 && rows <= 6 && columns >= 2 && columns <= 6;
    }

    private void ShowPossibleCombinations(int availableSprites)
    {
        int totalSprites = availableSprites;
        int requiredCards = totalSprites * 2;  // Total cards needed for all pairs

        string combinations = $"Possible configurations for {requiredCards} cards: ";
        HashSet<string> combinationSet = new HashSet<string>(); // To store unique combinations

        // Loop through potential row values from 2 up to half the requiredCards
        for (int rows = 2; rows <= requiredCards / 2; rows++)
        {
            // Calculate columns based on requiredCards
            if (requiredCards % rows == 0)
            {
                int columns = requiredCards / rows;

                // Ensure both rows and columns are >= 2
                if (columns >= 2)
                {
                    combinationSet.Add($"{rows}x{columns}");
                    if (rows != columns) // Add reverse only if they are different
                    {
                        combinationSet.Add($"{columns}x{rows}");
                    }
                }
            }
        }

        // Combine all unique combinations into the final string
        combinations += string.Join(", ", combinationSet);

        Debug.Log(combinations);
    }


    private bool IsEvenTotalCards(int rows, int columns)
    {
        return (rows * columns) % 2 == 0;
    }

    private bool HasEnoughSprites(int rows, int columns)
    {
        // Check if we have enough unique card faces for the required pairs
        int totalCards = rows * columns;
        return totalCards / 2 <= cardFaces.Length; // totalPairs needed
    }

    private void ShowMessage(string message)
    {
        Debug.Log(message);
    }

    public void GenerateCardLayout()
    {
        if (gridLayoutGroup == null)
        {
            Debug.LogError("GridLayoutGroup is not assigned!");
            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogError("Card prefab is not assigned!");
            return;
        }

        // Set the grid constraint count to match the number of rows
        gridLayoutGroup.constraintCount = dynamicGridLayout.rows;

        // Create shuffled card faces and their corresponding IDs
        (List<Sprite> shuffledFaces, List<int> idList) = CreateShuffledCardFaces();
        int totalCards = dynamicGridLayout.rows * dynamicGridLayout.columns;

        // Clear any existing cards in the list
        cards.Clear();

        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, gridLayoutGroup.transform);
            CardScript cardScript = cardObject.GetComponent<CardScript>();

            if (cardScript == null)
            {
                Debug.LogError("Card prefab does not have a CardScript component attached!");
                continue;
            }

            // Set up card properties and data
            cardScript.SetCardData(shuffledFaces[i], cardBack, idList[i]); // Use shuffled faces and corresponding IDs
            cardScript.OnCardSelected += OnCardClicked;  // Subscribe to the card's click event

            cards.Add(cardScript);  // Add the card to the list for future reference
        }
    }


    private (List<Sprite> shuffledFaces, List<int> idList) CreateShuffledCardFaces()
    {
        List<Sprite> faceList = new List<Sprite>();
        List<int> idList = new List<int>();
        totalPairs = (dynamicGridLayout.rows * dynamicGridLayout.columns) / 2;

        // Populate faceList with pairs and idList with IDs
        for (int i = 0; i < totalPairs; i++)
        {
            faceList.Add(cardFaces[i]); // Add each face once
            faceList.Add(cardFaces[i]); // Add each face again for a pair
            idList.Add(i); // Add ID for the first card
            idList.Add(i); // Add ID for the second card
        }

        // Shuffle the list
        for (int i = 0; i < faceList.Count; i++)
        {
            // Swap the current element with a random element
            int randomIndex = Random.Range(i, faceList.Count);
            // Swap sprites
            Sprite tempFace = faceList[i];
            faceList[i] = faceList[randomIndex];
            faceList[randomIndex] = tempFace;
            // Swap IDs accordingly
            int tempId = idList[i];
            idList[i] = idList[randomIndex];
            idList[randomIndex] = tempId;
        }

        return (faceList, idList);
    }


    private void OnCardClicked(CardScript clickedCard)
    {
        if (clickedCard.isFaceUp || secondSelectedCard != null || clickedCard == firstSelectedCard)
            return;

        clickedCard.ShowCardFace();

        if (firstSelectedCard == null)
        {
            firstSelectedCard = clickedCard;
        }
        else
        {
            secondSelectedCard = clickedCard;
            StartCoroutine(CheckForMatch());
        } 
    }


    private IEnumerator CheckForMatch()
    {
        yield return new WaitForSeconds(0.5f); // Wait briefly before checking for a match

        if (firstSelectedCard.cardID == secondSelectedCard.cardID)
        {
            // Cards match
            matchesFound++;
            Debug.Log("Match Found!");

            if (matchesFound == totalPairs)
            {
                Debug.Log("Game Over: All Matches Found!");
                // Here you might want to trigger a game over UI or similar
            }
        }
        else
        {
            // Cards do not match, flip them back over
            firstSelectedCard.ShowCardBack();
            secondSelectedCard.ShowCardBack();
        }

        // Reset selected cards
        firstSelectedCard = null;
        secondSelectedCard = null;
    }

}
