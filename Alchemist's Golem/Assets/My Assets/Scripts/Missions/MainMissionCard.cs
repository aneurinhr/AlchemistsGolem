using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMissionCard : SideMission
{
    public MainMissions mainMissionManager;

    public override void CompleteMission()
    {
        bank.ChangeMoney(Reward);

        beforeAcceptGroup.SetActive(false);
        HandinGroup.SetActive(false);
        CompletedGroup.SetActive(true);

        completed = true;
        acceptButton.interactable = false;

        mainMissionManager.CompletedCurrentMission();
    }
}
