using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutrientChangeItem : Item
{
    public int nutrient = 100;
    public int nutrientChange = 0;

    public override void UseItemOnPlot(Plot plot)
    {
        plot.ChangeNutrients(nutrient, nutrientChange);
    }
}
