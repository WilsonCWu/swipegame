using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using System.Linq;
using System;

class RunState
{
    private List<Card> deck;
    private List<Relic> relics = new List<Relic>();
    private Dictionary<HandType, int> handTypeLevel = new Dictionary<HandType, int>();
    private int round = 0;
    private int coins = 0;
    private int curHandSize;

    public RunState(List<Card> deck, int curHandSize)
    {
        this.deck = deck;
        this.curHandSize = curHandSize;
        foreach (HandType handType in Enum.GetValues(typeof(HandType)))
        {
            handTypeLevel[handType] = 1;
        }
    }

    public int CurHandSize
    {
        get { return curHandSize; }
    }

    public List<Card> Deck
    {
        get { return deck; }
    }

    public List<Relic> Relics
    {
        get { return relics; }
    }

    public int Round
    {
        get { return round; }
    }

    public int Coins
    {
        get { return coins; }
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

    public void AddRelic(Relic relic)
    {
        relics.Add(relic);
    }

    public int HandTypeLevel(HandType handType)
    {
        return handTypeLevel[handType];
    }

    public void LevelUpHandType(HandType handType)
    {
        handTypeLevel[handType]++;
    }
    
    public void IncrementRoundAndRewards()
    {
        coins += 10;
        round++;
        // TODO: should wait for results to finish first
        List<Reward> rewards = new List<Reward>();
        for (int i = 0; i < 3; i++)
        {
            rewards.Add((Reward)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Reward)).Length));
        }
        RewardSelectorManager.Instance.InitRewardSelection(rewards);
    }

    public int GetTargetPoints()
    {
        return 100 + 50 * round;
    }
}


class GameState
{
    private RunState runState;
    private List<Card> pile;
    private int discardsLeft;
    private int points;
    private int submitsLeft;


    public GameState(int submitsLeft, RunState runState)
    {
        this.submitsLeft = submitsLeft;
        this.runState = runState;
        pile = new List<Card>(runState.Deck);
        ShufflePile();
    }
    public void Discard(List<Card> cards){
        Assert.IsTrue(discardsLeft > 0);
        discardsLeft--;
        Remove(cards);
    }

    private void Remove(List<Card> cards)
    {
        // TODO: assert cards valid
        foreach (Card card in cards)
        {
            pile.Remove(card);
        }
    }

    public void Submit(List<Card> cards)
    {
        Assert.IsTrue(submitsLeft > 0);
        submitsLeft--;
        Remove(cards);
        EvaluateAndUpdatePoints(cards);
    }

    public void AddDiscards(int numDiscards){
        discardsLeft += numDiscards;
    }

    private void ShufflePile()
    {
        // Implement the logic to shuffle the drawPile list here
        // You can use the Fisher-Yates shuffle algorithm
        for (int i = pile.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            Card temp = pile[i];
            pile[i] = pile[randomIndex];
            pile[randomIndex] = temp;
        }
    }
    

    public override string ToString()
    {
        string result = "Pile: " + CardUtils.CardsToString(pile);
        return result;
    }


    private void EvaluateAndUpdatePoints(List<Card> cards)
    {
        // score = base points * handType multiplier
        // base points scale off of handType and cards in hand
        // multiplier scales off of handType
        Hand hand = CardUtils.EvaluateHand(cards);
        int basePoints = 0;
        switch (hand.handType)
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
        // TODO better scaling
        basePoints *= runState.HandTypeLevel(hand.handType);
        foreach (Relic relic in runState.Relics)
        {
            basePoints += relic.GetBasePoints(hand);
        }
        foreach (Card card in hand.cards)
        {
            basePoints += (int)card.Rank;
        }
        float multiplier = 1;
        switch (hand.handType)
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
        foreach (Relic relic in runState.Relics)
        {
            multiplier += relic.GetBaseMultiplier(hand);
        }
        foreach (Relic relic in runState.Relics)
        {
            multiplier *= relic.GetMultiplier(hand);
        }
        points += (int)(basePoints * multiplier);
        Debug.Log("Added " + basePoints * multiplier + " points. basePoints=" + basePoints + ", multiplier=" + multiplier);
    }

    public int SubmitsLeft
    {
        get { return submitsLeft; }
    }

    public int Points
    {
        get { return points; }
    }

    public List<Card> CurHand
    {
        get { return pile.GetRange(0, runState.CurHandSize); }
    }
    public List<Card> DrawPile
    {
        get { return pile.GetRange(runState.CurHandSize, pile.Count- runState.CurHandSize); }
    }
    public int DiscardsLeft
    {
        get { return discardsLeft; }
    }
    public List<Card> Deck
    {
        get { return runState.Deck; }
    }

    public RunState RunState
    {
        get { return runState; }
    }

}

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI cardsLeftText;
    public TextMeshProUGUI drawPileContentsText;
    public TextMeshProUGUI countsLeftText;
    public TextMeshProUGUI discardsLeftText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI submitsLeftText;
    public TextMeshProUGUI handTypeLevelsText;
    public TextMeshProUGUI relicsText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI roundPointsText;
    public TextMeshProUGUI coinsText;
    public StartingDeckType startingDeckType;
    public int relicMaxSelectable;
    public int relicCurHandSize;
    public int relicInitialDiscards;
    public int relicSubmitLimit;
    public int relicDiscardsGainedPerSubmit;
    public int relicDiscardsGainedPerSelect;
    private GameState gameState;
    private static GameController _instance;
    public static GameController Instance { get { return _instance; } }


    void UpdateUI()
    {
        cardsLeftText.text = "Cards Left: " + gameState.DrawPile.Count;
        // update draw pile contents in sorted order. 
        // show the relicNumDrawSeeable number of cards, then show rest.
        // Note that we draw from the back of the draw pile
        drawPileContentsText.text = "";
        List<Card> sortedDrawPile = new List<Card>(gameState.DrawPile);
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
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
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
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
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
        handTypeLevelsText.text = "";
        foreach (HandType handType in Enum.GetValues(typeof(HandType)))
        {
            handTypeLevelsText.text += handType.ToString() + ": " + gameState.RunState.HandTypeLevel(handType) + "\n";
        }
        relicsText.text = "";
        foreach (Relic relic in gameState.RunState.Relics)
        {
            relicsText.text += relic.ToString() + "\n";
        }
        roundText.text = "RD: " + gameState.RunState.Round;
        roundPointsText.text = "Target Pts: " + gameState.RunState.GetTargetPoints();
        coinsText.text = "Coins: " + gameState.RunState.Coins;
    }

    void ResetGame(bool keepRunState = false){
        RunState runState = keepRunState ? gameState.RunState : new RunState(CardUtils.InitializeDeck(startingDeckType), relicCurHandSize);
        gameState = new GameState(relicSubmitLimit, runState);
        CurCardSelectorManager.Instance.InitSelection(gameState.CurHand, relicMaxSelectable);
        gameState.AddDiscards(relicInitialDiscards);
        UpdateUI();
    }

    void OnAddCards(List<Card> cards)
    {
        gameState.RunState.AddCards(cards);
        ResetGame(true);
    }

    void OnRemoveCards(List<Card> cards)
    {
        gameState.RunState.RemoveCards(cards);
        ResetGame(true);
    }

    void OnAddRelic(Relic relic)
    {
        gameState.RunState.AddRelic(relic);
        ResetGame(true);
    }

    void OnLevelUpHandType(HandType handType)
    {
        gameState.RunState.LevelUpHandType(handType);
        ResetGame(true);
    }

    public void TriggerReward(Reward reward)
    {
        switch (reward)
        {
            case Reward.AddCard:
                // Generate random cards to add
                List<Card> cards = new List<Card>();
                for (int i = 0; i < 3; i++)
                {
                    cards.Add(Card.RandomCard());
                }
                CardSelectorManager.Instance.InitCardSelection(cards, 1, OnAddCards);
                break;
            case Reward.RemoveCard:
                CardSelectorManager.Instance.InitCardSelection(gameState.Deck, 1, OnRemoveCards);
                break;
            case Reward.RandomRelic:
                // Generate random relics to add
                List<Relic> relics = new List<Relic>();
                for (int i = 0; i < 3; i++)
                {
                    relics.Add(RelicUtils.RandomRelic());
                }
                RelicSelectorManager.Instance.InitRelicSelection(relics, OnAddRelic);
                break;
            case Reward.RandomHandLvl:
                List<HandType> handTypes = new List<HandType>();
                for (int i = 0; i < 3; i++)
                {
                    handTypes.Add(Utils.RandomEnumValue<HandType>());
                }
                HandTypeSelectorManager.Instance.InitHandTypeSelection(handTypes, OnLevelUpHandType);
                break;
            default:
                Debug.LogError("Reward not implemented: " + reward);
                break;
        }
    }
    public void OnDiscard(List<Card> cards)
    {
        gameState.Discard(cards);
        CurCardSelectorManager.Instance.InitSelection(gameState.CurHand, relicMaxSelectable);
        UpdateUI();
    }

    public void OnSubmit(List<Card> cards)
    {
        gameState.Submit(cards);
        CurCardSelectorManager.Instance.InitSelection(gameState.CurHand, relicMaxSelectable);
        UpdateUI();
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("Destroying duplicate GameController");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState.Points >= gameState.RunState.GetTargetPoints())
        {
            gameState.RunState.IncrementRoundAndRewards();
            ResetGame(true);
        }
        else if (gameState.SubmitsLeft == 0)
        {
            Debug.Log("Game over! Points: " + gameState.Points);
            ResetGame();
        }
        // input
        if (Input.GetKeyDown(KeyCode.N))
        {
            ResetGame();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame(true);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CardSelectorManager.Instance.InitCardSelection(gameState.Deck, 1, OnAddCards);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            TriggerReward(Reward.AddCard);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            TriggerReward(Reward.RemoveCard);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            TriggerReward(Reward.RandomRelic);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            TriggerReward(Reward.RandomHandLvl);
        }
    }
}



public enum Reward
{
    AddCard,
    RemoveCard,
    RandomRelic,
    RandomHandLvl,
}