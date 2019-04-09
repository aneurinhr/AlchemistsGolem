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

    public AudioSource interact;

    public MainMissions missions;

    private void OnEnable()
    {
        missions.StartNewMission(0);
    }

    public void UseItemOnPlot()
    {
        if (selectedItem.usable == true)
        {
            bool used = selectedItem.UseItemOnPlot(highlightedPlot);

            if (used == true) { inventory.UsedSelectedItem(); }
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
            interact.Play();
        }
    }

    private void LateUpdate()
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
            Debug.DrawRay(ray.origin, ray.direction* maxLookDistance, Color.blue);

            GameObject lookingAt = hit.collider.gameObject;
            if (hit.distance <= maxLookDistance) {
                if (lookingAt.tag == "Chest")
                {
                    lookingAt.GetComponent<Storage>().BeingLookedAt();
                }
                else if (lookingAt.tag == "Shop")
                {
                    lookingAt.GetComponent<Shop>().BeingLookedAt();
                }
                else if (lookingAt.tag == "Charging")
                {
                    lookingAt.GetComponent<TickAll>().BeingLookedAt();
                }
                else if (lookingAt.tag == "Upgrades")
                {
                    lookingAt.GetComponent<Upgrades>().BeingLookedAt();
                }
                else if (lookingAt.tag == "Missions")
                {
                    lookingAt.GetComponent<MissionBoardInteractable>().BeingLookedAt();
                }
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
