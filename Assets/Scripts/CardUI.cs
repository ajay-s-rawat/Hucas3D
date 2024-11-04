using UnityEngine;
using UnityEngine.UI;
using System;

public class CardUI : MonoBehaviour
{
    public int cardID;               // Unique ID for matching pairs
    public bool isFaceUp = false;    // Track if the card is face up
    public event Action<CardUI> OnCardSelected;  // Event to notify selection

    private Image cardImage;
    private Button button;
    public Sprite cardFace;          // Face image for the card
    public Sprite cardBack;          // Back image for the card

    void Start()
    {
        cardImage = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(NotifyCardManager);
        ShowCardBack();
    }

    // Notify the CardManager when the card is clicked
    private void NotifyCardManager()
    {
        OnCardSelected?.Invoke(this);
    }

    public void SetCardData(Sprite faceSprite, Sprite backSprite, int id)
    {
        cardFace = faceSprite;
        cardBack = backSprite;
        cardID = id;
    }

    public void FlipCard(bool show)
    {
        isFaceUp = show;
        cardImage.sprite = show ? cardFace : cardBack;  // Set the card image
    }

    public void ShowCardBack()
    {
        FlipCard(false);
    }

    public void ShowCardFace()
    {
        FlipCard(true);
    }
}
