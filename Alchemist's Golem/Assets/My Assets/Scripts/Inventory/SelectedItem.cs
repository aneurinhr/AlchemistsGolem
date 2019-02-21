using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectedItem : MonoBehaviour
{
    public Slot selectedSlot;
    public Inventory inventory;

    public int tempSlotPointers = 999;
    public int tempSlotQuant = 0;

    public Image dragImage;
    public Camera myCamera;
    public Canvas myCanvas;

    private void Start()
    {
        dragImage = gameObject.GetComponent<Image>();
        dragImage.enabled = false;
    }

    public void newSelected(Slot newSlot)
    {
        dragImage.enabled = true;
        selectedSlot = newSlot;

        dragImage.sprite = selectedSlot.hotBarDisplay.sprite;
        tempSlotPointers = selectedSlot.hotBarPointers;
        tempSlotQuant = selectedSlot.hotBarQuant;
    }

    public void swapSlots(Slot toSwapWith)
    {
        selectedSlot.newValues(toSwapWith.hotBarPointers, toSwapWith.hotBarQuant);

        //After the other so it is not changed before hand
        toSwapWith.newValues(tempSlotPointers, tempSlotQuant);

        inventory.UpdateInventory();
        dragImage.enabled = false;
    }

    private void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
        transform.position = myCanvas.transform.TransformPoint(pos);

        if (inventory.inventoryOpen == true) {
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("PointerDown");
                if ((inventory.highlightedSlot != null) && (inventory.highlightedSlot.hotBarPointers != 999))
                {
                    newSelected(inventory.highlightedSlot);
                }
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                Debug.Log("PointerUp");
                if ((inventory.highlightedSlot != null) && (tempSlotPointers != 999))
                {
                    swapSlots(inventory.highlightedSlot);
                    tempSlotPointers = 999;
                    tempSlotQuant = 0;
                }
            }
        }
    }

}
