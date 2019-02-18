using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickAll : MonoBehaviour
{
    public string tag = "Crop";

    public PlotCollection[] plots;

    void Update()
    {
        if (Input.GetKey("left ctrl") && Input.GetKeyDown("d"))
        {
            TickForAll();
            //UpdatePlots();
        }
    }

    public void UpdatePlots()
    {
        for (int i = 0; i < plots.Length; i++)
        {
            plots[i].NaturalPlotIncrease();
            //plots[i].PlotDiffuse();
        }
    }

    public void TickForAll()
    {
        GameObject[] plants = GameObject.FindGameObjectsWithTag(tag);

        for (int i = 0; i < plants.Length; i++)
        {
            plants[i].GetComponent<Crop>().Tick();
        }
    }
}
