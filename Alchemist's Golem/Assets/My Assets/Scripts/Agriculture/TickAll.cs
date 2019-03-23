using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TickAll : MonoBehaviour
{
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
    public float timeLimit = 300.0f;
    public float currentTime = 1.0f;
    public bool pauseTimer = true;
    public bool punish = false;
    public float punishmentVal = 0.3f;

    public ItemDatabase itemDatabase;

    public int day = 1;
    public int maxDay = 28;
    public Seasons seasons;
    public Text dayDisplay;

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

                    currentTime = 1.0f;
                    if (punish == true)
                    {
                        currentTime = currentTime - punishmentVal;
                        punish = false;
                    }
                    battery.fillAmount = currentTime;
                }

                StartCoroutine(FadeAway());
            }
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

        if (pauseTimer == false)
        {
            currentTime = currentTime - ((1.0f / timeLimit) * Time.deltaTime);
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

        if (Input.GetButtonDown("Interact") && (beingLookedAt == true) && ((fade == false) && (unfade == false)))
        {
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

        //Change Day
        day = day + 1;
        if (day > maxDay)
        {
            seasons.ChangeSeasons();
            day = 1;
        }
        dayDisplay.text = day.ToString();
    }
}
