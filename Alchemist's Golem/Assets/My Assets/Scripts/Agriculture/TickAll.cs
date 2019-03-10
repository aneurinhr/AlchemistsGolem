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

            Debug.Log(alpha);

            if (alpha <= 0.0f)//Faded Out
            {
                fade = false;
                player.pauseMovement = false;
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
                }

                StartCoroutine(FadeAway());
            }
        }
    }

    IEnumerator FadeAway()
    {
        print("Hey");
        yield return new WaitForSecondsRealtime(0.3f);
        print("fade");
        fade = true;
    }

    private void Update()
    {
        Fade();
    }

    private void LateUpdate()
    {

        if (Input.GetButtonDown("Interact") && (beingLookedAt == true) && ((fade == false) && (unfade == false)))
        {
            unfade = true;
            player.pauseMovement = true;
            ticked = false;
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
        GameObject[] plants = GameObject.FindGameObjectsWithTag(tag);

        for (int i = 0; i < plants.Length; i++)
        {
            plants[i].GetComponent<Crop>().Tick();
        }
    }
}
