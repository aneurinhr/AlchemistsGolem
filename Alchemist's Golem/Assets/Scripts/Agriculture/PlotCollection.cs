using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotCollection : MonoBehaviour
{

    public MapRow[] map;

    public int[] NutrientIncrease;
    public int WaterIncrease = 1;

    public int[] NutrientMax;
    public int WaterMax = 7;

    public int DiffuseLimit = 1;

    public void NaturalPlotIncrease()
    {
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[i].mapCol.Length; j++)
            {
                //Increase
                for (int k = 0; k < NutrientIncrease.Length; k++)
                {
                    if (map[i].mapCol[j].QuantNutrients[k] < NutrientMax[k])
                    {
                        map[i].mapCol[j].ChangeNutrients(k, NutrientIncrease[k]);
                    }
                }

                if (map[i].mapCol[j].WaterContent < WaterMax)
                {
                    map[i].mapCol[j].ChangeWaterContent(WaterIncrease);
                }
            }
        }
    }

    public void PlotDiffuse()
    {

    }
}

[System.Serializable]
public class MapRow
{
    public Plot[] mapCol;
}
