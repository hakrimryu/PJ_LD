using System.Collections.Generic;
using UnityEngine;

public delegate void OnAddMoneyEventHandler();

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public int Money { get; set; } = 50;
    public int SummonCount { get; set; } = 20;

    public event OnAddMoneyEventHandler OnAddMoney;
    
    public List<Monster> monsters = new List<Monster>();

    public void AddMoney(int value)
    {
        Money += value;
        OnAddMoney?.Invoke();
    }

    public void AddMonster(Monster monster)
    {
        monsters.Add(monster);
    }

    public void RemoveMonster(Monster monster)
    {
        monsters.Remove(monster);
    }
}
