using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterChangeItem : Item
{ 
    public int waterChange = 0;
    public AudioSource useSound;

    public override void UseItemOnPlot(Plot plot)
    {
        plot.ChangeWaterContent(waterChange);
        useSound.Play();
    }
}
