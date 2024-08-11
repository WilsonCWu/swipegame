using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using System.Linq;
using System;


class GameState
{
    private List<Card> deck;
    private List<Card> drawPile;
    private Card curCard;
    private List<Card> selectedCards;
    private int discardsLeft;
    private int flopSize;
    private int points;
    private int submitsLeft;
    private FlopType flopType;
    private StartingDeckType startingDeckType;

    public GameState(int flopSize, int submitsLeft, List<Card> deck, FlopType flopType)
    {
        this.flopSize = flopSize;
        this.submitsLeft = submitsLeft;
        this.deck = deck;
        this.flopType = flopType;
        drawPile = new List<Card>(deck);
        ShuffleDrawPile();
        selectedCards = new List<Card>();
        GenerateFlop();
    }
    public void Discard(){
        Assert.IsTrue(discardsLeft > 0);
        discardsLeft--;
        DrawCard();
    }
    public void AddDiscards(int numDiscards){
        discardsLeft += numDiscards;
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

    public void GenerateFlop(){
        switch (flopType)
        {
            case FlopType.Standard:
                for (int i = 0; i < flopSize; i++)
                {
                    selectedCards.Add(Card.RandomCard());
                }
                break;
            case FlopType.FromDeck:
                // Generate a flop of size flopSize. Generate a random card from the draw pile
                // and add it to the selected cards
                for (int i = 0; i < flopSize; i++)
                {
                    selectedCards.Add(drawPile[UnityEngine.Random.Range(0, drawPile.Count)]);
                }
                break;
            case FlopType.NoFaceCards:
                for (int i = 0; i < flopSize; i++)
                {
                    Card card = Card.RandomCard();
                    while (card.Rank == Rank.Jack || card.Rank == Rank.Queen || card.Rank == Rank.King)
                    {
                        card = Card.RandomCard();
                    }
                    selectedCards.Add(card);
                }
                break;
            case FlopType.OnlyRedCards:
                for (int i = 0; i < flopSize; i++)
                {
                    Card card = Card.RandomCard();
                    while (card.Suit == Suit.Clubs || card.Suit == Suit.Spades)
                    {
                        card = Card.RandomCard();
                    }
                    selectedCards.Add(card);
                }
                break;
            case FlopType.OnlyConsecutive:
                int startingRank = UnityEngine.Random.Range(1, 12);
                if (startingRank == 1)
                {
                    selectedCards.Add(new Card((Suit)UnityEngine.Random.Range(1, 5), Rank.Ace));
                    selectedCards.Add(new Card((Suit)UnityEngine.Random.Range(1, 5), Rank.Two));
                    selectedCards.Add(new Card((Suit)UnityEngine.Random.Range(1, 5), Rank.Three));
                }
                else
                {
                    selectedCards.Add(new Card((Suit)UnityEngine.Random.Range(1, 5), (Rank)startingRank));
                    selectedCards.Add(new Card((Suit)UnityEngine.Random.Range(1, 5), (Rank)(startingRank + 1)));
                    selectedCards.Add(new Card((Suit)UnityEngine.Random.Range(1, 5), (Rank)(startingRank + 2)));
                }
                break;
        }
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
    

    public override string ToString()
    {
        string result = "Current Card: " + curCard.ToString() + ", ";
        result += "\nSelected Cards: ";
        foreach(Card card in selectedCards){
            result += card.ToString() + ", ";
        }
        result += "\nDeck: ";
        foreach (Card card in deck)
        {
            result += card.ToString() + ", ";
        }
        result += "\nDraw Pile: ";
        foreach (Card card in drawPile)
        {
            result += card.ToString() + ", ";
        }
        return result;
    }


    public void EvaluateAndUpdatePoints()
    {
        Assert.IsTrue(submitsLeft > 0);
        submitsLeft--;
        // score = base points * handType multiplier
        // base points scale off of handType and cards in hand
        // multiplier scales off of handType
        Tuple<HandType, List<Card>> hand = CardUtils.EvaluateHand(selectedCards);
        int basePoints = 0;
        switch (hand.Item1)
        {
            case HandType.HighCard:
                basePoints = 5;
                break;
            case HandType.Pair:
                basePoints = 10;
                break;
            case HandType.TwoPair:
                basePoints = 20;
                break;
            case HandType.ThreeOfAKind:
                basePoints = 30;
                break;
            case HandType.Straight:
                basePoints = 30;
                break;
            case HandType.Flush:
                basePoints = 35;
                break;
            case HandType.FullHouse:
                basePoints = 40;
                break;
            case HandType.FourOfAKind:
                basePoints = 60;
                break;
            case HandType.StraightFlush:
                basePoints = 100;
                break;
            case HandType.RoyalFlush:
                basePoints = 200;
                break;
        }
        int baseCardPoints = 0;
        foreach (Card card in hand.Item2)
        {
            baseCardPoints += (int)card.Rank;
        }
        int multiplier = 1;
        switch (hand.Item1)
        {
            case HandType.HighCard:
                multiplier = 1;
                break;
            case HandType.Pair:
                multiplier = 2;
                break;
            case HandType.TwoPair:
                multiplier = 2;
                break;
            case HandType.ThreeOfAKind:
                multiplier = 3;
                break;
            case HandType.Straight:
                multiplier = 4;
                break;
            case HandType.Flush:
                multiplier = 4;
                break;
            case HandType.FullHouse:
                multiplier = 4;
                break;
            case HandType.FourOfAKind:
                multiplier = 7;
                break;
            case HandType.StraightFlush:
                multiplier = 8;
                break;
            case HandType.RoyalFlush:
                multiplier = 10;
                break;
        }
        points += (basePoints + baseCardPoints) * multiplier;
        Debug.Log("Added " + (basePoints + baseCardPoints) * multiplier + " points. basePoints=" + basePoints + ", baseCardPoints=" + baseCardPoints + ", multiplier=" + multiplier);
    }

    public int SubmitsLeft
    {
        get { return submitsLeft; }
    }

    public int Points
    {
        get { return points; }
    }

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
    public int DiscardsLeft
    {
        get { return discardsLeft; }
    }
    public List<Card> Deck
    {
        get { return deck; }
    }

    public void AddCards(List<Card> cards)
    {
        deck.AddRange(cards);
    }

    public void RemoveCards(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            deck.Remove(card);
        }
    }

}

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI curCardText;
    public TextMeshProUGUI[] selectedCardTexts;
    public TextMeshProUGUI cardsLeftText;
    public TextMeshProUGUI drawPileContentsText;
    public TextMeshProUGUI countsLeftText;
    public TextMeshProUGUI discardsLeftText;
    public TextMeshProUGUI handTypeText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI submitsLeftText;
    public StartingDeckType startingDeckType;
    public FlopType relicFlopType;
    public int relicFlopSize;
    public int relicNumDrawSeeable;
    public int relicInitialDiscards;
    public int relicSubmitLimit;
    public int relicDiscardsGainedPerSubmit;
    public int relicDiscardsGainedPerSelect;
    private GameState gameState;


    void UpdateUI()
    {
        curCardText.text = gameState.CurCard.ToString();
        if (gameState.SelectedCards.Count != 0)
        {
            Tuple<HandType, List<Card>> bestHand = CardUtils.EvaluateHand(gameState.SelectedCards);
            for (int i = 0; i < gameState.SelectedCards.Count; i++)
            {
                // if selectedCard is in bestHand, remove it from bestHand and color it
                if (bestHand.Item2.Contains(gameState.SelectedCards[i]))
                {
                    selectedCardTexts[i].text = "<color=green>" + gameState.SelectedCards[i].ToString() + "</color>";
                    bestHand.Item2.Remove(gameState.SelectedCards[i]);
                }
                else
                {
                    selectedCardTexts[i].text = gameState.SelectedCards[i].ToString();
                }
            }
            handTypeText.text = bestHand.Item1.ToString();
        }
        else{
            handTypeText.text = "";
        }
        for (int i = gameState.SelectedCards.Count; i < selectedCardTexts.Length; i++)
        {
            selectedCardTexts[i].text = "";
        }
        cardsLeftText.text = "Cards Left: " + gameState.DrawPile.Count;
        // update draw pile contents in sorted order. 
        // show the relicNumDrawSeeable number of cards, then show rest.
        // Note that we draw from the back of the draw pile
        drawPileContentsText.text = "";
        List<Card> sortedDrawPile = new List<Card>(gameState.DrawPile);
        sortedDrawPile.Reverse();
        for (int i = 0; i < relicNumDrawSeeable && i < sortedDrawPile.Count; i++)
        {
            drawPileContentsText.text += "<color=yellow>" + sortedDrawPile[i].ToString() + "</color>, ";
        }
        sortedDrawPile.RemoveRange(0, Mathf.Min(relicNumDrawSeeable, sortedDrawPile.Count));
        sortedDrawPile.Sort((a, b) =>
        {
            int rankComparison = a.Rank.CompareTo(b.Rank);
            if (rankComparison == 0)
            {
            return a.Suit.CompareTo(b.Suit);
            }
            return rankComparison;
        });
        foreach (Card card in sortedDrawPile)
        {
            drawPileContentsText.text += card.ToString() + ", ";
        }
        // also show the number left of each card number, and each suit
        countsLeftText.text = "";
        foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
        {
            int count = 0;
            foreach (Card card in gameState.DrawPile)
            {
                if (card.Rank == rank)
                {
                    count++;
                }
            }
            countsLeftText.text += Card.RankToString(rank) + ": " + count + "\n";
        }
        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            int count = 0;
            foreach (Card card in gameState.DrawPile)
            {
                if (card.Suit == suit)
                {
                    count++;
                }
            }
            countsLeftText.text += Card.SuitToString(suit) + ": " + count + "\n";
        }
        discardsLeftText.text = "Discards Left: " + gameState.DiscardsLeft;
        pointsText.text = "Points: " + gameState.Points;
        submitsLeftText.text = "Submits Left: " + gameState.SubmitsLeft;
    }

    void ResetGame(bool keepDeck = false){
        List<Card> deck = keepDeck ? gameState.Deck : CardUtils.InitializeDeck(startingDeckType);
        gameState = new GameState(relicFlopSize, relicSubmitLimit, deck, relicFlopType);
        gameState.DrawCard();
        gameState.AddDiscards(relicInitialDiscards);
        UpdateUI();
    }

    void OnAddCards(List<Card> cards)
    {
        gameState.AddCards(cards);
        ResetGame(true);
    }

    void OnRemoveCards(List<Card> cards)
    {
        gameState.RemoveCards(cards);
        ResetGame(true);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(selectedCardTexts.Length == 7);
        ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        // input
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            ResetGame(true);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && gameState.DiscardsLeft > 0)
        {
            gameState.Discard();
            UpdateUI();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && gameState.SelectedCards.Count < 7)
        {
            gameState.SelectCurCard();
            gameState.DrawCard();
            gameState.AddDiscards(relicDiscardsGainedPerSelect);
            UpdateUI();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CardSelectorManager.Instance.InitCardSelection(gameState.Deck, OnAddCards);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            // Generate random cards to add
            List<Card> cards = new List<Card>();
            for (int i = 0; i < 3; i++)
            {
                cards.Add(Card.RandomCard());
            }
            CardSelectorManager.Instance.InitCardSelection(cards, OnAddCards);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CardSelectorManager.Instance.InitCardSelection(gameState.Deck, OnRemoveCards);
        }
        // If full, evaluate hand and flush selected cards
        if ((gameState.SelectedCards.Count == 7 || (Input.GetKeyDown(KeyCode.UpArrow) && gameState.SelectedCards.Count > 0)) && gameState.SubmitsLeft > 0)
        {
            // Evaluate hand
            Debug.Log(gameState.ToString());
            Debug.Log(CardUtils.HandToString(CardUtils.EvaluateHand(gameState.SelectedCards)));
            // Flush selected cards
            gameState.EvaluateAndUpdatePoints();
            gameState.ClearSelectedCards();
            gameState.AddDiscards(relicDiscardsGainedPerSubmit);
            gameState.GenerateFlop();
            UpdateUI();
        }
    }
}
