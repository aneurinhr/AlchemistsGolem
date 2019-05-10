using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;

public class Options : MonoBehaviour
{
    public GameObject OptionsUI;
    public bool UIon = false;

    public AudioMixer masterMixer;

    public string gameDataFileName = "settings.json";
    public string path;

    //For start setting playerpref save and load
    public Toggle fullscreen;
    public Dropdown resolution;
    public Dropdown graphics;
    public Slider master;
    public Slider sfx;
    public Slider ui;
    public Slider music;

    private Resolution[] resolutionsArray;

    public void SaveSettings()
    {
        SettingsSaveData saveData = new SettingsSaveData();

        saveData.fullscreen = fullscreen.isOn;
        saveData.resolutionChoice = resolution.value;
        saveData.qualityChoice = graphics.value;

        saveData.MasterVol = master.value;
        saveData.SFXVol = sfx.value;
        saveData.UIVol = ui.value;
        saveData.MusicVol = music.value;

        string tempPath = path + "/" + gameDataFileName;
        string saveDataString = JsonUtility.ToJson(saveData);

        File.WriteAllText(tempPath, saveDataString);
    }

    public void LoadSettings()
    {
        string filePath = path + "/" + gameDataFileName;

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            SettingsSaveData saveData = JsonUtility.FromJson<SettingsSaveData>(dataAsJson);

            FullscreenToggle(saveData.fullscreen);
            fullscreen.isOn = saveData.fullscreen;
            ChangeResolution(saveData.resolutionChoice);
            resolution.value = saveData.resolutionChoice;
            ChangeGraphics(saveData.qualityChoice);
            graphics.value = saveData.qualityChoice;
            ChangeMasterVol(saveData.MasterVol);
            master.value = saveData.MasterVol;
            ChangeSFXVol(saveData.SFXVol);
            sfx.value = saveData.SFXVol;
            ChangeUIVol(saveData.UIVol);
            ui.value = saveData.UIVol;
            ChangeMusicVol(saveData.MusicVol);
            music.value = saveData.MusicVol;
        }
    }

    private void Awake()
    {
        path = Application.dataPath;

        StartUp();
    }

    private void StartUp()
    {
        //Resolutions
        resolutionsArray = Screen.resolutions;
        resolution.ClearOptions();

        List<string> options = new List<string>();

        Resolution current = Screen.currentResolution;
        int resolutionChoice = 0;

        for (int i = 0; i < resolutionsArray.Length; i++)
        {
            string temp = resolutionsArray[i].width + " x " + resolutionsArray[i].height;
            options.Add(temp);

            if ((resolutionsArray[i].height == current.height) && (resolutionsArray[i].width == current.width))
            {
                resolutionChoice = i;
            }
        }

        resolution.AddOptions(options);

        //SET Defaults
        fullscreen.isOn = Screen.fullScreen;

        resolution.value = resolutionChoice;
        resolution.RefreshShownValue();

        graphics.value = QualitySettings.GetQualityLevel();
        graphics.RefreshShownValue();

        float mastertemp = 0;
        masterMixer.GetFloat("masterVol", out mastertemp);
        master.value = 0;

        float sfxtemp = 0;
        masterMixer.GetFloat("sfxVol", out sfxtemp);
        sfx.value = 0;

        float uitemp = 0;
        masterMixer.GetFloat("uiVol", out uitemp);
        ui.value = 0;

        float musictemp = 0;
        masterMixer.GetFloat("musicVol", out musictemp);
        music.value = 0;

        //LOAD Settings
        LoadSettings();
    }
    //End of start

    public void ToggleOn()
    {
        OptionsUI.SetActive(true);
        UIon = true;
    }

    public void ToggleOff()
    {
        OptionsUI.SetActive(false);
        UIon = false;
    }

    public void FullscreenToggle(bool toggle)
    {
        Screen.fullScreen = toggle;
    }

    public void ChangeGraphics(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void ChangeResolution(int index)
    {
        Resolution tempRes = resolutionsArray[index];

        Screen.SetResolution(tempRes.width, tempRes.height, Screen.fullScreen);
    }

    public void ChangeMasterVol(float volume)
    {
        masterMixer.SetFloat("masterVol", volume);
    }

    public void ChangeSFXVol(float volume)
    {
        masterMixer.SetFloat("sfxVol", volume);
    }

    public void ChangeUIVol(float volume)
    {
        masterMixer.SetFloat("uiVol", volume);
    }

    public void ChangeMusicVol(float volume)
    {
        masterMixer.SetFloat("musicVol", volume);
    }
}

public class SettingsSaveData
{
    public bool fullscreen;
    public int resolutionChoice;
    public int qualityChoice;

    public float MasterVol;
    public float SFXVol;
    public float UIVol;
    public float MusicVol;
}