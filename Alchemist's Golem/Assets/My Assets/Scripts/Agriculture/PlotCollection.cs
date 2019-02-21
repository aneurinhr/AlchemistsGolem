﻿using System.Collections;
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
    public int minDifToDiffuse = 2;

    public void NaturalPlotIncrease()
    {
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[i].mapCol.Length; j++)
            {
                //For each nutrient
                for (int k = 0; k < NutrientIncrease.Length; k++)
                {
                    if (map[i].mapCol[j].QuantNutrients[k] <= NutrientMax[k])
                    {
                        map[i].mapCol[j].ChangeNutrients(k, NutrientIncrease[k]);
                    }
                }

                if (map[i].mapCol[j].WaterContent <= WaterMax)
                {
                    map[i].mapCol[j].ChangeWaterContent(WaterIncrease);
                }
            }
        }
    }

    public void PlotDiffuse()
    {
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[i].mapCol.Length; j++)//Each plot should only take.  Start from highest
            {
                WaterDiffuse(i, j);

                for (int val = 0; val < map[i].mapCol[j].QuantNutrients.Length; val++)
                {
                    NutrientDiffuse(i, j, val);
                }
            }
        }
    }

    //Nutrient stuff \/\/\/
    public void NutrientDiffuse(int i, int j, int nutrientVal)
    {
        int[] ND = { 0, 0, 0, 0 };

        //Compare plot to each neighbour
        if (j > 0)
        {
            ND[0] = CompareNeighboursNutrient(map[i].mapCol[j], map[i].mapCol[j - 1], nutrientVal);

            if (ND[0] >= minDifToDiffuse)//Diffuse
            {
                map[i].mapCol[j].ChangeNutrients(nutrientVal, 1);
                map[i].mapCol[j - 1].ChangeNutrients(nutrientVal, -1);
            }
        }

        if (i > 0)
        {
            ND[1] = CompareNeighboursNutrient(map[i].mapCol[j], map[i - 1].mapCol[j], nutrientVal);

            if (ND[1] >= minDifToDiffuse)//Diffuse
            {
                map[i].mapCol[j].ChangeNutrients(nutrientVal, 1);
                map[i - 1].mapCol[j].ChangeNutrients(nutrientVal, -1);
            }
        }

        if (j < (map[i].mapCol.Length - 1))
        {
            ND[2] = CompareNeighboursNutrient(map[i].mapCol[j], map[i].mapCol[j + 1], nutrientVal);

            if (ND[2] >= minDifToDiffuse)//Diffuse
            {
                map[i].mapCol[j].ChangeNutrients(nutrientVal, 1);
                map[i].mapCol[j + 1].ChangeNutrients(nutrientVal, -1);
            }
        }

        if (i < (map.Length - 1))
        {
            ND[3] = CompareNeighboursNutrient(map[i].mapCol[j], map[i + 1].mapCol[j], nutrientVal);

            if (ND[3] >= minDifToDiffuse)//Diffuse
            {
                map[i].mapCol[j].ChangeNutrients(nutrientVal, 1);
                map[i + 1].mapCol[j].ChangeNutrients(nutrientVal, -1);
            }
        }
    }

    public int CompareNeighboursNutrient(Plot main, Plot neighbour, int nutrientVal)
    {
        int result;
        result = neighbour.QuantNutrients[nutrientVal] - main.QuantNutrients[nutrientVal];

        return result;
    }
    //Nutrient stuff /\/\/\

    //Water stuff \/\/\/
    public void WaterDiffuse(int i, int j)
    {
        int[] WD = { 0, 0, 0, 0 };

        //Compare plot to each neighbour
        if (j > 0)
        {
            WD[0] = CompareNeighboursWater(map[i].mapCol[j], map[i].mapCol[j - 1]);

            if (WD[0] >= minDifToDiffuse)//Diffuse
            {
                map[i].mapCol[j].ChangeWaterContent(1);
                map[i].mapCol[j - 1].ChangeWaterContent(-1);
            }
        }

        if (i > 0)
        {
            WD[1] = CompareNeighboursWater(map[i].mapCol[j], map[i - 1].mapCol[j]);

            if (WD[1] >= minDifToDiffuse)//Diffuse
            {
                map[i].mapCol[j].ChangeWaterContent(1);
                map[i - 1].mapCol[j].ChangeWaterContent(-1);
            }
        }

        if (j < (map[i].mapCol.Length - 1))
        {
            WD[2] = CompareNeighboursWater(map[i].mapCol[j], map[i].mapCol[j + 1]);

            if (WD[2] >= minDifToDiffuse)//Diffuse
            {
                map[i].mapCol[j].ChangeWaterContent(1);
                map[i].mapCol[j + 1].ChangeWaterContent(-1);
            }
        }

        if (i < (map.Length - 1))
        {
            WD[3] = CompareNeighboursWater(map[i].mapCol[j], map[i + 1].mapCol[j]);

            if (WD[3] >= minDifToDiffuse)//Diffuse
            {
                map[i].mapCol[j].ChangeWaterContent(1);
                map[i + 1].mapCol[j].ChangeWaterContent(-1);
            }
        }
    }

    public int CompareNeighboursWater(Plot main, Plot neighbour)
    {
        int result;
        result = neighbour.WaterContent - main.WaterContent;

        return result;
    }
    //Water stuff /\/\/\

}

[System.Serializable]
public class MapRow
{
    public Plot[] mapCol;
}