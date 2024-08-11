using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: refactor and combine with cardselectorManager
public class RelicSelectorManager : MonoBehaviour
{
    public RelicSelector selectorPrefab;
    public Transform selectorParent;
    public GameObject selectorObject;
    private List<RelicSelector> selectors = new List<RelicSelector>();
    private static RelicSelectorManager _instance;
    public static RelicSelectorManager Instance { get { return _instance; } }
    public Action<Relic> selectedRelicsCallback;


    public void InitRelicSelection(List<Relic> relics, Action<Relic> selectedRelicsCallback)
    {
        this.selectedRelicsCallback = selectedRelicsCallback;
        foreach (Relic relic in relics)
        {
            RelicSelector relicSelector = Instantiate(selectorPrefab, selectorParent);
            relicSelector.Init(relic);
            selectors.Add(relicSelector);
        }
        selectorObject.SetActive(true);
    }

    public void OnConfirmClicked()
    {
        // TODO
        Relic selectedRelic = GetSelectedRelicsAndClear()[0];
        Debug.Log("Relic selected: " + selectedRelic.Name());
        selectedRelicsCallback.Invoke(selectedRelic);
    }

    public void OnCancelClicked()
    {
        GetSelectedRelicsAndClear();
    }

    private List<Relic> GetSelectedRelicsAndClear()
    {
        List<Relic> selectedRelics = new List<Relic>();
        foreach (RelicSelector selector in selectors)
        {
            if (selector.Selected)
            {
                selectedRelics.Add(selector.Relic);
            }
        }
        selectorObject.SetActive(false);
        foreach (RelicSelector selector in selectors)
        {
            Destroy(selector.gameObject);
        }
        selectors.Clear();
        return selectedRelics;
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
