using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera look;
    public float maxLookDistance = 5.0f;

    public int money;
    public Inventory inventory;
    public bool isPlotHightlighted = false;
    public Plot highlightedPlot = null;

    public Item selectedItem = null;
    public bool pause = false;

    public void UseItemOnPlot()
    {
        if (selectedItem.usable == true)
        {
            selectedItem.UseItemOnPlot(highlightedPlot);
            inventory.UsedSelectedItem();
        }
    }

    public void Harvest()
    {
        bool tryHarvestCrop = highlightedPlot.crop.Harvest();

        if (tryHarvestCrop == false)
        {
            Debug.Log("Failed Harvest");
        }
        else
        {
            highlightedPlot.NoCrop();
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown("space"))
        //{
        //    inventory.AddItem(1, 200);
        //}

        if (pause == false)
        {
            Look();

            if (Input.GetButtonUp("Fire1"))
            {
                if (isPlotHightlighted == true)//If plot is highlighted
                {
                    if (selectedItem != null)//if item is selected try using it
                    {
                        UseItemOnPlot();
                    }
                    else if (highlightedPlot.Occupied == true)//else try to harvest
                    {
                        Harvest();
                    }
                }
            }
        }
    }

    private void Look()
    {
        Ray ray = look.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject lookingAt = hit.collider.gameObject;

            if ((lookingAt.GetComponent<Storage>()) && (hit.distance <= maxLookDistance))
            {
                lookingAt.GetComponent<Storage>().BeingLookedAt();
            }

            if (highlightedPlot != null)
            {
                highlightedPlot.Highlight(false);
                isPlotHightlighted = false;
            }

            if (lookingAt.GetComponent<Plot>() && (hit.distance <= maxLookDistance))
            {
                highlightedPlot = lookingAt.GetComponent<Plot>();
                highlightedPlot.Highlight(true);
                isPlotHightlighted = true;
            }
        }
    }

}
