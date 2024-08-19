using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurCardSelector : Selector<Card>
{
    public TextMeshProUGUI cardName;
    public GameObject selectedBG;


    public override void RefreshVisuals()
    {
        cardName.text = Item.ToString();
        selectedBG.SetActive(Selected);
    }
}
