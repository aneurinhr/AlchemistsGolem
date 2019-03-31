using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMissionManager : MonoBehaviour
{
    public ItemDatabase itemDatabase;

    public int[] possibleRequestedItems;
    public string[] possiblePeopleRequesting;
    public Sprite[] peopleSprites;

    public int requestedItemRewardMulti = 3;
    public int maxWanted = 20;
    public int minWanted = 5;

    public SideMission[] sideMissionSlots;

    private void Start()
    {
        GenerateForAllQuestSlots();
    }

    public void GenerateNewQuest(int ID)
    {
        //Random person
        int randPerson = Random.Range(0, possiblePeopleRequesting.Length);
        string person = possiblePeopleRequesting[randPerson];
        Sprite personSprite = peopleSprites[randPerson];

        //Random Item
        int randItem = Random.Range(0, possibleRequestedItems.Length);
        int item = possibleRequestedItems[randItem];

        //Num of Item
        int numOfItem = Random.Range(minWanted, maxWanted);

        //Mission Reward
        Item actualItem = itemDatabase.GetItem(item);
        int itemSellPrice = actualItem.sellPrice;
        int basePrice = itemSellPrice * requestedItemRewardMulti;
        int randomPriceChange = Random.Range(-itemSellPrice, itemSellPrice);
        int reward = basePrice + randomPriceChange;
        reward = reward * numOfItem;

        sideMissionSlots[ID].PopulateMission(person, personSprite, numOfItem, item, reward);
    }

    public void GenerateForAllQuestSlots()
    {
        for (int i = 0; i < sideMissionSlots.Length; i++)
        {
            GenerateNewQuest(i);
        }
    }
}
