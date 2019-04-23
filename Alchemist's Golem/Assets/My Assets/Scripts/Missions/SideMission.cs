using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideMission : MonoBehaviour
{
    public int missionHolderID;
    public SideMissionManager manager;
    public ItemDatabase itemDatabase;
    public MissionHandinSlot myHandinSlot;
    public Bank bank;

    public Text nameDisplay;
    public Image personDisplay;
    public Text numWantedDisplay;
    private int p_numWanted;
    public Image itemDisplay;
    private int p_itemID;
    public Text goldRewardDisplay;
    public int Reward;

    public GameObject beforeAcceptGroup;
    public GameObject HandinGroup;
    public GameObject CompletedGroup;

    public Button rejectButton;
    public Button acceptButton;

    public bool completed = false;

    public SideMissionSaveData SaveGame()
    {
        MissionSaveData temp = myHandinSlot.SaveGame();
        temp.TurnInItemID = p_itemID;
        temp.totalNeeded = p_numWanted;

        SideMissionSaveData saveData = new SideMissionSaveData();
        saveData.missionSaveData = JsonUtility.ToJson(temp);

        saveData.name = nameDisplay.text;
        saveData.reward = Reward;
        saveData.beforeAcceptGroup = beforeAcceptGroup.activeSelf;
        saveData.HandinGroup = HandinGroup.activeSelf;
        saveData.CompletedGroup = CompletedGroup.activeSelf;

        return saveData;
    }

    public void LoadGame(SideMissionSaveData info, Sprite personInfo)
    {
        MissionSaveData temp = JsonUtility.FromJson<MissionSaveData>(info.missionSaveData);
        myHandinSlot.LoadGame(temp);

        p_numWanted = temp.totalNeeded;
        p_itemID = temp.TurnInItemID;
        Reward = info.reward;

        nameDisplay.text = info.name;
        personDisplay.sprite = personInfo;
        numWantedDisplay.text = p_numWanted.ToString();
        Item tempItem = itemDatabase.GetItem(p_itemID);
        itemDisplay.sprite = tempItem.itemImage;
        goldRewardDisplay.text = Reward.ToString();

        beforeAcceptGroup.SetActive(info.beforeAcceptGroup);
        HandinGroup.SetActive(info.HandinGroup);
        CompletedGroup.SetActive(info.CompletedGroup);
        
        if (info.beforeAcceptGroup == false)
        {
            myHandinSlot.SetRequirements(p_itemID, temp.currentHandinNeeded);
            myHandinSlot.overlayImage.sprite = tempItem.itemImage;
            acceptButton.interactable = false;
        }
    }

    public void AcceptMission()
    {
        myHandinSlot.SetRequirements(p_itemID, p_numWanted);

        beforeAcceptGroup.SetActive(false);
        HandinGroup.SetActive(true);
        CompletedGroup.SetActive(false);

        acceptButton.interactable = false;
    }

    public void RejectMission()
    {
        manager.CanReject(missionHolderID);
    }

    public virtual void CompleteMission()
    {
        bank.ChangeMoney(Reward);

        beforeAcceptGroup.SetActive(false);
        HandinGroup.SetActive(false);
        CompletedGroup.SetActive(true);

        completed = true;
        rejectButton.interactable = false;
        acceptButton.interactable = false;
    }

    public void PopulateMission(string name, Sprite person, int num, int itemID, int gold)
    {
        completed = false;

        p_numWanted = num;
        p_itemID = itemID;
        Reward = gold;

        nameDisplay.text = name;
        personDisplay.sprite = person;
        numWantedDisplay.text = num.ToString();
        Item temp = itemDatabase.GetItem(itemID);
        itemDisplay.sprite = temp.itemImage;
        goldRewardDisplay.text = gold.ToString();

        beforeAcceptGroup.SetActive(true);
        HandinGroup.SetActive(false);
        CompletedGroup.SetActive(false);

        acceptButton.interactable = true;
    }
}

public class SideMissionSaveData
{
    public string missionSaveData;
    public string name;
    public int reward;

    public bool beforeAcceptGroup;
    public bool HandinGroup;
    public bool CompletedGroup;
}
