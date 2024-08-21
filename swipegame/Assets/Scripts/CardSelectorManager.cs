using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardSelectorManager : SelectorManager<Card>
{
    private static CardSelectorManager _instance;
    public static CardSelectorManager Instance { get { return _instance; } }
    public Action<List<Card>> selectedCardsCallback;


    public override void RefreshUI()
    {

    }
    public void InitCardSelection(List<Card> cards, int maxSelectable, Action<List<Card>> selectedCardsCallback)
    {
        this.selectedCardsCallback = selectedCardsCallback;
        InitSelection(cards, maxSelectable);
    }

    public void OnConfirmClicked()
    {
        List<Card> selectedCards = SelectedItems();
        Debug.Log("Cards selected: " + CardUtils.CardsToString(selectedCards));
        selectedCardsCallback.Invoke(selectedCards);
        CloseAndClear();
    }

    public void OnCancelClicked()
    {
        CloseAndClear();
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
