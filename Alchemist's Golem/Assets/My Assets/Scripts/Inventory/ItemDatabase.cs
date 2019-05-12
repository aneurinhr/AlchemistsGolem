using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public Item[] itemlist;

    public List<FluctuationSaveData> SaveInfo()
    {
        List<FluctuationSaveData> saveData = new List<FluctuationSaveData>();

        for (int i = 0; i < itemlist.Length; i++)
        {
            if (itemlist[i].usable == false)
            {
                FluctuationSaveData temp = itemlist[i].GetComponent<fluctuatingPriceItem>().SaveInfo();
                temp.ID = i;
                saveData.Add(temp);
            }
        }

        return saveData;
    }

    public void LoadInfo(List<FluctuationSaveData> info)
    {
        int infoCount = 0;

        for (int i = 0; i < itemlist.Length; i++)
        {
            if (itemlist[i].usable == false)
            {
                itemlist[i].GetComponent<fluctuatingPriceItem>().LoadInfo(info[infoCount]);
                infoCount = infoCount + 1;
            }
        }
    }

    public void NewGame()
    {
        for (int i = 0; i < itemlist.Length; i++)
        {
            if (itemlist[i].usable == false)
            {
                itemlist[i].GetComponent<fluctuatingPriceItem>().NewGame();
            }
        }
    }

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
