using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    public Bank bank;
    public TickAll tickAll;

    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.F1))//Money
            {
                bank.ChangeMoney(10000);
            }
            else if (Input.GetKeyDown(KeyCode.F2))//Tick
            {
                tickAll.TickForAll();
                tickAll.UpdatePlots();
            }
        }
    }
}
