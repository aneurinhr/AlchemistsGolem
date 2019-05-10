using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    [SerializeField]//temp serialization
    private int p_currentTickToHarvest = 0;
    [SerializeField]
    private int p_baseQuality = 5;//make sure this the the value to keep

    public CropMother mother;
    public int quality = 5; //Changes Based of how long it was dying
                            //And how well kept it was
                     
    public int currentPhase = 0;
    public GameObject[] phases;
    public GameObject deadPhase;
    public GameObject dyingPhase;
    public bool canBeHarvested;
    public bool dead;
    public int baseHarvestNum = 3;

    public Plot plotOn;

    public string SaveInfo()
    {
        CropSaveData saveData = new CropSaveData();
        saveData.quality = quality;
        saveData.currentTickToHarvest = p_currentTickToHarvest;
        saveData.plantType = mother.name;//to id plant it is
        saveData.unhealthy = dyingPhase.activeSelf;

        string temp = JsonUtility.ToJson(saveData);

        return temp;
    }

    public void LoadInfo(CropSaveData info)
    {
        quality = info.quality;
        int tick = info.currentTickToHarvest;

        ForceGrow(tick, info.unhealthy);
    }

    private void OnEnable()
    {
        p_currentTickToHarvest = 0;
        currentPhase = 0;
        quality = p_baseQuality;

        canBeHarvested = false;
        dead = false;
        //Disable all apart from correct phase
        for (int i = 0; i < phases.Length; i++)
        {
            if (currentPhase == i)
            {
                phases[i].SetActive(true);
            }
            else
            {
                phases[i].SetActive(false);
            }
        }

        //if last stage, become harvestable
        if (currentPhase == (phases.Length - 1))
        {
            canBeHarvested = true;
        }
    }

    public void ForceFullGrow()
    {
        ForceGrow(999, false);
    }

    public void ForceGrow(int phase, bool unhealthy)
    {
        p_currentTickToHarvest = phase;

        if ((unhealthy == false) &&(dead == false))
        {
            //If quality hits 0 plant is dead
            if (quality <= 0)
            {
                dead = true;
                canBeHarvested = false;

                for (int i = 0; i < phases.Length; i++)// disable all
                {
                    phases[i].SetActive(false);
                }

                dyingPhase.SetActive(false);
                deadPhase.SetActive(true);
            }

            int oldPhase = currentPhase;
            currentPhase = mother.CurrentPhase(p_currentTickToHarvest);

            dyingPhase.SetActive(false);

            //Disable all apart from correct phase
            for (int i = 0; i < phases.Length; i++)
            {
                if (currentPhase == i)
                {
                    phases[i].SetActive(true);
                }
                else
                {
                    phases[i].SetActive(false);
                }
            }

            //if last stage, become harvestable
            if (currentPhase >= (phases.Length - 1))
            {
                currentPhase = (phases.Length - 1);
                canBeHarvested = true;
            }



        }
        else if (unhealthy == true)// unhealthy
        {
            for (int i = 0; i < phases.Length; i++)// disable all
            {
                phases[i].SetActive(false);
            }
            deadPhase.SetActive(false);
            dyingPhase.SetActive(true);
        }
    }

    //This will edit p_quality, detect if the plant is dying or dead
    //as well as changing the currentPhase gameobject (what it looks like)
    //Should also take nutrients and water from the plot
    public void Tick(int growth)
    {
        if (dead == false)
        {
            bool healthy = true;//to check if healthy

            //Get nutrients and water from plot using values in Mother
            bool neWater = plotOn.ChangeWaterContent(mother.RequiredWaterContent);//water

            int[] NutrientsNeeded = mother.NutrientsNeeded;
            int[] NutrientQuant = mother.QuantityNutrients;

            bool neNutrients = true;

            for (int i = 0; i < NutrientsNeeded.Length; i++)//nutrients
            {
                bool temp = plotOn.ChangeNutrients(NutrientsNeeded[i], NutrientQuant[i]);

                if (temp == false)
                {
                    neNutrients = false;
                }
            }

            if ((neNutrients == false) || (neWater == false))
            {
                healthy = false;
                quality = quality - 1;
            }

            //If quality hits 0 plant is dead
            if (quality <= 0)
            {
                dead = true;
                canBeHarvested = false;

                for (int i = 0; i < phases.Length; i++)// disable all
                {
                    phases[i].SetActive(false);
                }

                dyingPhase.SetActive(false);
                deadPhase.SetActive(true);
            }


            // if healthy tick up and check phase
            if (healthy == true)
            {
                if (canBeHarvested != true)
                {
                    p_currentTickToHarvest = p_currentTickToHarvest + growth;
                }

                int oldPhase = currentPhase;
                currentPhase = mother.CurrentPhase(p_currentTickToHarvest);

                dyingPhase.SetActive(false);

                //Disable all apart from correct phase
                for (int i = 0; i < phases.Length; i++)
                {
                    if (currentPhase == i)
                    {
                        phases[i].SetActive(true);
                    }
                    else
                    {
                        phases[i].SetActive(false);
                    }
                }

                //if last stage, become harvestable
                if (currentPhase >= (phases.Length - 1))
                {
                    currentPhase = (phases.Length - 1);
                    canBeHarvested = true;
                }

            }
            else if ((healthy == false) && (dead == false))// unhealthy
            {
                for (int i = 0; i < phases.Length; i++)// disable all
                {
                    phases[i].SetActive(false);
                }

                dyingPhase.SetActive(true);
            }
        }//not dead

        if (canBeHarvested == true)
        {
            for (int i = 0; i < phases.Length; i++)// disable all
            {
                phases[i].SetActive(false);
            }

            dyingPhase.SetActive(false);
            deadPhase.SetActive(false);

            phases[phases.Length - 1].SetActive(true);
        }
    }

    public bool Harvest() //Should directly deposite to inventory
    {
        if (canBeHarvested == true)
        {
            int num = baseHarvestNum;
            num = num + quality;//Add quality to up or decrease the num harvested

            mother.NewHarvest(num);

            plotOn.crop = null;
            plotOn.Occupied = false;
            gameObject.SetActive(false);

            return true;
        }
        else if (dead == true)
        {
            plotOn.crop = null;
            plotOn.Occupied = false;
            gameObject.SetActive(false);
        }

        return false;
    }
}

public class CropSaveData
{
    public string plantType;
    public int quality;
    public int currentTickToHarvest;
    public bool unhealthy;
}
