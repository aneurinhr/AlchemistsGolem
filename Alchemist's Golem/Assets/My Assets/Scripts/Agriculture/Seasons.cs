using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

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

    public Renderer terrain;
    public Material[] seasonTerrain;

    public PostProcessingProfile[] seasonProfiles;
    public PostProcessingBehaviour playerProfile;

    private void Start()
    {
        for (int i = 0; i < treeLeaves.Length; i++)
        {
            treeLeaves[i].color = treeLeavesSpringSummer[i];
        }

        SeasonDisplay.sprite = SeasonImages[0];
        terrain.material = seasonTerrain[0];

        playerProfile.profile = seasonProfiles[0];
    }

    public int ChangeSeasons()
    {
        switch(p_mySeason)
        {
            case CurrentSeason.Spring:
                Summer();
                p_mySeason = CurrentSeason.Summer;
                return 1;
                break;
            case CurrentSeason.Summer:
                Autumn();
                p_mySeason = CurrentSeason.Autumn;
                return 2;
                break;
            case CurrentSeason.Autumn:
                Winter();
                p_mySeason = CurrentSeason.Winter;
                return 3;
                break;
            case CurrentSeason.Winter:
                Spring();
                p_mySeason = CurrentSeason.Spring;
                return 0;
                break;
        }

        return 999;
    }

    public void Spring()
    {
        for (int i = 0; i < treeLeaves.Length; i++)
        {
            treeLeaves[i].color = treeLeavesSpringSummer[i];
        }

        Debug.Log("Spring");
        terrain.material = seasonTerrain[0];
        SeasonDisplay.sprite = SeasonImages[0];
        playerProfile.profile = seasonProfiles[0];
    }

    public void Summer()
    {
        Debug.Log("Summer");
        terrain.material = seasonTerrain[1];
        SeasonDisplay.sprite = SeasonImages[1];
        playerProfile.profile = seasonProfiles[1];
    }

    public void Autumn()
    {
        for (int i = 0; i < treeLeaves.Length; i++)
        {
            treeLeaves[i].color = treeLeavesAutumn[i];
        }

        Debug.Log("Autumn");
        terrain.material = seasonTerrain[2];
        SeasonDisplay.sprite = SeasonImages[2];
        playerProfile.profile = seasonProfiles[2];
    }

    public void Winter()
    {
        for (int i = 0; i < treeLeaves.Length; i++)
        {
            treeLeaves[i].color = treeLeavesWinter[i];
        }

        Debug.Log("Winter");
        terrain.material = seasonTerrain[3];
        SeasonDisplay.sprite = SeasonImages[3];
        playerProfile.profile = seasonProfiles[4];
    }
}
