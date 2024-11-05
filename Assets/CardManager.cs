using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Sprite[] cardFaces;
    [SerializeField] private Sprite cardBack;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private DynamicGridLayout dynamicGridLayout;
    [SerializeField] private GameObject cardPrefab;

    public List<CardUI> cards = new List<CardUI>();
    public List<CardUI> flippedCards = new List<CardUI>();
  
    private int totalPairs;
    private int matchesFound = 0;

    public List<CardUI> Cards => cards;
    public int TotalPairs => totalPairs;
    public int MatchesFound => matchesFound;

   
    public void SetupGridAndCards()
    {
        ClearCardLayout();

        if (dynamicGridLayout != null)
        {
            dynamicGridLayout.UpdateGridLayout();
        }

        if (!IsWithinValidRange(dynamicGridLayout.rows, dynamicGridLayout.columns))
        {
            ShowMessage("Please set rows and columns between 2 and 6.");
            return;
        }

        if (!IsEvenTotalCards(dynamicGridLayout.rows, dynamicGridLayout.columns))
        {
            ShowMessage("The total number of cards must be even for pairs.");
            return;
        }

        if (!HasEnoughSprites(dynamicGridLayout.rows, dynamicGridLayout.columns))
        {
            ShowMessage("Not enough unique card faces available for the number of cards. Available: " + cardFaces.Length);
            return;
        }

        int totalCards = dynamicGridLayout.rows * dynamicGridLayout.columns;
        totalPairs = totalCards / 2;
        ShowMessage($"Generating {totalPairs} pairs of cards.");

        GenerateCardLayout();
    }

    private bool IsWithinValidRange(int rows, int columns)
    {
        return rows >= 2 && rows <= 6 && columns >= 2 && columns <= 6;
    }

    private bool IsEvenTotalCards(int rows, int columns)
    {
        return (rows * columns) % 2 == 0;
    }

    private bool HasEnoughSprites(int rows, int columns)
    {
        int totalCards = rows * columns;
        return totalCards / 2 <= cardFaces.Length;
    }

    private void ShowMessage(string message)
    {
        Debug.Log(message);
    }

    public void RestoreSavedCards(int totalPairs, int matchesFound, CardData[] savedCards)
    {
        ClearCardLayout();

        //Set Data using Game Data
        this.totalPairs = totalPairs;
        this.matchesFound = matchesFound;


        Debug.Log("totalPairs : " + totalPairs);
        Debug.Log("SavedCards : " + savedCards.Length);
        for(int i = 0; i < savedCards.Length ; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, gridLayoutGroup.transform);
            if (cardObject.TryGetComponent<CardUI>(out var cardScript))
            {
                // Use SetCardData to set the saved card's face, back, and ID
                cardScript.SetCardData(cardFaces[savedCards[i].CardID], cardBack, savedCards[i].CardID, savedCards[i].IsFaceUp);
                cardScript.OnCardSelected += OnCardClicked;
                
                cards.Add(cardScript);
                Debug.Log("Check if card Faceup or down!" + savedCards[i].IsFaceUp);
                // Update the UI based on whether the card is face-up or not
                if (savedCards[i].IsFaceUp)
                {
                    Debug.Log("ShowCardFace!");
                    cards[i].ShowCardFace(); // Show card face if it's face-up
                }
                else
                {
                    Debug.Log("ShowCardBack!");
                    cards[i].ShowCardBack(); // Show card back if it's face-down
                }
            }
            else
            {
                Debug.LogError("Card prefab is missing the CardUI component!");
            }
        }
        // Ensure grid layout is updated based on the saved layout settings
        if (dynamicGridLayout != null)
        {
            dynamicGridLayout.UpdateGridLayout();
        }

        gridLayoutGroup.constraintCount = dynamicGridLayout.rows;
    }

    public void GenerateCardLayout()
    {
        if (gridLayoutGroup == null || cardPrefab == null)
        {
            Debug.LogError("GridLayoutGroup or Card prefab is not assigned!");
            return;
        }

        gridLayoutGroup.constraintCount = dynamicGridLayout.rows;

        (List<Sprite> shuffledFaces, List<int> idList) = CreateShuffledCardFaces();
        int totalCards = dynamicGridLayout.rows * dynamicGridLayout.columns;

        cards.Clear();

        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, gridLayoutGroup.transform);
            
            if (!cardObject.TryGetComponent<CardUI>(out var cardScript))
            {
                Debug.LogError("Card prefab does not have a Card component attached!");
                continue;
            }

            cardScript.SetCardData(shuffledFaces[i], cardBack, idList[i], false);
            cardScript.ShowCardBack();
            cardScript.OnCardSelected += OnCardClicked;
            cards.Add(cardScript);
        }
    }


    public void ClearCardLayout()
    {
        foreach (CardUI card in cards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        cards.Clear();
    }

    private (List<Sprite> shuffledFaces, List<int> idList) CreateShuffledCardFaces()
    {
        List<Sprite> faceList = new List<Sprite>();
        List<int> idList = new List<int>();
        totalPairs = (dynamicGridLayout.rows * dynamicGridLayout.columns) / 2;

        for (int i = 0; i < totalPairs; i++)
        {
            faceList.Add(cardFaces[i]);
            faceList.Add(cardFaces[i]);
            idList.Add(i);
            idList.Add(i);
        }

        for (int i = 0; i < faceList.Count; i++)
        {
            int randomIndex = Random.Range(i, faceList.Count);
            (faceList[randomIndex], faceList[i]) = (faceList[i], faceList[randomIndex]);
            (idList[randomIndex], idList[i]) = (idList[i], idList[randomIndex]);
        }

        return (faceList, idList);
    }

    private void OnCardClicked(CardUI clickedCard)
    {
        if (clickedCard.isFaceUp || flippedCards.Count >= 2 || flippedCards.Contains(clickedCard))
            return;

        clickedCard.ShowCardFace();
        flippedCards.Add(clickedCard);

        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckForMatchCoroutine(flippedCards[0], flippedCards[1]));
            flippedCards.Clear();
        }
    }

    private IEnumerator CheckForMatchCoroutine(CardUI firstCard, CardUI secondCard)
    {
        yield return new WaitForSeconds(0.5f);

        ScoreManager.Instance.IncrementMoves();

        if (firstCard.cardID == secondCard.cardID)
        {
            matchesFound++;
            ScoreManager.Instance.RewardPointsForMatch();
            ScoreManager.Instance.IncreaseCombo();
            firstCard.isMatched = secondCard.isMatched = true;

            if (matchesFound == totalPairs)
            {
                Debug.Log("Game Over: All Matches Found!");
                GameManager.Instance.EndGame();
            }
        }
        else
        {
            ScoreManager.Instance.ResetCombo();
            yield return new WaitForSeconds(0.2f);
            firstCard.ShowCardBack();
            secondCard.ShowCardBack();
        }
    }

    public void ResetCards()
    {
        StopAllCoroutines();  // Stop any ongoing coroutines to prevent reference errors
                              // Reset match tracking and clear card objects
        matchesFound = 0;
        flippedCards.Clear();
        // Destroy existing card GameObjects
        ClearCardLayout();

        // Reinitialize the card layout
        SetupGridAndCards();
    }
}
