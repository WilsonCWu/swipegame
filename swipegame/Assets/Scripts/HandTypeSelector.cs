using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandTypeSelector : MonoBehaviour
{
    public TextMeshProUGUI handTypeName;
    public TextMeshProUGUI handTypeDescription;
    private bool selected = false;
    HandType handType;

    public HandType HandType
    {
        get { return handType; }
    }

    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            if (selected)
            {
                handTypeName.color = Color.green;
            }
            else
            {
                handTypeName.color = Color.black;
            }
        }
    }

    public void Init(HandType handType)
    {
        this.handType = handType;
        handTypeName.text = handType.ToString();
        handTypeDescription.text = "TODO STATS";
    }

    public void OnClicked()
    {
        Selected = !Selected;
    }
}
