using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideMission : MonoBehaviour
{
    public int missionHolderID;
    public SideMissionManager manager;
    public ItemDatabase itemDatabase;

    public Text nameDisplay;
    public Image personDisplay;
    public Text numWantedDisplay;
    private int p_numWanted;
    public Image itemDisplay;
    private int p_itemID;
    public Text goldRewardDisplay;
    private int p_Reward;

    public bool AcceptedQuest = false;
    public GameObject beforeAcceptGroup;
    public GameObject HandinGroup;

    public bool CanReject = true;
    public Button rejectButton;
    public Button acceptButton;

    public void AcceptMission()
    {
        Debug.Log("Accepted Mission");
    }

    public void RejectMission()
    {
        Debug.Log("Rejected Mission");
    }

    public void CompleteMission()
    {
        Debug.Log("Completed Mission");
    }

    public void PopulateMission(string name, Sprite person, int num, int itemID, int gold)
    {
        Debug.Log("Populated Mission: " + missionHolderID);

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
