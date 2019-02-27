using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SellSlot : Slot
{
    public SelectedItem selected;
    public Text sellPrice;
    public ItemDatabase database;
    public Bank bank;
    public AudioSource sell;

    private bool hovered = false;

    public override void Additional()
    {
        int temp = selected.tempSlotPointers;
        int price = database.GetItem(temp).sellPrice;
        price = price * selected.tempSlotQuant;

        //Add price from money
        bank.ChangeMoney(price);

        //Reset
        hotBarPointers = 999;//999 mean no item
        hotBarQuant = 0;//max stack 99

        //sell.Play();
    }

    private void Update()
    {
        if (hoveredOver == true)
        {
            if (hovered == false)
            {
                int temp = selected.tempSlotPointers;

                if (temp != 999)
                {
                    int price = database.GetItem(temp).sellPrice;
                    price = price * selected.tempSlotQuant;

                    sellPrice.text = price.ToString();
                }
            }

            hovered = true;
        }
        else
        {
            sellPrice.text = "0";
            hovered = false;
        }
    }
}
