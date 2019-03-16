using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public Item[] itemlist;

    public Item GetItem(int pointer)
    {
        if (pointer < itemlist.Length)
        {
            return itemlist[pointer];
        }

        return null;
    }

    public void UpdateFluctiatingPrices()
    {
        for (int i = 0; i < itemlist.Length; i++)
        {
            if (itemlist[i].usable == false)
            {
                itemlist[i].GetComponent<fluctuatingPriceItem>().TickPriceChange();
            }
        }
    }
}
