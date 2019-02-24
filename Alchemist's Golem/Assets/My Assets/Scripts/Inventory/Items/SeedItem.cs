using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedItem : Item
{
    public CropMother seed;

    public override void UseItemOnPlot(Plot plot)
    {
        if (plot.Occupied == false)
        {
            seed.NewPlant(plot);
        }
    }
}
