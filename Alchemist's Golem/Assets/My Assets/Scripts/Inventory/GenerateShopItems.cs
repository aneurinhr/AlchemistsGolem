using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateShopItems : MonoBehaviour
{
    public GameObject itemTemplate;
    public GameObject parent;
    public ItemDatabase database;

    public float startY = -50.0f;
    public float yDif = 80.0f;

    private int numUsable = 0;

    void Start()
    {
        for (int i = 0; i < database.itemlist.Length; i++)
        {
            Item temp = database.GetItem(i);

            if (temp.usable == true)//if true it is also a buyable item
            {
                float y = startY - (yDif * numUsable);
                Vector3 newPosition = parent.transform.position + new Vector3(0, y, 0);

                GameObject shopItem = Instantiate(itemTemplate, newPosition, parent.transform.rotation);
                shopItem.transform.SetParent(parent.transform);
                shopItem.GetComponent<BuyItem>().Setup(temp.itemImage, temp.description, i, temp.PurchasePrice);

                shopItem.SetActive(true);

                numUsable = numUsable + 1;
            }
        }
    }

}
