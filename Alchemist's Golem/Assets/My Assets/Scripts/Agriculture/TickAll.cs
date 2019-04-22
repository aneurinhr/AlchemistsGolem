using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TickAll : MonoBehaviour
{
    public enum CurrentSeason { Spring, Summer, Autumn, Winter };

    public string tag = "Crop";
    public GameObject highlight;
    public bool beingLookedAt = false;

    public PlotCollection[] plots;
    public Image fadeTo;

    public bool fade = false;
    public bool unfade = false;
    public bool ticked = false;

    public float f = 0.0f;
    public float fadeSpeed = 100.0f;

    public PlayerMovement player;

    public Image battery;
    public Sprite NormalCharge;
    public Sprite OverCharged;
    public Sprite UnderCharged;
    public bool OverCharging = false;
    public bool UnderCharging = false;

    public int RandomMax = 1;
    public float timeLimit = 300.0f;
    public float currentTime = 1.0f;
    public bool pauseTimer = true;
    public bool punish = false;
    public float punishmentVal = 0.3f;

    public bool pauseKeyPress = false;

    public ItemDatabase itemDatabase;
    public SideMissionManager sideMissions;

    public int day = 1;
    public int maxDay = 28;
    public Seasons seasons;
    public Text dayDisplay;

    public GameObject warpPoint;

    public SaveAndLoad save;

    public TickSaveData SaveInfo()
    {
        TickSaveData saveData = new TickSaveData();
        saveData.day = day;
        saveData.punish = punish;
        saveData.OverCharging = OverCharging;
        saveData.UnderCharging = UnderCharging;
        saveData.season = seasons.SaveInfo();

        return saveData;
    }

    public void LoadInfo(string info)
    {

    }

    private void Start()
    {
        currentTime = 1.0f;
        pauseTimer = true;
    }

    public void BeingLookedAt()
    {
        highlight.SetActive(true);
        beingLookedAt = true;
    }

    public void Fade()
    {
        float alpha = 999;

        if (fade == true)
        {
            f = f - fadeSpeed * Time.deltaTime;
        }
        else if (unfade == true)
        {
            f = f + fadeSpeed * Time.deltaTime;
        }

        if (f != 0.0f)
        {
            Color c = fadeTo.color;
            c.a = f;
            fadeTo.color = c;

            alpha = fadeTo.color.a;

            if (alpha <= 0.0f)//Faded Out
            {
                fade = false;
                pauseKeyPress = false;
                player.pauseMovement = false;

                pauseTimer = false;
                f = 0.0f;
            }
            else if (alpha >= 1.0f)//Faded In
            {
                unfade = false;

                //Do these
                if (ticked == false)
                {
                    TickForAll();
                    UpdatePlots();
                    ticked = true;

                    RandomCharge();
                    currentTime = 1.0f;

                    if (punish == true)
                    {
                        currentTime = currentTime - punishmentVal;
                        punish = false;
                    }
                    battery.fillAmount = currentTime;

                    save.SaveGame();

                    player.WarpPlayer(warpPoint.transform.position);
                }

                StartCoroutine(FadeAway());
            }
        }
    }

    public void RandomCharge()
    {
        OverCharging = false;
        UnderCharging = false;
        battery.sprite = NormalCharge;

        int rand = Random.Range(0, (RandomMax+1));

        if (rand == RandomMax)
        {
            OverCharging = true;
            battery.sprite = OverCharged;
        }
        else if (rand == 0)
        {
            UnderCharging = true;
            battery.sprite = UnderCharged;
        }
    }

    IEnumerator FadeAway()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        fade = true;
    }

    private void Update()
    {
        Fade();

        if (pauseTimer == false) //Charge changing
        {
            float mult = 1.0f;

            if (UnderCharging == true)
            {
                mult = 2.0f;
            }
            else if (OverCharging == true)
            {
                mult = 0.5f;
            }

            currentTime = currentTime - ((1.0f / timeLimit) * Time.deltaTime * mult);
            battery.fillAmount = currentTime;
        }

        if (currentTime < 0.0f)
        {
            LatePunishment();
        }
    }

    public void LatePunishment()
    {
        unfade = true;
        player.pauseMovement = true;
        ticked = false;
        pauseTimer = true;
        punish = true;
    }

    private void LateUpdate()
    {

        if (Input.GetButtonDown("Interact") && (beingLookedAt == true) && (pauseKeyPress == false))
        {
            pauseKeyPress = true;
            unfade = true;
            player.pauseMovement = true;
            ticked = false;
            pauseTimer = true;
            punish = false;
        }

        highlight.SetActive(false);
        beingLookedAt = false;
    }

    public void UpdatePlots()
    {
        for (int i = 0; i < plots.Length; i++)
        {
            plots[i].PlotDiffuse();
            plots[i].NaturalPlotIncrease();
        }
    }

    public void TickForAll()
    {
        for (int i = 0; i < plots.Length; i++)
        {
            plots[i].TickAll();
        }

        itemDatabase.UpdateFluctiatingPrices();
        sideMissions.NewDay();

        //Change Day
        day = day + 1;
        if (day > maxDay)
        {
            int season = seasons.ChangeSeasons();

            for (int i = 0; i < plots.Length; i++)
            {
                plots[i].seasonVal = season;
            }

            day = 1;
            sideMissions.GenerateForAllQuestSlots();
        }
        dayDisplay.text = day.ToString();
    }
}

public class TickSaveData
{
    public int day;
    public bool OverCharging;
    public bool UnderCharging;
    public bool punish;
    public CurrentSeason season;
}
