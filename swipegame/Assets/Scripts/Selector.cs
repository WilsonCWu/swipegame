using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Selector<T> : MonoBehaviour
{
    private SelectorManager<T> sm;
    private bool selected = false;
    private T item;

    public T Item
    {
        get { return item; }
    }

    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            RefreshVisuals();
        }
    }

    public abstract void RefreshVisuals();
    public void Init(SelectorManager<T> sm, T item)
    {
        this.sm = sm;
        this.item = item;
        RefreshVisuals();
    }

    public void OnClicked()
    {
        sm.OnClicked(this);
    }

}
