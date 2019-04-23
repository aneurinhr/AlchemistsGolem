using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bank : MonoBehaviour
{
    public int money = 10;
    public Text display;

    public int SaveGame()
    {
        return money;
    }

    public void LoadGame(int newMoney)
    {
        money = newMoney;
    }

    private void Start()
    {
        display.text = money.ToString();
    }

    public bool ChangeMoney(int change)
    {
        int temp = money + change;

        if (temp < 0)
        {
            return false;
        }
        else
        {
            money = temp;
            display.text = money.ToString();
            return true;
        }
    }

}
