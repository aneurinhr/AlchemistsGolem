using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public Item[] itemlist;

    public Item GetItem(int pointer)
    {
        if (pointer < itemlist.Length)
        {
            return itemlist[pointer];
        }

        return null;
    }
}
