using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;

class GameState
{
    private List<Card> deck;
    private List<Card> drawPile;
    private Card curCard;
    private List<Card> selectedCards;

    public Card CurCard
    {
        get { return curCard; }
    }
    public List<Card> SelectedCards
    {
        get { return selectedCards; }
    }
    public List<Card> DrawPile
    {
        get { return drawPile; }
    }

    public GameState()
    {
        InitializeDeck();
        drawPile = new List<Card>(deck);
        ShuffleDrawPile();
        selectedCards = new List<Card>();
    }

    public void DrawCard()
    {
        if (drawPile.Count == 0)
        {
            Debug.Log("reshuffling the draw pile");
            drawPile = new List<Card>(deck);
            ShuffleDrawPile();
        }
        curCard = drawPile[drawPile.Count - 1];
        drawPile.RemoveAt(drawPile.Count - 1);
    }

    public void SelectCurCard(){
        selectedCards.Add(curCard);
    }

    public void ClearSelectedCards(){
        selectedCards.Clear();
    }

    private void ShuffleDrawPile()
    {
        // Implement the logic to shuffle the drawPile list here
        // You can use the Fisher-Yates shuffle algorithm
        for (int i = drawPile.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            Card temp = drawPile[i];
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
    }

    private void InitializeDeck()
    {
        deck = new List<Card>();
        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
            {
                deck.Add(new Card(suit, rank));
            }
        }
    }

    public override string ToString()
    {
        string result = "Deck: ";
        foreach (Card card in deck)
        {
            result += card.ToString() + ", ";
        }
        result += "\nDraw Pile: ";
        foreach (Card card in drawPile)
        {
            result += card.ToString() + ", ";
        }
        result += "\nCurrent Card: " + curCard.ToString() + ", ";
        foreach(Card card in selectedCards){
            result += "\nSelected Card: " + card.ToString() + ", ";
        }
        return result;
    }
}

class Card
{
    public Suit Suit { get; }
    public Rank Rank { get; }

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public override string ToString()
    {
        string rankString = "";
        switch (Rank)
        {
            case Rank.Ace:
            rankString = "A";
            break;
            case Rank.King:
            rankString = "K";
            break;
            case Rank.Queen:
            rankString = "Q";
            break;
            case Rank.Jack:
            rankString = "J";
            break;
            default:
            rankString = ((int)Rank).ToString();
            break;
        }
        return rankString + Suit.ToString().Substring(0, 1);
    }
}

enum Suit
{
    Spades,
    Hearts,
    Diamonds,
    Clubs
}

enum Rank
{
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
}

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI curCardText;
    public TextMeshProUGUI[] selectedCardTexts;
    public TextMeshProUGUI cardsLeftText;
    private GameState gameState;


    void UpdateUI()
    {
        curCardText.text = gameState.CurCard.ToString();
        for (int i = 0; i < gameState.SelectedCards.Count; i++)
        {
            selectedCardTexts[i].text = gameState.SelectedCards[i].ToString();
        }
        for (int i = gameState.SelectedCards.Count; i < selectedCardTexts.Length; i++)
        {
            selectedCardTexts[i].text = "";
        }
        cardsLeftText.text = "Cards Left: " + gameState.DrawPile.Count;
    }
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(selectedCardTexts.Length == 7);
        gameState = new GameState();
        gameState.DrawCard();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        // input
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            gameState.DrawCard();
            UpdateUI();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            gameState.SelectCurCard();
            gameState.DrawCard();
            UpdateUI();
        }
        // If full, evaluate hand and flush selected cards
        if (gameState.SelectedCards.Count == 7 || Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Evaluate hand
            Debug.Log("TODO: Evaluate hand");
            Debug.Log(gameState.ToString());
            // Flush selected cards
            gameState.ClearSelectedCards();
            UpdateUI();
        }
    }
}
