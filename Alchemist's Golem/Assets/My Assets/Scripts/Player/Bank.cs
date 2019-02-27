using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bank : MonoBehaviour
{
    public int money = 10;
    public Text display;

    private void Start()
    {
        display.text = money.ToString();
    }

    public void ChangeMoney(int change)
    {
        money = money + change;
        display.text = money.ToString();
    }

}
