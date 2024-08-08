using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using System.Linq;
using System;

class Utils{
    public static string CardsToString(List<Card> cards){
        string result = "";
        foreach(Card card in cards){
            result += card.ToString() + ", ";
        }
        return result;
    }
    public static string HandToString(Tuple<HandType, List<Card>> hand){
        return hand.Item1.ToString() + ": " + CardsToString(hand.Item2);
    }
}
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
    private StartingDeckType startingDeckType;

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

    public GameState(int flopSize, int submitsLeft, StartingDeckType startingDeckType)
    {
        this.flopSize = flopSize;
        this.submitsLeft = submitsLeft;
        this.startingDeckType = startingDeckType;
        InitializeDeck();
        drawPile = new List<Card>(deck);
        ShuffleDrawPile();
        selectedCards = new List<Card>();
        GenerateFlop(flopSize);
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

    public void GenerateFlop(int flopSize){
        // Generate a flop of size flopSize. Generate a random card, not from the draw pile
        // and add it to the selected cards
        for (int i = 0; i < flopSize; i++)
        {
            selectedCards.Add(deck[UnityEngine.Random.Range(0, deck.Count)]);
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

    private void InitializeDeck()
    {
        deck = new List<Card>();
        switch (startingDeckType)
        {
            case StartingDeckType.Standard:
                foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
                {
                    foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
                    {
                        deck.Add(new Card(suit, rank));
                    }
                }
                break;
            case StartingDeckType.NoFaceCards:
                foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
                {
                    foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
                    {
                        if (rank < Rank.Jack)
                        {
                            deck.Add(new Card(suit, rank));
                        }
                    }
                }
                break;
            case StartingDeckType.OnlyRedCards:
                foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
                {
                    foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
                    {
                        if (suit == Suit.Hearts || suit == Suit.Diamonds)
                        {
                            deck.Add(new Card(suit, rank));
                            deck.Add(new Card(suit, rank));
                        }
                    }
                }
                break;
            case StartingDeckType.DoubleCardsBelow7:
                foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
                {
                    foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
                    {
                        deck.Add(new Card(suit, rank));
                        if (rank <= Rank.Seven)
                        {
                            deck.Add(new Card(suit, rank));
                        }
                    }
                }
                break;
            case StartingDeckType.Random:
                for(int i = 0; i < 52; i++){
                    deck.Add(new Card((Suit)UnityEngine.Random.Range(1, 5), (Rank)UnityEngine.Random.Range(2, 15)));
                }
                break;
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

    public static Tuple<bool, List<Card>> containsRoyalFlush(List<Card> cards){
        // Returns a tuple of whether the cards contain a royal flush and the cards that form the royal flush
        // cards can contain any number of cards
        // Royal flush is a straight flush from 10 to Ace

        // use containsStraightFlush
        Tuple<bool, List<Card>> straightFlush = containsStraightFlush(cards);
        if (!straightFlush.Item1)
        {
            return new Tuple<bool, List<Card>>(false, new List<Card>());
        }
        List<Card> straightFlushCards = straightFlush.Item2;
        if (straightFlushCards[0].Rank == Rank.Ace && straightFlushCards[straightFlushCards.Count - 1].Rank == Rank.Ten)
        {
            return new Tuple<bool, List<Card>>(true, straightFlushCards);
        }
        return new Tuple<bool, List<Card>>(false, new List<Card>());
    }

    public static Tuple<bool, List<Card>> containsStraightFlush(List<Card> cards){
        // Returns a tuple of whether the cards contain a straight flush and the cards that form the straight flush
        // cards can contain any number of cards
        // Straight flush is a straight of 5 cards of the same suit
        // If there are multiple straight flushes, return the highest

        Dictionary<Suit, List<Card>> suitToCards = new Dictionary<Suit, List<Card>>();
        foreach (Card card in cards)
        {
            if (!suitToCards.ContainsKey(card.Suit))
            {
                suitToCards[card.Suit] = new List<Card>();
            }
            suitToCards[card.Suit].Add(card);
        }
        foreach (List<Card> suitCards in suitToCards.Values)
        {
            if (suitCards.Count < 5)
            {
                continue;
            }
            Dictionary<Rank, Card> rankToCard = new Dictionary<Rank, Card>();
            foreach (Card card in suitCards)
            {
                rankToCard[card.Rank] = card;
            }
            List<Rank> ranks = new List<Rank>(rankToCard.Keys);
            ranks.Sort();
            ranks.Reverse();
            for (int i = 0; i < ranks.Count - 4; i++)
            {
                if (ranks[i] - ranks[i + 4] == 4)
                {
                    return new Tuple<bool, List<Card>>(true, new List<Card> { rankToCard[ranks[i]], rankToCard[ranks[i + 1]], rankToCard[ranks[i + 2]], rankToCard[ranks[i + 3]], rankToCard[ranks[i + 4]] });
                }
            }
            // check ace low straight
            if (rankToCard.ContainsKey(Rank.Ace) && rankToCard.ContainsKey(Rank.Two) && rankToCard.ContainsKey(Rank.Three) && rankToCard.ContainsKey(Rank.Four) && rankToCard.ContainsKey(Rank.Five))
            {
                return new Tuple<bool, List<Card>>(true, new List<Card> { rankToCard[Rank.Ace], rankToCard[Rank.Two], rankToCard[Rank.Three], rankToCard[Rank.Four], rankToCard[Rank.Five] });
            }
        }
        return new Tuple<bool, List<Card>>(false, new List<Card>());
    }

    public static Tuple<bool, List<Card>> containsNOfAKind(List<Card> cards, int n){
        // Returns a tuple of whether the cards contain n of a kind and the cards that form the n of a kind
        // cards can contain any number of cards
        // n of a kind is n cards of the same rank
        // If there are multiple n of a kinds, return the highest

        Dictionary<Rank, List<Card>> rankToCards = new Dictionary<Rank, List<Card>>();
        foreach (Card card in cards)
        {
            if (!rankToCards.ContainsKey(card.Rank))
            {
                rankToCards[card.Rank] = new List<Card>();
            }
            rankToCards[card.Rank].Add(card);
        }
        List<Rank> ranks = new List<Rank>(rankToCards.Keys);
        ranks.Sort();
        ranks.Reverse();
        foreach (Rank rank in ranks)
        {
            List<Card> rankCards = rankToCards[rank];
            if (rankCards.Count >= n)
            {
                return new Tuple<bool, List<Card>>(true, rankCards.GetRange(0, n));
            }
        }
        return new Tuple<bool, List<Card>>(false, new List<Card>());
    }

    public static Tuple<bool, List<Card>> containsFourOfAKind(List<Card> cards){
        return containsNOfAKind(cards, 4);
    }

    public static Tuple<bool, List<Card>> containsThreeOfAKind(List<Card> cards){
        return containsNOfAKind(cards, 3);
    }
    

    public static Tuple<bool, List<Card>> containsTwoOfAKind(List<Card> cards){
        return containsNOfAKind(cards, 2);
    }
    

    public static Tuple<bool, List<Card>> containsHighCard(List<Card> cards){
        return containsNOfAKind(cards, 1);
    }

    public static Tuple<bool, List<Card>> containsFullHouse(List<Card> cards){
        // Returns a tuple of whether the cards contain a full house and the cards that form the full house
        // cards can contain any number of cards
        // Full house is a three of a kind and a pair
        // If there are multiple full houses, return the highest

        Tuple<bool, List<Card>> threeOfAKind = containsThreeOfAKind(cards);
        if (!threeOfAKind.Item1)
        {
            return new Tuple<bool, List<Card>>(false, new List<Card>());
        }
        List<Card> threeOfAKindCards = threeOfAKind.Item2;
        List<Card> remainingCards = new List<Card>(cards);
        foreach (Card card in threeOfAKindCards)
        {
            remainingCards.Remove(card);
        }
        Tuple<bool, List<Card>> twoOfAKind = containsTwoOfAKind(remainingCards);
        if (!twoOfAKind.Item1)
        {
            return new Tuple<bool, List<Card>>(false, new List<Card>());
        }
        List<Card> twoOfAKindCards = twoOfAKind.Item2;
        return new Tuple<bool, List<Card>>(true, threeOfAKindCards.Concat(twoOfAKindCards).ToList());
    }

    public static Tuple<bool, List<Card>> containsTwoPair(List<Card> cards){
        // Returns a tuple of whether the cards contain two pair and the cards that form the two pair
        // cards can contain any number of cards
        // Two pair is two pairs of cards of the same rank
        // If there are multiple two pairs, return the highest

        Tuple<bool, List<Card>> pair1 = containsTwoOfAKind(cards);
        if (!pair1.Item1)
        {
            return new Tuple<bool, List<Card>>(false, new List<Card>());
        }
        List<Card> pair1Cards = pair1.Item2;
        List<Card> remainingCards = new List<Card>(cards);
        foreach (Card card in pair1Cards)
        {
            remainingCards.Remove(card);
        }
        Tuple<bool, List<Card>> pair2 = containsTwoOfAKind(remainingCards);
        if (!pair2.Item1)
        {
            return new Tuple<bool, List<Card>>(false, new List<Card>());
        }
        List<Card> pair2Cards = pair2.Item2;
        return new Tuple<bool, List<Card>>(true, pair1Cards.Concat(pair2Cards).ToList());
    }
    public static Tuple<bool, List<Card>> containsStraight(List<Card> cards){
        // Returns a tuple of whether the cards contain a straight and the cards that form the straight
        // cards can contain any number of cards
        // Straight is a sequence of 5 cards of any suit
        // If there are multiple straights, return the highest

        Dictionary<Rank, Card> rankToCard = new Dictionary<Rank, Card>();
        foreach (Card card in cards)
        {
            rankToCard[card.Rank] = card;
        }
        List<Rank> ranks = new List<Rank>(rankToCard.Keys);
        ranks.Sort();
        ranks.Reverse();
        for (int i = 0; i < ranks.Count - 4; i++)
        {
            if (ranks[i] - ranks[i + 4] == 4)
            {
                return new Tuple<bool, List<Card>>(true, new List<Card> { rankToCard[ranks[i]], rankToCard[ranks[i + 1]], rankToCard[ranks[i + 2]], rankToCard[ranks[i + 3]], rankToCard[ranks[i + 4]] });
            }
        }
        // check ace low straight
        if (rankToCard.ContainsKey(Rank.Ace) && rankToCard.ContainsKey(Rank.Two) && rankToCard.ContainsKey(Rank.Three) && rankToCard.ContainsKey(Rank.Four) && rankToCard.ContainsKey(Rank.Five))
        {
            return new Tuple<bool, List<Card>>(true, new List<Card> { rankToCard[Rank.Ace], rankToCard[Rank.Two], rankToCard[Rank.Three], rankToCard[Rank.Four], rankToCard[Rank.Five] });
        }
        return new Tuple<bool, List<Card>>(false, new List<Card>());
    }
    public static Tuple<bool, List<Card>> containsFlush(List<Card> cards){
        // Returns a tuple of whether the cards contain a flush and the cards that form the flush
        // cards can contain any number of cards
        // Flush is 5 cards of the same suit
        // If there are multiple flushes, return the highest

        Dictionary<Suit, List<Card>> suitToCards = new Dictionary<Suit, List<Card>>();
        foreach (Card card in cards)
        {
            if (!suitToCards.ContainsKey(card.Suit))
            {
                suitToCards[card.Suit] = new List<Card>();
            }
            suitToCards[card.Suit].Add(card);
        }
        foreach (List<Card> suitCards in suitToCards.Values)
        {
            suitCards.Sort();
            suitCards.Reverse();
            if (suitCards.Count >= 5)
            {
                return new Tuple<bool, List<Card>>(true, suitCards.GetRange(0, 5));
            }
        }
        return new Tuple<bool, List<Card>>(false, new List<Card>());
    }
    public void EvaluateAndUpdatePoints(){
        Assert.IsTrue(submitsLeft > 0);
        submitsLeft--;
        // score = base points * handType multiplier
        // base points scale off of handType and cards in hand
        // multiplier scales off of handType
        Tuple<HandType, List<Card>> hand = EvaluateHand();
        int basePoints = 0;
        switch(hand.Item1){
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
        foreach(Card card in hand.Item2){
            baseCardPoints += (int)card.Rank;
        }
        int multiplier = 1;
        switch(hand.Item1){
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

    public Tuple<HandType, List<Card>> EvaluateHand(){
        // same but sorted
        Tuple<HandType, List<Card>> result = _EvaluateHand();
        result.Item2.Sort();
        result.Item2.Reverse();
        return result;
    }


    private Tuple<HandType, List<Card>> _EvaluateHand()
    {
        // Returns the best hand that can be formed from the selected cards
        // and the cards that form the hand
        // The cards should be sorted in descending order of rank
        // The hand should be sorted in the following order:
        // RoyalFlush, StraightFlush, FourOfAKind, FullHouse, Flush, Straight, ThreeOfAKind, TwoPair, Pair, HighCard
        // Unlike poker, where top 5 cards are used, we only use cards that are a part of the handtype.
        // For example, if the hand is a pair, we only use the pair and not the other 5 cards.
        // If there are multiple hands of the same type, tie break with the highest value cards.
        // Otherwise, tie break by selecting first selected cards.
        Assert.IsTrue(selectedCards.Count != 0);
        Tuple<bool, List<Card>> royalFlush = containsRoyalFlush(selectedCards);
        if (royalFlush.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.RoyalFlush, royalFlush.Item2);
        }
        Tuple<bool, List<Card>> straightFlush = containsStraightFlush(selectedCards);
        if (straightFlush.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.StraightFlush, straightFlush.Item2);
        }
        Tuple<bool, List<Card>> fourOfAKind = containsFourOfAKind(selectedCards);
        if (fourOfAKind.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.FourOfAKind, fourOfAKind.Item2);
        }
        Tuple<bool, List<Card>> fullHouse = containsFullHouse(selectedCards);
        if (fullHouse.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.FullHouse, fullHouse.Item2);
        }
        Tuple<bool, List<Card>> flush = containsFlush(selectedCards);
        if (flush.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.Flush, flush.Item2);
        }
        Tuple<bool, List<Card>> straight = containsStraight(selectedCards);
        if (straight.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.Straight, straight.Item2);
        }
        Tuple<bool, List<Card>> threeOfAKind = containsThreeOfAKind(selectedCards);
        if (threeOfAKind.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.ThreeOfAKind, threeOfAKind.Item2);
        }
        Tuple<bool, List<Card>> twoPair = containsTwoPair(selectedCards);
        if (twoPair.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.TwoPair, twoPair.Item2);
        }
        Tuple<bool, List<Card>> pair = containsTwoOfAKind(selectedCards);
        if (pair.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.Pair, pair.Item2);
        }
        Tuple<bool, List<Card>> highCard = containsHighCard(selectedCards);
        if (highCard.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.HighCard, highCard.Item2);
        }
        Assert.IsTrue(false);
        return new Tuple<HandType, List<Card>>(HandType.HighCard, new List<Card>());
    }
}

class Card : IComparable
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
        return SuitToString(Suit) + RankToString(Rank);
    }

    public static string SuitToString(Suit suit)
    {
        switch(suit)
        {
            case Suit.Spades:
                return "♠";
            case Suit.Hearts:
                return "♥";
            case Suit.Diamonds:
                return "♦";
            case Suit.Clubs:
                return "♣";
            default:
                return "";
        }
    }

    public static string RankToString(Rank rank)
    {
        switch(rank)
        {
            case Rank.Ace:
                return "A";
            case Rank.Two:
                return "2";
            case Rank.Three:
                return "3";
            case Rank.Four:
                return "4";
            case Rank.Five:
                return "5";
            case Rank.Six:
                return "6";
            case Rank.Seven:
                return "7";
            case Rank.Eight:
                return "8";
            case Rank.Nine:
                return "9";
            case Rank.Ten:
                return "10";
            case Rank.Jack:
                return "J";
            case Rank.Queen:
                return "Q";
            case Rank.King:
                return "K";
            default:
                return "";
        }
    }

    public int CompareTo(object obj) {
        // Rank is more important than suit
        Card other = (Card)obj;
        int rankComparison = Rank.CompareTo(other.Rank);
        if (rankComparison == 0)
        {
            return Suit.CompareTo(other.Suit);
        }
        return rankComparison;
    }
}

enum Suit
{
    Spades=4,
    Hearts=3,
    Diamonds=2,
    Clubs=1,
}

enum Rank
{
    Ace = 14,
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

enum HandType
{
    HighCard,
    Pair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    RoyalFlush
}

public enum StartingDeckType
{
    Standard,
    NoFaceCards,
    OnlyRedCards,
    DoubleCardsBelow7,
    Random,
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
            Tuple<HandType, List<Card>> bestHand = gameState.EvaluateHand();
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
    void ResetGame(){
        gameState = new GameState(relicFlopSize, relicSubmitLimit, startingDeckType);
        gameState.DrawCard();
        gameState.AddDiscards(relicInitialDiscards);
        UpdateUI();
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
        // If full, evaluate hand and flush selected cards
        if ((gameState.SelectedCards.Count == 7 || (Input.GetKeyDown(KeyCode.UpArrow) && gameState.SelectedCards.Count > 0)) && gameState.SubmitsLeft > 0)
        {
            // Evaluate hand
            Debug.Log(gameState.ToString());
            Debug.Log(Utils.HandToString(gameState.EvaluateHand()));
            // Flush selected cards
            gameState.EvaluateAndUpdatePoints();
            gameState.ClearSelectedCards();
            gameState.AddDiscards(relicDiscardsGainedPerSubmit);
            gameState.GenerateFlop(relicFlopSize);
            UpdateUI();
        }
    }
}
