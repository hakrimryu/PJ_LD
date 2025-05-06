using System;
using TMPro;
using UnityEngine;

public class UI_Main : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI monsterCount;
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private TextMeshProUGUI summonCount;

    [SerializeField] private Animator moneyAnimator;

    private void Start()
    {
        GameManager.Instance.OnAddMoney -= AddMoneyAnimation;
        GameManager.Instance.OnAddMoney += AddMoneyAnimation;
    }

    private void Update()
    {
        monsterCount.text = GameManager.Instance.monsters.Count + " / 100";
        money.text = GameManager.Instance.Money.ToString();
        summonCount.text = GameManager.Instance.SummonCount.ToString();
        
        summonCount.color = GameManager.Instance.Money >= GameManager.Instance.SummonCount ? Color.white : Color.red;
    }

    private void AddMoneyAnimation()
    {
        moneyAnimator.SetTrigger("Get");
    }
}
