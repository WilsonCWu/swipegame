using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class RelicSelector : Selector<Relic>
{
    public TextMeshProUGUI relicName;
    public TextMeshProUGUI relicDescription;


    public override void RefreshVisuals()
    {
        relicName.text = Item.Name();
        relicDescription.text = Item.Description();
        relicName.color = Selected ? Color.green : Color.black;
    }
}
