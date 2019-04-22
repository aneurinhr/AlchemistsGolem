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
    public Image highlightBorder;

    public Crop crop;

    public Slider frostSlider;
    public Slider heatSlider;
    public Slider windSlider;
    public Slider waterSlider;

    public PlotNutrientPointers pointers;

    public Color unoccupied;
    public Color harvest;
    public Color growing;
    public Color dead;

    public int randomMinWeed = 90;
    public int randomGoldWeed = 100;
    public CropMother Weed;
    public CropMother GoldWeed;

    public int SliderMax = 20;

    public PlotSaveData SaveInfo()
    {
        PlotSaveData saveData = new PlotSaveData();
        saveData.Occupied = Occupied;
        saveData.QuantNutrients = QuantNutrients;
        saveData.WaterContent = WaterContent;

        if (Occupied == true)
        {
            saveData.crop = crop.SaveInfo();
        }
        else
        {
            saveData.crop = "";
        }

        return saveData;
    }

    public void LoadInfo(string info)
    {

    }

    private void Start()
    {
        UpdateSliders();

        Weed.NewPlant(this);
    }

    public void WeedTick()
    {
        int rand = Random.Range(0, (randomGoldWeed + 1));

        if (rand >= randomMinWeed)
        {
            if (rand == randomGoldWeed)
            {
                GoldWeed.NewPlant(this);
            }
            else
            {
                Weed.NewPlant(this);
            }
        }
    }

    public void UpdateSliders()
    {
        frostSlider.value = QuantNutrients[0];
        heatSlider.value = QuantNutrients[1];
        windSlider.value = QuantNutrients[2];
        waterSlider.value = WaterContent;
    }

    public void Max()
    {
        QuantNutrients[0] = SliderMax;
        QuantNutrients[1] = SliderMax;
        QuantNutrients[2] = SliderMax;
        WaterContent = SliderMax;
    }

    public void Min()
    {
        QuantNutrients[0] = 0;
        QuantNutrients[1] = 0;
        QuantNutrients[2] = 0;
        WaterContent = 0;
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
        if (Occupied == true)
        {
            if (crop.canBeHarvested == true)
            {
                highlightBorder.color = harvest;
            }
            else if (crop.dead == true)
            {
                highlightBorder.color = dead;
            }
            else
            {
                highlightBorder.color = growing;
            }
        }
        else
        {
            highlightBorder.color = unoccupied;
        }

        hightlight.SetActive(highlight);
    }
}

public class PlotSaveData
{
    public int[] ID = { 999, 999 };

    public bool Occupied;
    public int[] QuantNutrients;
    public int WaterContent;

    public string crop;
}