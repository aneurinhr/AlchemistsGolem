using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotCollection : MonoBehaviour
{

    public MapRow[] map;

    public int[] NutrientIncrease;
    public int WaterIncrease = 1;
    public int growth = 1;

    //Upgrades
    public bool[] UpgradesUnlocked;

    public int[] UpgradeNutrientValues;
    public int UpgradeWaterValues = 4;
    //Upgrades

    public int[] NutrientMax;
    public int WaterMax = 7;

    public int DiffuseLimit = 1;
    public int minDifToDiffuse = 2;

    // This is to allow the upgrade buttons to be a lot more modular
    //Which allows for future additions easier
    public void Upgrade(int i)
    {
        UpgradesUnlocked[i] = true;

        switch (i)//0-4 e.g. 5 upgrades
        {
            case 0:
                NutrientIncrease[0] = UpgradeNutrientValues[0];
                break;
            case 1:
                NutrientIncrease[1] = UpgradeNutrientValues[1];
                break;
            case 2:
                NutrientIncrease[2] = UpgradeNutrientValues[2];
                break;
            case 3:
                WaterIncrease = UpgradeWaterValues;
                break;
            case 4:
                growth = 2;
                break;
        }
    }

    public void TickAll()
    {
        for (int i = 0; i < map.Length; i++)//row
        {
            for (int j = 0; j < map[i].mapCol.Length; j++)//col
            {
                if (map[i].mapCol[j].crop != null)
                {
                    map[i].mapCol[j].crop.Tick(growth);
                }
            }
        }
    }

    public void NaturalPlotIncrease()
    {
        //Check for each plot in the map
        for (int i = 0; i < map.Length; i++)//row
        {
            for (int j = 0; j < map[i].mapCol.Length; j++)//col
            {
                //For each nutrient
                for (int k = 0; k < NutrientIncrease.Length; k++)
                {
                    if (map[i].mapCol[j].QuantNutrients[k] <= NutrientMax[k])//if not greater than of equal to max limit
                    {
                        map[i].mapCol[j].ChangeNutrients(k, NutrientIncrease[k]);//increase
                    }
                }

                //water
                if (map[i].mapCol[j].WaterContent <= WaterMax)//if not greater than of equal to max limit
                {
                    map[i].mapCol[j].ChangeWaterContent(WaterIncrease);//increase
                }
            }
        }
    }

    public void PlotDiffuse()
    {
        for (int i = 0; i < map.Length; i++)//Row
        {
            for (int j = 0; j < map[i].mapCol.Length; j++)//Col
            {
                WaterDiffuse(i, j);

                for (int val = 0; val < map[i].mapCol[j].QuantNutrients.Length; val++)//Nutrient diffuse for each nutrient
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
