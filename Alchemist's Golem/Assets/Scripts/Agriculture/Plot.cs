using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    public bool Occupied = false;
    public int[] QuantNutrients;
    public int WaterContent;

    public GameObject hightlight;

    public Crop crop;

    public bool ChangeNutrients(int nutrient, int quantity)
    {
        int tempNut = QuantNutrients[nutrient] + quantity;

        if (tempNut >= 0)
        {
            QuantNutrients[nutrient] = QuantNutrients[nutrient] + quantity;
            return true; // worked
        }
        else
        {
            return false; // failed
        }
    }

    public bool ChangeWaterContent(int waterQuant)
    {
        int tempWater = WaterContent + waterQuant;

        if (tempWater >= 0)
        {
            WaterContent = WaterContent + waterQuant;
            return true; // worked
        }
        else
        {
            return false; // failed
        }
    }

    public void Highlight(bool highlight)
    {
        hightlight.SetActive(highlight);
    }
}
