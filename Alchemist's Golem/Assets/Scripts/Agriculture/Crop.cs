using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    [SerializeField]//temp serialization
    private int p_currentTickToHarvest = 0;
    private int p_baseQuality;

    public CropMother mother;
    public int quality = 5; //Changes Based of how long it was dying
                            //And how well kept it was
                     
    public int currentPhase = 0;
    public GameObject[] phases;
    public GameObject deadPhase;
    public GameObject dyingPhase;
    public bool canBeHarvested;
    public bool dead;
    public bool dying;

    public int baseHarvestNum = 3;

    public Plot plotOn;

    //Inventory

    private void Start()
    {
        p_baseQuality = quality;

    }

    private void OnEnable()
    {
        p_currentTickToHarvest = 0;
        quality = p_baseQuality;

        currentPhase = mother.CurrentPhase(p_currentTickToHarvest);
        //Disable all apart from correct phase
    }

    //This will edit p_quality, detect if the plant is dying or dead
    //as well as changing the currentPhase gameobject (what it looks like)
    //Should also take nutrients and water from the plot
    public void Tick()
    {
        bool healthy = false;

        //Get nutrients and water from plot using values in Mother

        //Check if unhealthy

        //If quality hits 0 plant is dead

        if (healthy == true)// if healthy tick up and check phase
        {
            p_currentTickToHarvest = p_currentTickToHarvest + 1;
            currentPhase = mother.CurrentPhase(p_currentTickToHarvest);

            //Disable all apart from correct phase

            //if last stage, become harvestable
        }
    }

    public void Harvest() //Should directly deposite to inventory
    {
        int num = baseHarvestNum;
        num = num + quality;//Add quality to up or decrease the num harvested

        Debug.Log("Harvest " + gameObject.name + " for " + num);
        gameObject.SetActive(false);
    }
}
