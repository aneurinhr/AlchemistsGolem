using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public int rejectionCount = 3;
    public int rejectionMax = 3;
    public Text rejectionDisplay;

    public SideMissionManagerSaveData SaveGame()
    {
        SideMissionManagerSaveData saveData = new SideMissionManagerSaveData();

        for (int i = 0; i < sideMissionSlots.Length; i++)
        {
            SideMissionSaveData temp = sideMissionSlots[i].SaveGame();
            saveData.sideMissionSaveData.Add(JsonUtility.ToJson(temp));
        }

        saveData.rejectionCount = rejectionCount;

        return saveData;
    }

    public void LoadGame(SideMissionManagerSaveData saveData)
    {
        rejectionCount = saveData.rejectionCount;
        
        for (int i = 0; i < saveData.sideMissionSaveData.Count; i++)
        {
            SideMissionSaveData temp = JsonUtility.FromJson<SideMissionSaveData>(saveData.sideMissionSaveData[i]);
            string nameOfPerson = temp.name;
            Sprite missionSprite = null;

            for (int j = 0; j < possiblePeopleRequesting.Length; j++)
            {
                if (possiblePeopleRequesting[j] == nameOfPerson)
                {
                    missionSprite = peopleSprites[j];
                }
            }

            sideMissionSlots[i].LoadGame(temp, missionSprite);
        }
    }

    public void NewGame()
    {
        GenerateForAllQuestSlots();
        rejectionCount = rejectionMax;
        rejectionDisplay.text = rejectionCount.ToString();
    }

    public void NewDay()
    {
        rejectionCount = rejectionMax;

        for (int i = 0; i < sideMissionSlots.Length; i++)
        {
            sideMissionSlots[i].rejectButton.interactable = true;

            if (sideMissionSlots[i].completed == true)
            {
                GenerateNewQuest(i);
            }
        }

        rejectionDisplay.text = rejectionCount.ToString();
    }

    public void CanReject(int ID)
    {
        if (rejectionCount > 0)
        {
            rejectionCount = rejectionCount - 1;
            GenerateNewQuest(ID);
        }

        if (rejectionCount <= 0)
        {
            for (int i = 0; i < sideMissionSlots.Length; i++)
            {
                sideMissionSlots[i].rejectButton.interactable = false;
            }
        }

        rejectionDisplay.text = rejectionCount.ToString();
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

public class SideMissionManagerSaveData
{
    public List<string> sideMissionSaveData = new List<string>();
    public int rejectionCount;
}