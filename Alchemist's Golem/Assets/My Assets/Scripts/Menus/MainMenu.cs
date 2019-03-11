using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public GameObject Hotbar;
    public GameObject Player;
    public GameObject MenuCamera;
    public GameObject MainMenuUI;

    public Inventory invent;
    public EscapeMenu escapeMenu;
    public TickAll dayChange;

    private void Start()
    {
        invent.scroll = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        escapeMenu.prevent = true;
        invent.canChange = false;
    }

    private void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleOn()
    {
        invent.canChange = false;
        invent.uiOFF();

        Hotbar.SetActive(false);
        Player.SetActive(false);

        MenuCamera.SetActive(true);
        MainMenuUI.SetActive(true);
        invent.scroll = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        escapeMenu.prevent = true;
        dayChange.pauseTimer = true;
    }

    public void ToggleOff()
    {
        Hotbar.SetActive(true);
        Player.SetActive(true);

        MenuCamera.SetActive(false);
        MainMenuUI.SetActive(false);
        invent.scroll = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        escapeMenu.prevent = false;

        invent.canChange = true;
        dayChange.pauseTimer = false;
    }

    public void ExitGame()
    {
        Debug.Log("Exiting");
        Application.Quit();
    }

}
