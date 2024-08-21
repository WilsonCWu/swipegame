using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurCardSelectorManager : SelectorManager<Card>
{
    private static CurCardSelectorManager _instance;
    public static CurCardSelectorManager Instance { get { return _instance; } }
    public TextMeshProUGUI handTypeText;

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
