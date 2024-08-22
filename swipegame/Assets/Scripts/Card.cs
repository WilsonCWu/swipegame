using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class CardUtils
{
    public static string CardsToString(List<Card> cards)
    {
        string result = "";
        foreach (Card card in cards)
        {
            result += card.ToString() + ", ";
        }
        return result;
    }
    public static string HandToString(Hand hand)
    {
        return hand.handType.ToString() + ": " + CardsToString(hand.cards);
    }
    public static List<Card> InitializeDeck(StartingDeckType startingDeckType)
    {
        List<Card> deck = new List<Card>();
        switch (startingDeckType)
        {
            case StartingDeckType.Standard:
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                    {
                        deck.Add(new Card(suit, rank));
                    }
                }
                break;
            case StartingDeckType.NoFaceCards:
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                    {
                        if (rank < Rank.Jack)
                        {
                            deck.Add(new Card(suit, rank));
                        }
                    }
                }
                break;
            case StartingDeckType.OnlyRedCards:
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    foreach (Rank rank in Enum.GetValues(typeof(Rank)))
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
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    foreach (Rank rank in Enum.GetValues(typeof(Rank)))
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
                for (int i = 0; i < 52; i++)
                {
                    deck.Add(Card.RandomCard());
                }
                break;
        }
        return deck;
    }
    public static Hand ContainsRoyalFlush(List<Card> cards)
    {
        // Returns a tuple of whether the cards contain a royal flush and the cards that form the royal flush
        // cards can contain any number of cards
        // Royal flush is a straight flush from 10 to Ace

        // use ContainsStraightFlush
        Hand straightFlush = ContainsStraightFlush(cards);
        if (straightFlush == null)
        {
            return null;
        }
        List<Card> straightFlushCards = straightFlush.cards;
        if (straightFlushCards[0].Rank == Rank.Ace && straightFlushCards[straightFlushCards.Count - 1].Rank == Rank.Ten)
        {
            return new Hand(HandType.RoyalFlush, straightFlush.cards);
        }
        return null;
    }

    public static Hand ContainsStraightFlush(List<Card> cards)
    {
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
                    return new Hand(HandType.StraightFlush, new List<Card> { rankToCard[ranks[i]], rankToCard[ranks[i + 1]], rankToCard[ranks[i + 2]], rankToCard[ranks[i + 3]], rankToCard[ranks[i + 4]] });
                }
            }
            // check ace low straight
            if (rankToCard.ContainsKey(Rank.Ace) && rankToCard.ContainsKey(Rank.Two) && rankToCard.ContainsKey(Rank.Three) && rankToCard.ContainsKey(Rank.Four) && rankToCard.ContainsKey(Rank.Five))
            {
                return new Hand(HandType.StraightFlush, new List<Card> { rankToCard[Rank.Ace], rankToCard[Rank.Two], rankToCard[Rank.Three], rankToCard[Rank.Four], rankToCard[Rank.Five] });
            }
        }
        return null;
    }

    public static Hand ContainsNOfAKind(List<Card> cards, int n, HandType handType)
    {
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
                return new Hand(handType, rankCards.GetRange(0, n));
            }
        }
        return null;
    }

    public static Hand ContainsFourOfAKind(List<Card> cards)
    {
        return ContainsNOfAKind(cards, 4, HandType.FourOfAKind);
    }

    public static Hand ContainsThreeOfAKind(List<Card> cards)
    {
        return ContainsNOfAKind(cards, 3, HandType.ThreeOfAKind);
    }


    public static Hand ContainsTwoOfAKind(List<Card> cards)
    {
        return ContainsNOfAKind(cards, 2, HandType.Pair);
    }


    public static Hand ContainsHighCard(List<Card> cards)
    {
        return ContainsNOfAKind(cards, 1, HandType.HighCard);
    }

    public static Hand ContainsFullHouse(List<Card> cards)
    {
        // Returns a tuple of whether the cards contain a full house and the cards that form the full house
        // cards can contain any number of cards
        // Full house is a three of a kind and a pair
        // If there are multiple full houses, return the highest

        Hand threeOfAKind = ContainsThreeOfAKind(cards);
        if (threeOfAKind == null)
        {
            return null;
        }
        List<Card> threeOfAKindCards = threeOfAKind.cards;
        List<Card> remainingCards = new List<Card>(cards);
        foreach (Card card in threeOfAKindCards)
        {
            remainingCards.Remove(card);
        }
        Hand twoOfAKind = ContainsTwoOfAKind(remainingCards);
        if (twoOfAKind == null)
        {
            return null;
        }
        List<Card> twoOfAKindCards = twoOfAKind.cards;
        return new Hand(HandType.FullHouse, threeOfAKindCards.Concat(twoOfAKindCards).ToList());
    }

    public static Hand ContainsTwoPair(List<Card> cards)
    {
        // Returns a tuple of whether the cards contain two pair and the cards that form the two pair
        // cards can contain any number of cards
        // Two pair is two pairs of cards of the same rank
        // If there are multiple two pairs, return the highest

        Hand pair1 = ContainsTwoOfAKind(cards);
        if (pair1 == null)
        {
            return null;
        }
        List<Card> pair1Cards = pair1.cards;
        List<Card> remainingCards = new List<Card>(cards);
        foreach (Card card in pair1Cards)
        {
            remainingCards.Remove(card);
        }
        Hand pair2 = ContainsTwoOfAKind(remainingCards);
        if (pair2 == null)
        {
            return null;
        }
        List<Card> pair2Cards = pair2.cards;
        return new Hand(HandType.TwoPair, pair1Cards.Concat(pair2Cards).ToList());
    }
    public static Hand ContainsStraight(List<Card> cards)
    {
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
                return new Hand(HandType.Straight, new List<Card> { rankToCard[ranks[i]], rankToCard[ranks[i + 1]], rankToCard[ranks[i + 2]], rankToCard[ranks[i + 3]], rankToCard[ranks[i + 4]] });
            }
        }
        // check ace low straight
        if (rankToCard.ContainsKey(Rank.Ace) && rankToCard.ContainsKey(Rank.Two) && rankToCard.ContainsKey(Rank.Three) && rankToCard.ContainsKey(Rank.Four) && rankToCard.ContainsKey(Rank.Five))
        {
            return new Hand(HandType.Straight, new List<Card> { rankToCard[Rank.Ace], rankToCard[Rank.Two], rankToCard[Rank.Three], rankToCard[Rank.Four], rankToCard[Rank.Five] });
        }
        return null;
    }
    public static Hand ContainsFlush(List<Card> cards)
    {
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
                return new Hand(HandType.Flush, suitCards.GetRange(0, 5));
            }
        }
        return null;
    }

    public static Hand EvaluateHand(List<Card> cards)
    {
        // same but sorted
        Hand result = _EvaluateHand(cards);
        result.cards.Sort();
        result.cards.Reverse();
        return result;
    }


    private static Hand _EvaluateHand(List<Card> cards)
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
        Assert.IsTrue(cards.Count != 0);
        Hand royalFlush = ContainsRoyalFlush(cards);
        if (royalFlush != null)
        {
            return royalFlush;
        }
        Hand straightFlush = ContainsStraightFlush(cards);
        if (straightFlush != null)
        {
            return straightFlush;
        }
        Hand fourOfAKind = ContainsFourOfAKind(cards);
        if (fourOfAKind != null)
        {
            return fourOfAKind;
        }
        Hand fullHouse = ContainsFullHouse(cards);
        if (fullHouse != null)
        {
            return fullHouse;
        }
        Hand flush = ContainsFlush(cards);
        if (flush != null)
        {
            return flush;
        }
        Hand straight = ContainsStraight(cards);
        if (straight != null)
        {
            return straight;
        }
        Hand threeOfAKind = ContainsThreeOfAKind(cards);
        if (threeOfAKind != null)
        {
            return threeOfAKind;
        }
        Hand twoPair = ContainsTwoPair(cards);
        if (twoPair != null)
        {
            return twoPair;
        }
        Hand pair = ContainsTwoOfAKind(cards);
        if (pair != null)
        {
            return pair;
        }
        Hand highCard = ContainsHighCard(cards);
        if (highCard != null)
        {
            return highCard;
        }
        Assert.IsTrue(false);
        return new Hand(HandType.HighCard, new List<Card>());
    }
}

public class Hand
{
    public HandType handType;
    public List<Card> cards;

    public Hand(HandType handType, List<Card> cards)
    {
        this.handType = handType;
        this.cards = cards;
    }
}


public class Card : IComparable
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
        string str =  SuitToString(Suit) + RankToString(Rank);
        if (Suit == Suit.Clubs || Suit == Suit.Spades)
        {
            str = "<color=black>" + str + "</color>";
        }
        else
        {
            str = "<color=red>" + str + "</color>";
        }
        return str;
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

    public static Card RandomCard()
    {
        return new Card(Utils.RandomEnumValue<Suit>(), Utils.RandomEnumValue<Rank>());
    }
}

public enum Suit
{
    Spades=4,
    Hearts=3,
    Clubs=2,
    Diamonds = 1, // TODO: diamond lowest for color constrast
}

public enum Rank
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

public enum HandType
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
