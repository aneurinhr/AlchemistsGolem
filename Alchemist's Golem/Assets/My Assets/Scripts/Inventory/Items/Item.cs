using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool usable;

    public int PurchasePrice = 0;
    public int sellPrice = 0;

    public virtual void UseItemOnPlot(Plot plot)
    {
        Debug.Log("Not meant to be here");
    }

}
