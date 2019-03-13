using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public bool beingLookedAt = false;
    public AudioSource openShop;

    public Inventory inventory;
    public GameObject highlight;
    public GameObject shop;

    public bool open = false;
    public bool canChange = true;

    private void Start()
    {
        shop.SetActive(false);//needs to be on in editor to generate buttons correctly
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
                shop.SetActive(true);
                inventory.uiON();
                inventory.canChange = false;
                openShop.Play();
            }
            else if (open == true)
            {
                ShopOff();
                inventory.uiOFF();
                openShop.Play();
            }
        }

        beingLookedAt = false;
    }

    public void ShopOff()
    {
        open = false;
        shop.SetActive(false);
        inventory.canChange = true;
    }
}
