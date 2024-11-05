using UnityEngine;
using UnityEngine.UI;
using System;

public class CardUI : MonoBehaviour
{
    // Unique ID for matching pairs
    public int cardID;
    // Track if the card is face up
    public bool isFaceUp = false;
    // Event to notify selection
    public event Action<CardUI> OnCardSelected;

    public bool isMatched;

    private Image cardImage;
    private Button button;
    // Face image for the card
    [SerializeField] private Sprite cardFace;
    // Back image for the card
    [SerializeField] private Sprite cardBack;

    private void Awake()
    {
        // Cache references to UI components
        cardImage = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(NotifyCardManager);
    }

    private void Start()
    {
       // ShowCardBack();
    }

    // Notify the CardManager when the card is clicked
    private void NotifyCardManager()
    {
        Debug.Log("Card selected: " + cardID);
        OnCardSelected?.Invoke(this);
    }

    // Set data for the card
    public void SetCardData(Sprite faceSprite, Sprite backSprite, int id, bool isFaceUp)
    {
        cardFace = faceSprite;
        cardBack = backSprite;
        cardID = id;
        this.isFaceUp = isFaceUp;
    }

    // Flip the card to show face or back
    public void FlipCard(bool show)
    {
        isFaceUp = show;
        cardImage.sprite = show ? cardFace : cardBack; // Set the card image
    }

    // Show the card back side
    public void ShowCardBack()
    {
        FlipCard(false);
    }

    // Show the card face side
    public void ShowCardFace()
    {
        FlipCard(true);
    }

}
