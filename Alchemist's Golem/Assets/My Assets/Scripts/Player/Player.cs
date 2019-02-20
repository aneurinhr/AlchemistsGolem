using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera look;
    public float maxLookDistance = 5.0f;

    public int money;
    //public Inventory inventory
    public bool isPlotHightlighted = false;
    public Plot highlightedPlot = null;

    //TEMP whilst no inventory in place
    public CropMother tempPlanting;
    //TEMP

    public void UseItemOnPlot()
    {
        //Uses default item, change when inventory is introduced. 
        tempPlanting.NewPlant(highlightedPlot);
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
        Look();

        if (Input.GetButtonUp("Fire1"))
        {
            if (isPlotHightlighted == true)//And valid item is selected
            {
                if (highlightedPlot.Occupied == false)
                {
                    UseItemOnPlot();
                }
                else if (highlightedPlot.Occupied == true)
                {
                    Harvest();
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
