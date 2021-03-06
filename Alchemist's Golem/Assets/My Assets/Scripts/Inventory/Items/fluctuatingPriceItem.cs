﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fluctuatingPriceItem : Item
{
    public int currentPercentagePrice = 100;
    public int percentageMaxDif = 30; //+- 30% of og price

    private float p_difperStock;
    private int basePrice;

    public int playerStock = 0; //num sold
    public int dailyStockDecrease = 10;
    public int stockNumForMaxDecrease = 60;

    public int state = 1; //0 price down, 1 price same, 2 price up

    public FluctuationSaveData SaveInfo()
    {
        FluctuationSaveData saveData = new FluctuationSaveData();
        saveData.saveState = state;
        saveData.saveStock = playerStock;

        return saveData;
    }

    public void LoadInfo(FluctuationSaveData info)
    {
        state = info.saveState;
        playerStock = info.saveStock;


        UpdatePrice();
    }

    private void Start()
    {
        usable = false;
        basePrice = sellPrice;

        p_difperStock = ((float)percentageMaxDif * 2.0f) / (float)stockNumForMaxDecrease;
    }

    public void NewGame()
    {
        state = 1;
    }

    public void FluctuatePrice(int numSold)
    {
        playerStock = playerStock + numSold;

        UpdatePrice();
    }

    public void UpdatePrice()
    {
        float dif = percentageMaxDif - (p_difperStock * (float)playerStock);
        
        if (dif > 30.0f)
        {
            dif = 30.0f;
        }
        else if (dif < -30.0f)
        {
            dif = -30.0f;
        }

        currentPercentagePrice = 100 + (int)dif;

        int previousPrice = sellPrice;
        float floatPrice = (float)basePrice * ((float)currentPercentagePrice / 100.0f);
        sellPrice = (int)floatPrice;

        if (sellPrice > previousPrice)//up
        {
            state = 2;
        }
        else if (sellPrice < previousPrice)//down
        {
            state = 0;
        }
        else //same
        {
            state = 1;
        }
    }

    public void TickPriceChange()
    {
        playerStock = playerStock - dailyStockDecrease;

        if (playerStock < 0)
        {
            playerStock = 0;
        }

        UpdatePrice();
    }

}

public class FluctuationSaveData
{
    public int ID;
    public int saveState;
    public int saveStock;
}
