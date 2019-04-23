using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMissions : MonoBehaviour
{
    public MissionStrings[] missions;
    public MissionCardData[] missionData;
    public int currentMission = 0;
    public int currentStringDisplay = 0;

    public GameObject TextDisplay;
    public Text DispText;
    public bool textIsDisplaying = false;
    public bool pauseSkip = true;

    public MainMissionCard missionSlot;

    public PlayerMovement player;
    public MissionBoardInteractable missionBoard;

    public EscapeMenu escp;

    public Sprite alchemistMaster;

    public MainMissionSaveData SaveGame()
    {
        SideMissionSaveData temp = missionSlot.SaveGame();
        MainMissionSaveData saveData = new MainMissionSaveData();
        saveData.missionSaveData = JsonUtility.ToJson(temp);
        saveData.currentMission = currentMission;
        saveData.currentStringDisplay = currentStringDisplay;

        return saveData;
    }

    public void LoadGame(MainMissionSaveData info)
    {
        SideMissionSaveData temp = JsonUtility.FromJson<SideMissionSaveData>(info.missionSaveData);
        missionSlot.LoadGame(temp, alchemistMaster);

        currentMission = info.currentMission;
        currentStringDisplay = info.currentStringDisplay;
    }

    public void StartNewMission(int mission)
    {
        currentMission = mission;
        escp.prevent = true;

        if (currentMission < missions.Length)
        {
            StartStringDisplay();

            if (currentMission < missionData.Length)
            {
                missionData[currentMission].PopulateMissionSlot(missionSlot);
            }
        }
    }

    public void StartStringDisplay()
    {
        escp.prevent = true;

        currentStringDisplay = 0;
        player.pauseMovement = true;

        textIsDisplaying = true;
        TextDisplay.SetActive(true);
        DispText.text = missions[currentMission].Lines[currentStringDisplay];

        StartCoroutine(PauseSkip());
    }

    public void NextStringDisplay()
    {
        escp.prevent = true;
        currentStringDisplay = currentStringDisplay + 1;

        if (currentStringDisplay <= (missions[currentMission].Lines.Length - 1))
        {
            TextDisplay.SetActive(true);
            DispText.text = missions[currentMission].Lines[currentStringDisplay];
            StartCoroutine(PauseSkip());
        }
        else
        {
            TextDisplay.SetActive(false);
            player.pauseMovement = false;
            textIsDisplaying = false;
            escp.prevent = false; 
        }
    }

    public void CompletedCurrentMission()//This needs to get called
    {
        missionBoard.MissionsOff();
        StartNewMission(currentMission + 1);
    }

    private void Update()
    {
        if ((Input.anyKeyDown) && (textIsDisplaying == true) && (pauseSkip == false))
        {
            NextStringDisplay();//Skip text
        }
    }

    IEnumerator PauseSkip()
    {
        pauseSkip = true;
        yield return new WaitForSeconds(0.3f);
        pauseSkip = false;
    }

}

[System.Serializable]
public class MissionStrings
{
    public string[] Lines;
}

[System.Serializable]
public class MissionCardData
{
    public string name;
    public Sprite person;
    public int num;
    public int itemID;
    public int gold;

    public void PopulateMissionSlot(SideMission missionSlot)
    {
        missionSlot.PopulateMission(name, person, num, itemID, gold);
        missionSlot.AcceptMission();
    }
}

public class MainMissionSaveData
{
    public string missionSaveData;
    public int currentMission = 0;
    public int currentStringDisplay = 0;
}