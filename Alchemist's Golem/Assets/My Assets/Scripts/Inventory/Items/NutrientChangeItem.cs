using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutrientChangeItem : Item
{
    public int nutrient = 100;
    public int nutrientChange = 0;
    public AudioSource useSound;

    public override bool UseItemOnPlot(Plot plot)
    {
        int plotNutrient = plot.QuantNutrients[nutrient];

        if (plotNutrient < 10)
        {
            plot.ChangeNutrients(nutrient, nutrientChange);
            useSound.Play();
            return true;
        }
        return false;
    }
}
