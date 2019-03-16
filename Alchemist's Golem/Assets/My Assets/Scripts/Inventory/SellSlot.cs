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

    public Text fluctPercentage;
    public GameObject[] fluctSymbol;//1 = same

    private bool hovered = false;

    public override void Additional()
    {
        int temp = selected.tempSlotPointers;
        int price = database.GetItem(temp).sellPrice;
        price = price * selected.tempSlotQuant;

        if (database.GetItem(temp).usable == false)
        {
            database.GetItem(temp).GetComponent<fluctuatingPriceItem>().FluctuatePrice(selected.tempSlotQuant);
        }

        //Add price from money
        bank.ChangeMoney(price);

        //Reset
        hotBarPointers = 999;//999 mean no item
        hotBarQuant = 0;//max stack 99

        sell.Play();
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
                    int fluctSymbolPointer = 1;

                    if (database.GetItem(temp).GetComponent<fluctuatingPriceItem>())
                    {
                        fluctuatingPriceItem tempItem = database.GetItem(temp).GetComponent<fluctuatingPriceItem>();
                        int percentage = tempItem.currentPercentagePrice;
                        fluctPercentage.text = percentage.ToString();

                        fluctSymbolPointer = tempItem.state;

                    }
                    else
                    {
                        int defaultText = 100;
                        fluctPercentage.text = defaultText.ToString();
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        if (i == fluctSymbolPointer)
                        {
                            fluctSymbol[i].SetActive(true);
                        }
                        else
                        {
                            fluctSymbol[i].SetActive(false);
                        }
                    }

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

            int fluctSymbolPointer = 1;
            int defaultText = 100;
            fluctPercentage.text = defaultText.ToString();

            for (int i = 0; i < 3; i++)
            {
                if (i == fluctSymbolPointer)
                {
                    fluctSymbol[i].SetActive(true);
                }
                else
                {
                    fluctSymbol[i].SetActive(false);
                }
            }
        }
    }
}
