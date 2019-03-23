using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Seasons : MonoBehaviour
{
    enum CurrentSeason { Spring, Summer, Autumn, Winter };

    public Material[] treeLeaves;

    public Color[] treeLeavesSpringSummer;
    public Color[] treeLeavesAutumn;
    public Color[] treeLeavesWinter;

    private CurrentSeason p_mySeason = CurrentSeason.Spring;

    public Image SeasonDisplay;
    public Sprite[] SeasonImages;

    private void Start()
    {
        for (int i = 0; i < treeLeaves.Length; i++)
        {
            treeLeavesSpringSummer[i] = treeLeaves[i].color;
        }

        SeasonDisplay.sprite = SeasonImages[0];
    }

    public void ChangeSeasons()
    {
        switch(p_mySeason)
        {
            case CurrentSeason.Spring:
                Summer();
                p_mySeason = CurrentSeason.Summer;
                break;
            case CurrentSeason.Summer:
                Autumn();
                p_mySeason = CurrentSeason.Autumn;
                break;
            case CurrentSeason.Autumn:
                Winter();
                p_mySeason = CurrentSeason.Winter;
                break;
            case CurrentSeason.Winter:
                Spring();
                p_mySeason = CurrentSeason.Spring;
                break;
        }
    }

    public void Spring()
    {
        for (int i = 0; i < treeLeaves.Length; i++)
        {
            treeLeaves[i].color = treeLeavesSpringSummer[i];
        }

        Debug.Log("Spring");
        SeasonDisplay.sprite = SeasonImages[0];
    }

    public void Summer()
    {
        Debug.Log("Summer");
        SeasonDisplay.sprite = SeasonImages[1];
    }

    public void Autumn()
    {
        for (int i = 0; i < treeLeaves.Length; i++)
        {
            treeLeaves[i].color = treeLeavesAutumn[i];
        }

        Debug.Log("Autumn");
        SeasonDisplay.sprite = SeasonImages[2];
    }

    public void Winter()
    {
        for (int i = 0; i < treeLeaves.Length; i++)
        {
            treeLeaves[i].color = treeLeavesWinter[i];
        }

        Debug.Log("Winter");
        SeasonDisplay.sprite = SeasonImages[3];
    }
}
