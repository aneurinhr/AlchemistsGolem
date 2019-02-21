using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    //Hotbar
    public Slot[] hotbarSlots;
    public int highlightedHotbarSlot = 0;
    //Hotbar

    //Invent
    public SlotRow[] inventorySlotRows;
    public int rowsActive = 1;
    //Invent

    public Slot highlightedSlot = null;
    public GameObject inventoryUI;
    public bool inventoryOpen = false;

    public ItemDatabase database;
    public PlayerMovement playerMovement;
    public Player player;

    public float delay = 0.3f;
    private bool scroll = true;

    private void Start()
    {
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        //Hotbar
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].hotBarQuant <= 0)
            {
                hotbarSlots[i].hotBarPointers = 999;
            }

            if (hotbarSlots[i].hotBarPointers == 999)
            {
                hotbarSlots[i].HighlightDescription.text = "";
                hotbarSlots[i].hotBarDisplay.gameObject.SetActive(false);
                hotbarSlots[i].hotBarQuantDisplay.gameObject.SetActive(false);
            }
            else
            {
                Sprite temp = null;
                Item forSprite = GetItemFromHotbars(i);
                temp = forSprite.itemImage;

                hotbarSlots[i].HighlightDescription.text = forSprite.description;
                hotbarSlots[i].hotBarDisplay.sprite = temp;
                hotbarSlots[i].hotBarQuantDisplay.text = hotbarSlots[i].hotBarQuant.ToString();
                hotbarSlots[i].hotBarDisplay.gameObject.SetActive(true);
                hotbarSlots[i].hotBarQuantDisplay.gameObject.SetActive(true);
            }
        }
        //Hotbar

        //Inventory
        //INSERT CODE
        //Inventory
    }

    public Item GetItemFromHotbars(int pointer)//Middle Man
    {
        return database.GetItem(hotbarSlots[pointer].hotBarPointers);
    }

    public Item GetItemFromInventory(int rowPointer, int colPointer)//Middle Man
    {
        return null;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            inventoryOpen = !inventoryOpen;

            if (inventoryOpen == true)
            {
                for (int i = 0; i < hotbarSlots.Length; i++)
                {
                    hotbarSlots[i].HighlightDisplay.SetActive(false);
                }

                playerMovement.pauseMovement = true;
                inventoryUI.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;

                player.selectedItem = null;
            }
            else
            {
                playerMovement.pauseMovement = false;
                inventoryUI.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        Scroll();
    }

    private void Scroll()
    {
        if (inventoryOpen == false)
        {
            if (scroll == true)
            {
                float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

                if (scrollWheel > 0f)
                {
                    // scroll up
                    highlightedHotbarSlot = highlightedHotbarSlot + 1;

                    if (highlightedHotbarSlot >= hotbarSlots.Length)
                    {
                        highlightedHotbarSlot = 0;
                    }

                    for (int i = 0; i < hotbarSlots.Length; i++)
                    {
                        if (highlightedHotbarSlot == i)
                        {
                            hotbarSlots[highlightedHotbarSlot].HighlightDisplay.SetActive(true);
                            player.selectedItem = GetItemFromHotbars(highlightedHotbarSlot);
                        }
                        else
                        {
                            hotbarSlots[i].HighlightDisplay.SetActive(false);
                        }
                    }
                }
                else if (scrollWheel < 0f)
                {
                    // scroll down
                    highlightedHotbarSlot = highlightedHotbarSlot - 1;

                    if (highlightedHotbarSlot < 0)
                    {
                        highlightedHotbarSlot = hotbarSlots.Length - 1;
                    }

                    for (int i = 0; i < hotbarSlots.Length; i++)
                    {
                        if (highlightedHotbarSlot == i)
                        {
                            hotbarSlots[highlightedHotbarSlot].HighlightDisplay.SetActive(true);
                            player.selectedItem = GetItemFromHotbars(highlightedHotbarSlot);
                        }
                        else
                        {
                            hotbarSlots[i].HighlightDisplay.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    IEnumerator WaitForScroll()
    {
        scroll = false;

        yield return new WaitForSeconds(delay);

        scroll = true;
    }
}

[System.Serializable]
public class SlotRow
{
    public Slot[] rowSlots;
}
