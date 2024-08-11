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
    public static string HandToString(Tuple<HandType, List<Card>> hand)
    {
        return hand.Item1.ToString() + ": " + CardsToString(hand.Item2);
    }
    public static List<Card> InitializeDeck(StartingDeckType startingDeckType)
    {
        List<Card> deck = new List<Card>();
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
                for (int i = 0; i < 52; i++)
                {
                    deck.Add(new Card((Suit)UnityEngine.Random.Range(1, 5), (Rank)UnityEngine.Random.Range(2, 15)));
                }
                break;
        }
        return deck;
    }
    public static Tuple<bool, List<Card>> containsRoyalFlush(List<Card> cards)
    {
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

    public static Tuple<bool, List<Card>> containsStraightFlush(List<Card> cards)
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

    public static Tuple<bool, List<Card>> containsNOfAKind(List<Card> cards, int n)
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
                return new Tuple<bool, List<Card>>(true, rankCards.GetRange(0, n));
            }
        }
        return new Tuple<bool, List<Card>>(false, new List<Card>());
    }

    public static Tuple<bool, List<Card>> containsFourOfAKind(List<Card> cards)
    {
        return containsNOfAKind(cards, 4);
    }

    public static Tuple<bool, List<Card>> containsThreeOfAKind(List<Card> cards)
    {
        return containsNOfAKind(cards, 3);
    }


    public static Tuple<bool, List<Card>> containsTwoOfAKind(List<Card> cards)
    {
        return containsNOfAKind(cards, 2);
    }


    public static Tuple<bool, List<Card>> containsHighCard(List<Card> cards)
    {
        return containsNOfAKind(cards, 1);
    }

    public static Tuple<bool, List<Card>> containsFullHouse(List<Card> cards)
    {
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

    public static Tuple<bool, List<Card>> containsTwoPair(List<Card> cards)
    {
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
    public static Tuple<bool, List<Card>> containsStraight(List<Card> cards)
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
    public static Tuple<bool, List<Card>> containsFlush(List<Card> cards)
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
                return new Tuple<bool, List<Card>>(true, suitCards.GetRange(0, 5));
            }
        }
        return new Tuple<bool, List<Card>>(false, new List<Card>());
    }

    public static Tuple<HandType, List<Card>> EvaluateHand(List<Card> cards)
    {
        // same but sorted
        Tuple<HandType, List<Card>> result = _EvaluateHand(cards);
        result.Item2.Sort();
        result.Item2.Reverse();
        return result;
    }


    private static Tuple<HandType, List<Card>> _EvaluateHand(List<Card> cards)
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
        Tuple<bool, List<Card>> royalFlush = containsRoyalFlush(cards);
        if (royalFlush.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.RoyalFlush, royalFlush.Item2);
        }
        Tuple<bool, List<Card>> straightFlush = containsStraightFlush(cards);
        if (straightFlush.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.StraightFlush, straightFlush.Item2);
        }
        Tuple<bool, List<Card>> fourOfAKind = containsFourOfAKind(cards);
        if (fourOfAKind.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.FourOfAKind, fourOfAKind.Item2);
        }
        Tuple<bool, List<Card>> fullHouse = containsFullHouse(cards);
        if (fullHouse.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.FullHouse, fullHouse.Item2);
        }
        Tuple<bool, List<Card>> flush = containsFlush(cards);
        if (flush.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.Flush, flush.Item2);
        }
        Tuple<bool, List<Card>> straight = containsStraight(cards);
        if (straight.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.Straight, straight.Item2);
        }
        Tuple<bool, List<Card>> threeOfAKind = containsThreeOfAKind(cards);
        if (threeOfAKind.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.ThreeOfAKind, threeOfAKind.Item2);
        }
        Tuple<bool, List<Card>> twoPair = containsTwoPair(cards);
        if (twoPair.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.TwoPair, twoPair.Item2);
        }
        Tuple<bool, List<Card>> pair = containsTwoOfAKind(cards);
        if (pair.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.Pair, pair.Item2);
        }
        Tuple<bool, List<Card>> highCard = containsHighCard(cards);
        if (highCard.Item1)
        {
            return new Tuple<HandType, List<Card>>(HandType.HighCard, highCard.Item2);
        }
        Assert.IsTrue(false);
        return new Tuple<HandType, List<Card>>(HandType.HighCard, new List<Card>());
    }
}