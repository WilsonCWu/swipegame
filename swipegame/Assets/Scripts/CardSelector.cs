using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardSelector : MonoBehaviour
{
    public TextMeshProUGUI cardName;
    private bool selected = false;
    Card card;

    public Card Card
    {
        get { return card; }
    }

    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            if (selected)
            {
                cardName.color = Color.green;
            }
            else
            {
                cardName.color = Color.black;
            }
        }
    }


    public void Init(Card card)
    {
        this.card = card;
        cardName.text = card.ToString();
    }

    public void OnClicked()
    {
        Selected = !Selected;
    }
}
