using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMissions : MonoBehaviour
{
    public MissionStrings[] missions;
    public int currentMission = 0;
    public int currentStringDisplay = 0;

    public GameObject TextDisplay;
    public Text DispText;
    public bool textIsDisplaying = false;
    public bool pauseSkip = true;

    public PlayerMovement player;

    private void Start()
    {
        //Move when finished testing
        StartNewMission(currentMission);
    }

    public void StartNewMission(int mission)
    {
        currentMission = mission;

        StartStringDisplay();
    }

    public void StartStringDisplay()
    {
        currentStringDisplay = 0;
        player.pauseMovement = true;

        textIsDisplaying = true;
        TextDisplay.SetActive(true);
        DispText.text = missions[currentMission].Lines[currentStringDisplay];

        StartCoroutine(PauseSkip());
    }

    public void NextStringDisplay()
    {
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
        }
    }

    public void CompletedCurrentMission()//This needs to get called
    {
        //Stuff

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