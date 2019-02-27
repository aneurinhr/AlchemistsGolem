using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyItem : MonoBehaviour
{
    public Image myImage;
    public Text myText;

    public Inventory invent;
    public Bank bank;
    public int itemPointer;
    public int price;

    public AudioSource buySound;

    public void Setup(Sprite image, string text, int item, int value)
    {
        myImage.sprite = image;
        myText.text = text;
        itemPointer = item;
        price = value;
    }

    public void BuyItemOnPress()
    {
        bool worked = bank.ChangeMoney(-price);

        if (worked == true)
        {
            invent.AddItem(itemPointer, 1);
            buySound.Play();
        }
    }
}
