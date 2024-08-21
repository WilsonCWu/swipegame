using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class SelectorManager<T> : MonoBehaviour
{
    public Selector<T> selectorPrefab;
    public Transform selectorParent;
    public GameObject selectorObject;
    private List<Selector<T>> selectors = new List<Selector<T>>();
    private int maxSelectable;

    // TODO: redeisgn. maybe pass selector?
    public abstract void RefreshUI();
    public void InitSelection(List<T> items, int maxSelectable)
    {
        CloseAndClear();
        this.maxSelectable = maxSelectable;
        foreach (T item in items)
        {
            Selector<T> selector = Instantiate(selectorPrefab, selectorParent);
            selector.Init(this, item);
            selectors.Add(selector);
        }
        RefreshUI();
        SetVisible(true);
    }

    public List<T> SelectedItems()
    {

        List<T> ret = new List<T>();

        foreach (Selector<T> selector in SelectedSelectors())
        {
            ret.Add(selector.Item);
        }
        return ret;
    }

    public List<Selector<T>> SelectedSelectors()
    {
        List<Selector<T>> ret = new List<Selector<T>>();

        foreach(Selector<T> selector in selectors)
        {
            if (selector.Selected)
            {
                ret.Add(selector);
            }
        }
        return ret;
    }

    public void OnClicked(Selector<T> selector)
    {
        if (!selector.Selected && SelectedSelectors().Count == maxSelectable)
        {
            return;
        }
        selector.Selected = !selector.Selected;
        RefreshUI();
    }

    public void CloseAndClear()
    {
        selectorObject.SetActive(false);
        foreach (Selector<T> selector in selectors)
        {
            Destroy(selector.gameObject);
        }
        selectors.Clear();
    }

    public void SetVisible(bool visible)
    {
        selectorObject.SetActive(visible);
    }
}
