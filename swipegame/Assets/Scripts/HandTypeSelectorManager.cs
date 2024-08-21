using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTypeSelectorManager : SelectorManager<HandType>
{
    private static HandTypeSelectorManager _instance;
    public static HandTypeSelectorManager Instance { get { return _instance; } }
    public Action<HandType> selectedHandTypesCallback;

    public override void RefreshUI()
    {
    }


    public void InitHandTypeSelection(List<HandType> handTypes, Action<HandType> selectedHandTypesCallback)
    {
        this.selectedHandTypesCallback = selectedHandTypesCallback;
        InitSelection(handTypes, 1);
    }

    public void OnConfirmClicked()
    {
        // TODO
        HandType selectedHandType = SelectedItems()[0];
        Debug.Log("HandType selected: " + selectedHandType.ToString());
        selectedHandTypesCallback.Invoke(selectedHandType);
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
            Debug.LogError("Destroying duplicate HandTypeSelectorManager");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
