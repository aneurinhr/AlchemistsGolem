using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plot : MonoBehaviour
{
    public bool Occupied = false;
    public int[] QuantNutrients;
    public int WaterContent;

    public GameObject hightlight;

    public Crop crop;

    public Slider frostSlider;
    public Slider heatSlider;
    public Slider windSlider;
    public Slider waterSlider;

    public PlotNutrientPointers pointers;

    public void UpdateSliders()
    {
        frostSlider.value = QuantNutrients[0];
        heatSlider.value = QuantNutrients[1];
        windSlider.value = QuantNutrients[2];
        waterSlider.value = WaterContent;
    }

    public void NewCrop(int[] nutrients, int[] nutrientQuants, int water)
    {
        pointers.ChangePosition(water, 3);

        for (int i = 0; i < nutrients.Length; i++)
        {
            pointers.ChangePosition(nutrientQuants[i], nutrients[i]);
        }
    }

    public void NoCrop()
    {
        pointers.Deactivate();
    }

    private void Start()
    {
        UpdateSliders();
    }

    public bool ChangeNutrients(int nutrient, int quantity)
    {
        int tempNut = QuantNutrients[nutrient] + quantity;

        if (tempNut >= 0)
        {
            QuantNutrients[nutrient] = QuantNutrients[nutrient] + quantity;
            UpdateSliders();
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
            UpdateSliders();
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
