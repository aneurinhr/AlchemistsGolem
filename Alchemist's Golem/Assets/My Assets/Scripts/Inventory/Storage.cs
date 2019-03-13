using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    //storage
    public SlotRow[] inventorySlotRows;

    public GameObject highlight;
    public GameObject storageUI;
    public bool storageOpen = false;
    public bool beingLookedAt = false;

    public ItemDatabase database;
    public Inventory inventory;

    public AudioSource openChest;

    public bool canChange = true;

    private void Start()
    {
        UpdateStorage();
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
            if ((canChange == true) && (inventory.inventoryOpen == false) && (storageOpen == false) && (beingLookedAt == true))
            {
                //Open invent and storage
                storageOpen = true;
                storageUI.SetActive(true);
                inventory.uiON();
                inventory.canChange = false;
                openChest.Play();
            }
            else if (storageOpen == true)
            {
                //Close invent and storage
                StorageOff();
                inventory.uiOFF();
                openChest.Play();
            }
        }

        beingLookedAt = false;
    }

    public void StorageOff()
    {
        //Close invent and storage
        storageOpen = false;
        storageUI.SetActive(false);
        inventory.canChange = true;
    }

    public void UpdateStorage()
    {
        //Same as Inventory
        for (int i = 0; i < inventorySlotRows.Length; i++)
        {
            for (int k = 0; k < inventorySlotRows[i].rowSlots.Length; k++) //inventorySlotRows[i].rowSlots[k]
            {
                if (inventorySlotRows[i].rowSlots[k].hotBarQuant <= 0)
                {
                    inventorySlotRows[i].rowSlots[k].hotBarPointers = 999;
                }

                if (inventorySlotRows[i].rowSlots[k].hotBarPointers == 999)
                {
                    inventorySlotRows[i].rowSlots[k].HighlightDescription.text = "";
                    inventorySlotRows[i].rowSlots[k].hotBarDisplay.gameObject.SetActive(false);
                    inventorySlotRows[i].rowSlots[k].hotBarQuantDisplay.gameObject.SetActive(false);
                }
                else
                {
                    Sprite temp = null;
                    Item forSprite = GetItemFromInventory(i, k);
                    temp = forSprite.itemImage;

                    inventorySlotRows[i].rowSlots[k].HighlightDescription.text = forSprite.description;
                    inventorySlotRows[i].rowSlots[k].hotBarDisplay.sprite = temp;
                    inventorySlotRows[i].rowSlots[k].hotBarQuantDisplay.text = inventorySlotRows[i].rowSlots[k].hotBarQuant.ToString();
                    inventorySlotRows[i].rowSlots[k].hotBarDisplay.gameObject.SetActive(true);
                    inventorySlotRows[i].rowSlots[k].hotBarQuantDisplay.gameObject.SetActive(true);
                }
            }
        }
        //Same as Inventory
    }

    public Item GetItemFromInventory(int i, int k)//Middle Man
    {
        return database.GetItem(inventorySlotRows[i].rowSlots[k].hotBarPointers);
    }
}
