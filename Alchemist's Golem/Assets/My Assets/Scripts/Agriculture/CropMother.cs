using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropMother : MonoBehaviour
{
    public int[] TicksPerPhase;

    public int[] NutrientsNeeded;
    public int[] QuantityNutrients;
    public int RequiredWaterContent;
    public int RequirementsSensitivity = 3; //Used to see how much less nutrients it
                                            //can take and still grow

    public GameObject Crop;//To spawn new crops
    public int cropPoolSize = 50;
    private List<GameObject> p_cropPool;//As a list for overflow

    public int harvestItemID = 999;
    public Inventory inventory;

    public int harvestLimit = 100;

    public void Awake()
    {
        //initialize crop pool
        p_cropPool = new List<GameObject>();
        for (int i = 0; i < cropPoolSize; i++)
        {
            GameObject tempCrop = (GameObject)Instantiate(Crop);

            p_cropPool.Add(tempCrop);
        }
    }

    public void NewHarvest(int quant)
    {
        int temp = quant;

        if (temp > harvestLimit)
        {
            temp = harvestLimit;
        }

        inventory.AddItem(harvestItemID, temp);
    }

    public void NewPlant(Plot plantingArea) //Change from uml as the crop is not information the player script needs.
    {
        for (int i = 0; i < p_cropPool.Count; i++)
        {
            if (p_cropPool[i].activeSelf == false)
            {
                plantingArea.Occupied = true;
                p_cropPool[i].transform.position = plantingArea.transform.position;
                p_cropPool[i].GetComponent<Crop>().plotOn = plantingArea;
                p_cropPool[i].GetComponent<Crop>().mother = this;
                plantingArea.crop = p_cropPool[i].GetComponent<Crop>();
                plantingArea.NewCrop(NutrientsNeeded, QuantityNutrients, RequiredWaterContent);

                p_cropPool[i].SetActive(true);

                break;
            }
        }
    }

    public int CurrentPhase(int tick)
    {

        for (int i = 0; i < TicksPerPhase.Length; i++)
        {
            if (tick <= TicksPerPhase[i])
            {
                return i;
            }
        }
        Debug.Log("Hit here so full grown");
        return TicksPerPhase[TicksPerPhase.Length - 1];
    }

}
