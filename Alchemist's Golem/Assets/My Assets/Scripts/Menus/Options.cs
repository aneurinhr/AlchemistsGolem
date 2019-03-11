using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public GameObject OptionsUI;
    public bool UIon = false;

    public void ToggleOn()
    {
        OptionsUI.SetActive(true);
        UIon = true;
    }

    public void ToggleOff()
    {
        OptionsUI.SetActive(false);
        UIon = false;
    }
}
