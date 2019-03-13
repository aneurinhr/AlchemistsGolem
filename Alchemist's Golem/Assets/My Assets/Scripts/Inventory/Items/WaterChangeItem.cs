using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterChangeItem : Item
{ 
    public int waterChange = 0;
    public AudioSource useSound;

    public override bool UseItemOnPlot(Plot plot)
    {
        int waterContent = plot.WaterContent;

        if (waterContent < 10)
        {
            plot.ChangeWaterContent(waterChange);
            useSound.Play();
            return true;
        }
        return false;
    }
}
