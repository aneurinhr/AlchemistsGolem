using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickAll : MonoBehaviour
{
    public string tag = "Crop";

    void Update()
    {
        if (Input.GetKey("left ctrl") && Input.GetKeyDown("d"))
        {
            TickForAll();
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
