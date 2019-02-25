using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedItem : Item
{
    public CropMother seed;
    public AudioSource useSound;

    public override void UseItemOnPlot(Plot plot)
    {
        if (plot.Occupied == false)
        {
            seed.NewPlant(plot);
            useSound.Play();
        }
    }
}
