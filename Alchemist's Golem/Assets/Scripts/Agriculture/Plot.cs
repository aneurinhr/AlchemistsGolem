using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    public bool Occupied = false;
    public int[] QuantNutrients;
    public int WaterContent;

    public GameObject hightlight;

    public void ChangeNutrients(int nutrient, int quantity)
    {
        QuantNutrients[nutrient] = quantity;
    }

    public void ChangeWaterContent(int waterQuant)
    {
        WaterContent = WaterContent + waterQuant;
    }

    public void Highlight(bool highlight)
    {
        hightlight.SetActive(highlight);
    }
}
