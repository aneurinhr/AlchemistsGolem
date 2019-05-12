using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    public int maxStack = 99;

    public ItemDatabase database;
    public PlayerMovement playerMovement;
    public Player player;

    public float delay = 0.3f;
    public bool scroll = true;
    public bool canChange = true;

    public AudioSource scrollInteraction;
    public AudioSource openClose;

    public EventSystem m_EventSystem;

    public List<SlotSaveData> SaveInfo()
    {
        List<SlotSaveData> saveData = new List<SlotSaveData>();

        //Hotbar
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            SlotSaveData temp = hotbarSlots[i].SaveInfo();
            temp.ID[0] = i;
            temp.ID[1] = 999;
            saveData.Add(temp);
        }

        for (int i = 0; i < inventorySlotRows.Length; i++)
        {
            for (int k = 0; k < inventorySlotRows[i].rowSlots.Length; k++)
            {
                SlotSaveData temp = inventorySlotRows[i].rowSlots[k].SaveInfo();
                temp.ID[0] = i;
                temp.ID[1] = k;
                saveData.Add(temp);
            }
        }

        return saveData;
    }

    public void LoadInfo(List<SlotSaveData> info)
    {
        for (int j = 0; j < info.Count; j++)
        {
            int i = info[j].ID[0];
            int k = info[j].ID[1];

            if (k == 999)
            {
                hotbarSlots[i].LoadInfo(info[j]);
            }
            else
            {
                inventorySlotRows[i].rowSlots[k].LoadInfo(info[j]);
            }
        }

        UpdateInventory();
        highlightedSlot = null;
    }

    public void NewGame()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].ResetThis();
        }

        for (int i = 0; i < inventorySlotRows.Length; i++)
        {
            for (int k = 0; k < inventorySlotRows[i].rowSlots.Length; k++)
            {
                inventorySlotRows[i].rowSlots[k].ResetThis();
            }
        }

        UpdateInventory();
        highlightedSlot = null;
    }

    public void AddItem(int item, int quant)
    {
        int leftOver = quant;

        //Hotbar - looking to stack
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].hotBarPointers == item)
            {
                int temp = hotbarSlots[i].hotBarQuant;
                temp = temp + leftOver;

                if (temp > maxStack)//if over max stack
                {
                    hotbarSlots[i].hotBarQuant = maxStack;
                    leftOver = temp - maxStack;
                }
                else
                {
                    hotbarSlots[i].hotBarQuant = temp;
                    leftOver = 0;
                }
            }
        }
        //Hotbar - looking to stack

        if (leftOver > 0)
        {
            //Inventory - looking to stack
            for (int i = 0; i < inventorySlotRows.Length; i++)
            {
                for (int k = 0; k < inventorySlotRows[i].rowSlots.Length; k++) //inventorySlotRows[i].rowSlots[k]
                {
                    if (inventorySlotRows[i].rowSlots[k].hotBarPointers == item)
                    {
                        int temp = inventorySlotRows[i].rowSlots[k].hotBarQuant;
                        temp = temp + leftOver;

                        if (temp > maxStack)//if over max stack
                        {
                            inventorySlotRows[i].rowSlots[k].hotBarQuant = maxStack;
                            leftOver = temp - maxStack;
                        }
                        else
                        {
                            inventorySlotRows[i].rowSlots[k].hotBarQuant = temp;
                            leftOver = 0;
                        }
                    }
                }
            }
            //Inventory - looking to stack
        }

        //Here is for if fully stacked stuff

        if (leftOver > 0)
        {
            //Hotbar - looking for empty
            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                if (hotbarSlots[i].hotBarPointers == 999)
                {
                    hotbarSlots[i].hotBarPointers = item;
                    if (leftOver >= 99)
                    {
                        hotbarSlots[i].hotBarQuant = 99;
                        leftOver = leftOver - 99;
                    }
                    else
                    {
                        hotbarSlots[i].hotBarQuant = leftOver;
                        leftOver = 0;
                    }
                }
            }
            //Hotbar - looking for empty
        }

        if (leftOver > 0)
        {
            //Inventory - looking for empty
            for (int i = 0; i < inventorySlotRows.Length; i++)
            {
                for (int k = 0; k < inventorySlotRows[i].rowSlots.Length; k++) //inventorySlotRows[i].rowSlots[k]
                {
                    if (inventorySlotRows[i].rowSlots[k].hotBarPointers == 999)
                    {
                        inventorySlotRows[i].rowSlots[k].hotBarPointers = item;
                        if (leftOver >= 99)
                        {
                            inventorySlotRows[i].rowSlots[k].hotBarQuant = 99;
                            leftOver = leftOver - 99;
                        }
                        else
                        {
                            inventorySlotRows[i].rowSlots[k].hotBarQuant = leftOver;
                            leftOver = 0;
                        }
                    }
                }
            }
            //Inventory
        }

        UpdateInventory();
    }

    public void UsedSelectedItem()
    {
        int temp = hotbarSlots[highlightedHotbarSlot].hotBarQuant;
        hotbarSlots[highlightedHotbarSlot].hotBarQuant = temp - 1;

        if (hotbarSlots[highlightedHotbarSlot].hotBarQuant <= 0)
        {
            player.selectedItem = null;
        }

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
        //Inventory
    }

    public Item GetItemFromHotbars(int pointer)//Middle Man
    {
        return database.GetItem(hotbarSlots[pointer].hotBarPointers);
    }

    public Item GetItemFromInventory(int i, int k)//Middle Man
    {
        return database.GetItem(inventorySlotRows[i].rowSlots[k].hotBarPointers);
    }

    private void Update()
    {
        if ((Input.GetButtonDown("Inventory")) && (canChange == true))
        {
            inventoryOpen = !inventoryOpen;

            if (inventoryOpen == true)
            {
                uiON();
            }
            else
            {
                uiOFF();
            }

            openClose.Play();
        }

        Scroll();
    }

    public void uiON()
    {
        inventoryOpen = true;

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].HighlightDisplay.SetActive(false);
        }

        playerMovement.pauseMovement = true;
        inventoryUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        player.selectedItem = null;

        GameObject temp = hotbarSlots[0].gameObject;
        m_EventSystem.SetSelectedGameObject(temp);
    }

    public void uiOFF()
    {
        inventoryOpen = false;

        playerMovement.pauseMovement = false;
        inventoryUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Scroll()
    {
        if (inventoryOpen == false)
        {
            if (scroll == true)
            {
                bool change = false;
                //Button press - 11 slots
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    highlightedHotbarSlot = 0;
                    change = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    highlightedHotbarSlot = 1;
                    change = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    highlightedHotbarSlot = 2;
                    change = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    highlightedHotbarSlot = 3;
                    change = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    highlightedHotbarSlot = 4;
                    change = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    highlightedHotbarSlot = 5;
                    change = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    highlightedHotbarSlot = 6;
                    change = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    highlightedHotbarSlot = 7;
                    change = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    highlightedHotbarSlot = 8;
                    change = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    highlightedHotbarSlot = 9;
                    change = true;
                }
                else if (Input.GetKeyDown(KeyCode.Minus))
                {
                    highlightedHotbarSlot = 10;
                    change = true;
                }
                else //scroll if button not pressed
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

                        change = true;
                    }
                    else if (scrollWheel < 0f)
                    {
                        // scroll down
                        highlightedHotbarSlot = highlightedHotbarSlot - 1;

                        if (highlightedHotbarSlot < 0)
                        {
                            highlightedHotbarSlot = hotbarSlots.Length - 1;
                        }

                        change = true;
                    }
                }

                if (change == true)
                {
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

                    scrollInteraction.Play();
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
