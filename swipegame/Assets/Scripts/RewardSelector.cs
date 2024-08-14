using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardSelector : MonoBehaviour
{
    public TextMeshProUGUI description;
    Reward reward;

    public void Init(Reward reward)
    {
        this.reward = reward;
        description.text = reward.ToString();
    }

    public void OnClicked()
    {
        GameController.Instance.TriggerReward(reward);
        Destroy(this.gameObject);
    }
}
