using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public Sprite[] cardFaces;
    public Sprite cardBack;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private DynamicGridLayout dynamicGridLayout;
    public GameObject cardPrefab;

    private List<CardUI> cards = new List<CardUI>();
    private CardUI firstSelectedCard;
    private CardUI secondSelectedCard;
    private int totalPairs;
    private int matchesFound = 0;

    void Start()
    {
        SetupGridAndCards();
    }

    private void SetupGridAndCards()
    {
        ClearCardLayout();  // Clear existing cards before setting up new ones

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
            CardUI cardScript = cardObject.GetComponent<CardUI>();

            if (cardScript == null)
            {
                Debug.LogError("Card prefab does not have a Card component attached!");
                continue;
            }

            cardScript.SetCardData(shuffledFaces[i], cardBack, idList[i]);
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
            Sprite tempFace = faceList[i];
            faceList[i] = faceList[randomIndex];
            faceList[randomIndex] = tempFace;

            int tempId = idList[i];
            idList[i] = idList[randomIndex];
            idList[randomIndex] = tempId;
        }

        return (faceList, idList);
    }

    private void OnCardClicked(CardUI clickedCard)
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
        yield return new WaitForSeconds(0.5f);

        ScoreManager.Instance.IncrementMoves();

        if (firstSelectedCard.cardID == secondSelectedCard.cardID)
        {
            matchesFound++;
            ScoreManager.Instance.RewardPointsForMatch();
            ScoreManager.Instance.IncreaseCombo();

            if (matchesFound == totalPairs)
            {
                Debug.Log("Game Over: All Matches Found!");
                GameManager.Instance.EndGame();
            }

            firstSelectedCard = null;
            secondSelectedCard = null;
        }
        else
        {
            ScoreManager.Instance.ResetCombo();
            firstSelectedCard.ShowCardBack();
            secondSelectedCard.ShowCardBack();

            firstSelectedCard = null;
            secondSelectedCard = null;
        }
    }

    public void ResetCards()
    {
        // Reset match tracking and clear card objects
        matchesFound = 0;
        firstSelectedCard = null;
        secondSelectedCard = null;

        // Destroy existing card GameObjects
        ClearCardLayout();
       
        // Reinitialize the card layout
        SetupGridAndCards();
    }


}
