using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSelectorManager : MonoBehaviour
{
    public RewardSelector selectorPrefab;
    public Transform selectorParent;
    public GameObject selectorObject;
    private List<RewardSelector> selectors = new List<RewardSelector>();
    private static RewardSelectorManager _instance;
    public static RewardSelectorManager Instance { get { return _instance; } }


    public void InitRewardSelection(List<Reward> rewards)
    {
        foreach (Reward reward in rewards)
        {
            RewardSelector rewardSelector = Instantiate(selectorPrefab, selectorParent);
            rewardSelector.Init(reward);
            selectors.Add(rewardSelector);
        }
        selectorObject.SetActive(true);
    }

    public void OnClose()
    {
        selectorObject.SetActive(false);
        foreach (RewardSelector selector in selectors)
        {
            if (selector != null)
            {
                Destroy(selector.gameObject);
            }
        }
        selectors.Clear();
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("Destroying duplicate RewardSelectorManager");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
