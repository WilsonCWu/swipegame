using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: refactor and combine with cardselectorManager
public class HandTypeSelectorManager : MonoBehaviour
{
    public HandTypeSelector selectorPrefab;
    public Transform selectorParent;
    public GameObject selectorObject;
    private List<HandTypeSelector> selectors = new List<HandTypeSelector>();
    private static HandTypeSelectorManager _instance;
    public static HandTypeSelectorManager Instance { get { return _instance; } }
    public Action<HandType> selectedHandTypesCallback;


    public void InitHandTypeSelection(List<HandType> handTypes, Action<HandType> selectedHandTypesCallback)
    {
        this.selectedHandTypesCallback = selectedHandTypesCallback;
        foreach (HandType handType in handTypes)
        {
            HandTypeSelector handTypeSelector = Instantiate(selectorPrefab, selectorParent);
            handTypeSelector.Init(handType);
            selectors.Add(handTypeSelector);
        }
        selectorObject.SetActive(true);
    }

    public void OnConfirmClicked()
    {
        // TODO
        HandType selectedHandType = GetSelectedHandTypesAndClear()[0];
        Debug.Log("HandType selected: " + selectedHandType.ToString());
        selectedHandTypesCallback.Invoke(selectedHandType);
    }

    public void OnCancelClicked()
    {
        GetSelectedHandTypesAndClear();
    }

    private List<HandType> GetSelectedHandTypesAndClear()
    {
        List<HandType> selectedHandTypes = new List<HandType>();
        foreach (HandTypeSelector selector in selectors)
        {
            if (selector.Selected)
            {
                selectedHandTypes.Add(selector.HandType);
            }
        }
        selectorObject.SetActive(false);
        foreach (HandTypeSelector selector in selectors)
        {
            Destroy(selector.gameObject);
        }
        selectors.Clear();
        return selectedHandTypes;
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
