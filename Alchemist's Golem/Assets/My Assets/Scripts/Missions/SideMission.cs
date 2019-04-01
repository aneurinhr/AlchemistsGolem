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
    public GameObject CompletedGroup;

    public Button rejectButton;
    public Button acceptButton;

    public bool completed = false;

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

    public void CompleteMission()
    {
        bank.ChangeMoney(p_Reward);

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
        p_Reward = gold;

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
