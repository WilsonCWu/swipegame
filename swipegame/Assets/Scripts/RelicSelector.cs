using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RelicSelector : MonoBehaviour
{
    public TextMeshProUGUI relicName;
    public TextMeshProUGUI relicDescription;
    private bool selected = false;
    Relic relic;

    public Relic Relic
    {
        get { return relic; }
    }

    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            if (selected)
            {
                relicName.color = Color.green;
            }
            else
            {
                relicName.color = Color.black;
            }
        }
    }

    public void Init(Relic relic)
    {
        this.relic = relic;
        relicName.text = relic.Name();
        relicDescription.text = relic.Description();
    }

    public void OnClicked()
    {
        Selected = !Selected;
    }
}
