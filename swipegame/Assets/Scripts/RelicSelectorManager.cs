using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicSelectorManager : SelectorManager<Relic>
{
    private static RelicSelectorManager _instance;
    public static RelicSelectorManager Instance { get { return _instance; } }
    public Action<Relic> selectedRelicsCallback;


    public override void RefreshUI()
    {

    }

    public void InitRelicSelection(List<Relic> relics, Action<Relic> selectedRelicsCallback)
    {
        this.selectedRelicsCallback = selectedRelicsCallback;
        InitSelection(relics, 1);

    }
    public void OnConfirmClicked()
    {
        // TODO
        Relic selectedRelic = SelectedItems()[0];
        Debug.Log("Relic selected: " + selectedRelic.Name());
        selectedRelicsCallback.Invoke(selectedRelic);
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
            Debug.LogError("Destroying duplicate RelicSelectorManager");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
