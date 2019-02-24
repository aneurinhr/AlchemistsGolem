using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject HighlightDisplay;
    public Text HighlightDescription;

    public SelectedItem selectedItem;
    public string inventoryTag = "Inventory";

    public int hotBarPointers;//999 mean no item
    public int hotBarQuant;//max stack 99
    public Image hotBarDisplay;//11
    public Text hotBarQuantDisplay;

    public bool inventSlot = true;

    private Button p_meButton;

    private void Start()
    {
        p_meButton = gameObject.GetComponent<Button>();
    }

    public void newValues(int newPointer, int newQuant)
    {
        hotBarPointers = newPointer;
        hotBarQuant = newQuant;
    }

    private void OnDisable()
    {
        HighlightDisplay.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightDisplay.SetActive(true);

        GameObject.FindGameObjectWithTag(inventoryTag).GetComponent<Inventory>().highlightedSlot = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightDisplay.SetActive(false);

        GameObject.FindGameObjectWithTag(inventoryTag).GetComponent<Inventory>().highlightedSlot = null;
    }

}
