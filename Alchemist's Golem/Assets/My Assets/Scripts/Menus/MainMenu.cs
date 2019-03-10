using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public GameObject Hotbar;

    public GameObject Player;
    public GameObject MenuCamera;

    public GameObject MainMenuUI;

    public void ToggleOn()
    {
        Hotbar.SetActive(false);
        Player.SetActive(false);

        MenuCamera.SetActive(true);
        MainMenuUI.SetActive(true);
    }

    public void ToggleOff()
    {
        Hotbar.SetActive(true);
        Player.SetActive(true);

        MenuCamera.SetActive(false);
        MainMenuUI.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting");
        Application.Quit();
    }

}
