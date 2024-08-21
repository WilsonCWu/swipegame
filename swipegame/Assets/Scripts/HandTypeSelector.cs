using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HandTypeSelector : Selector<HandType>
{
    public TextMeshProUGUI handTypeName;
    public TextMeshProUGUI handTypeDescription;


    public override void RefreshVisuals()
    {
        handTypeName.text = Item.ToString();
        handTypeDescription.text = "TODO STATS";
        handTypeName.color = Selected ? Color.green : Color.black;
    }
}
