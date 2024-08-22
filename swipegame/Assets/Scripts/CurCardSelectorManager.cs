using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
enum SortMode
{
    ByRank,
    BySuit,
}


public class CurCardSelectorManager : SelectorManager<Card>
{
    private static CurCardSelectorManager _instance;
    public static CurCardSelectorManager Instance { get { return _instance; } }
    public TextMeshProUGUI handTypeText;
    private SortMode sortMode = SortMode.ByRank;

    private void RefreshCardOrder()
    {
        List<Selector<Card>> selectors = Selectors;
        switch (sortMode)
        {
            case SortMode.ByRank:
                // sort by rank, then by suit enum value
                selectors.Sort((a, b) =>
                {
                    int rankCompare = a.Item.Rank.CompareTo(b.Item.Rank);
                    if (rankCompare == 0)
                    {
                        return a.Item.Suit.CompareTo(b.Item.Suit);
                    }
                    return rankCompare;
                });
                break;
            case SortMode.BySuit:
                selectors.Sort((a, b) =>
                {
                    int suitCompare = a.Item.Suit.CompareTo(b.Item.Suit);
                    if (suitCompare == 0)
                    {
                        return a.Item.Rank.CompareTo(b.Item.Rank);
                    }
                    return suitCompare;
                });
                break;
            default:
                Debug.LogError("TODO missing implementation for " + sortMode);
                break;

        }
        ReorderSelectors(selectors);
    }

    public void OrderByRank()
    {
        sortMode = SortMode.ByRank;
        RefreshCardOrder();
    }

    public void OrderBySuit()
    {
        sortMode = SortMode.BySuit;
        RefreshCardOrder();
    }

    public override void RefreshUI()
    {
        List<Card> selectedCards = SelectedItems();
        if (selectedCards.Count > 0)
        {
            Hand bestHand = CardUtils.EvaluateHand(selectedCards);
            handTypeText.text = bestHand.handType.ToString();
        }
        else
        {
            handTypeText.text = "";
        }
        RefreshCardOrder();
    }
    public void OnSubmit()
    {
        List<Card> cards = SelectedItems();
        CloseAndClear();
        GameController.Instance.OnSubmit(cards);
    }

    public void OnDiscard()
    {
        List<Card> cards = SelectedItems();
        CloseAndClear();
        GameController.Instance.OnDiscard(cards);
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("Destroying duplicate CurCardSelectorManager");
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
