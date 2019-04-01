using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionBoardInteractable : MonoBehaviour
{
    public bool beingLookedAt = false;
    public AudioSource openMissions;

    public Inventory inventory;
    public GameObject highlight;
    public GameObject missions;

    public bool open = false;
    public bool canChange = true;

    private void Start()
    {
        missions.SetActive(false);//needs to be on in editor to generate buttons correctly
    }

    public void BeingLookedAt()
    {
        highlight.SetActive(true);
        beingLookedAt = true;
    }

    private void Update()
    {
        highlight.SetActive(false);
    }

    private void LateUpdate()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if ((canChange == true) && (inventory.inventoryOpen == false) && (open == false) && (beingLookedAt == true))
            {
                open = true;
                missions.SetActive(true);
                inventory.uiON();
                inventory.canChange = false;
            }
            else if (open == true)
            {
                MissionsOff();
                inventory.uiOFF();
            }
        }

        beingLookedAt = false;
    }

    public void MissionsOff()
    {
        open = false;
        missions.SetActive(false);
        inventory.canChange = true;
    }
}
