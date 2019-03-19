using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenu : MonoBehaviour
{

    public GameObject EscapeMenuUI;
    public string EscapeButtonName = "";

    public bool UIon = false;
    public bool prevent = false;

    public Upgrades upgrades;
    public Inventory invent;
    public Storage storage;
    public Shop shop;

    public Options options;

    public PlayerMovement rb;

    public void ToggleOn()
    {
        rb.rb.isKinematic = true;
        invent.uiOFF();
        invent.canChange = false;
        storage.StorageOff();
        storage.canChange = false;
        shop.ShopOff();
        shop.canChange = false;
        upgrades.UIOff();
        upgrades.canChange = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0;

        EscapeMenuUI.SetActive(true);
        UIon = true;
    }

    public void ToggleOff()
    {
        rb.rb.isKinematic = false;
        invent.canChange = true;
        storage.canChange = true;
        shop.canChange = true;
        upgrades.canChange = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1;

        EscapeMenuUI.SetActive(false);
        UIon = false;

        options.ToggleOff();
    }

    void Update()
    {
        if ((Input.GetButtonDown(EscapeButtonName)) && (prevent == false))
        {
            if (UIon == true)
            {
                ToggleOff();
            }
            else
            {
                ToggleOn();
            }
        }
    }
}
