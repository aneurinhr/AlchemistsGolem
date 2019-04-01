using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionHandinSlot : Slot
{
    public Text quantityDisplay;
    public Image overlayImage;

    public int TurnInItemID;
    public int totalNeeded;
    public int currentHandinNeeded = 0;

    public Inventory inventory;
    public ItemDatabase database;
    public SideMission mission;

    public void SetRequirements(int TurnIn, int QTurnIn)
    {
        TurnInItemID = TurnIn;
        totalNeeded = QTurnIn;
        currentHandinNeeded = totalNeeded;

        quantityDisplay.text = currentHandinNeeded.ToString();
        overlayImage.sprite = database.GetItem(TurnIn).itemImage;
    }

    public override void Additional()
    {
        if (hotBarPointers != 999)
        {
            if (TurnInItemID == hotBarPointers)//if correct item
            {
                currentHandinNeeded = currentHandinNeeded - hotBarQuant;

                if (currentHandinNeeded < 0)//Means extras
                {
                    int extras = -currentHandinNeeded;
                    currentHandinNeeded = 0;

                    inventory.AddItem(hotBarPointers, extras);//return extras to invent
                }

                if (currentHandinNeeded == 0)
                {
                    mission.CompleteMission();
                }

                quantityDisplay.text = currentHandinNeeded.ToString();
            }
            else //Return item to inventory
            {
                inventory.AddItem(hotBarPointers, hotBarQuant);

                hotBarPointers = 999;
                hotBarQuant = 0;
            }
        }
    }

}
