using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardSelectorManager : MonoBehaviour
{
    public CardSelector cardSelectorPrefab;
    public Transform cardSelectorParent;
    public GameObject selectorObject;
    private List<CardSelector> selectors = new List<CardSelector>();
    private static CardSelectorManager _instance;
    public static CardSelectorManager Instance { get { return _instance; } }


    public void InitCardSelection(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            CardSelector cardSelector = Instantiate(cardSelectorPrefab, cardSelectorParent);
            cardSelector.Init(card);
            selectors.Add(cardSelector);
        }
        selectorObject.SetActive(true);
    }

    public void OnConfirmClicked()
    {
        List<Card> selectedCards = GetSelectedCardsAndClear();
        Debug.Log("Cards selected: " + Utils.CardsToString(selectedCards));
        Debug.LogError("Implement this method");
    }

    private List<Card> GetSelectedCardsAndClear(){
        List<Card> selectedCards = new List<Card>();
        foreach (CardSelector selector in selectors)
        {
            if (selector.Selected)
            {
                selectedCards.Add(selector.Card);
            }
        }
        selectorObject.SetActive(false);
        foreach (CardSelector selector in selectors)
        {
            Destroy(selector.gameObject);
        }
        return selectedCards;
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("Destroying duplicate CardSelectorManager");
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
}
