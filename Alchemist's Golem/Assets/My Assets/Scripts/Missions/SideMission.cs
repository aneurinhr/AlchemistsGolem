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
    private int p_Reward;

    public GameObject beforeAcceptGroup;
    public GameObject HandinGroup;

    public Button rejectButton;
    public Button acceptButton;


    public void AcceptMission()
    {
        myHandinSlot.SetRequirements(p_itemID, p_numWanted);

        beforeAcceptGroup.SetActive(false);
        HandinGroup.SetActive(true);
    }

    public void RejectMission()
    {
        manager.CanReject(missionHolderID);
    }

    public void CompleteMission()
    {
        bank.ChangeMoney(p_Reward);
        manager.GenerateNewQuest(missionHolderID);
    }

    public void PopulateMission(string name, Sprite person, int num, int itemID, int gold)
    {
        p_numWanted = num;
        p_itemID = itemID;
        p_Reward = gold;

        nameDisplay.text = name;
        personDisplay.sprite = person;
        numWantedDisplay.text = num.ToString();
        Item temp = itemDatabase.GetItem(itemID);
        itemDisplay.sprite = temp.itemImage;
        goldRewardDisplay.text = gold.ToString();

        beforeAcceptGroup.SetActive(true);
        HandinGroup.SetActive(false);
    }
}
