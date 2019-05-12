using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Upgrades : MonoBehaviour
{
    public PlotCollection plotCollection;
    public Bank wallet;
    public PlayerMovement playerMovement;

    public AudioSource openUpgrades;

    public GameObject highlight;
    public GameObject upgradeMenu;

    public bool beingLookedAt = false;
    public bool open = false;
    public bool canChange = true;

    public Button[] UpgradeButtons;
    public Text[] Costs;
    public bool[] UpgradesBrought;
    public GameObject[] physicalUpgrade;
    public int[] UpgradesCost;

    public void NewGame()
    {
        for (int i = 0; i < UpgradesBrought.Length; i++)
        {
            UpgradesBrought[i] = false;
            UpgradeButtons[i].interactable = true;
            physicalUpgrade[i].SetActive(false);
        }
    }

    public void LoadGame(bool[] upgradesUnlocked)
    {
        for (int i = 0; i < upgradesUnlocked.Length; i++)
        {
            if (upgradesUnlocked[i] == true)
            {
                UpgradeButtons[i].interactable = false;
                UpgradesBrought[i] = true;
                physicalUpgrade[i].SetActive(true);
                plotCollection.Upgrade(i);
            }
        }
    }

    private void Start()
    {
        for (int i = 0; i < Costs.Length; i++)
        {
            Costs[i].text = UpgradesCost[i].ToString();
        }
    }

    public void BuyUpgrade(int upgradeNum)
    {
        bool worked = wallet.ChangeMoney(-UpgradesCost[upgradeNum]);

        if (worked == true)
        {
            UpgradeButtons[upgradeNum].interactable = false;
            UpgradesBrought[upgradeNum] = true;
            physicalUpgrade[upgradeNum].SetActive(true);
            plotCollection.Upgrade(upgradeNum);
        }
    }

    public void BeingLookedAt()
    {
        highlight.SetActive(true);
        beingLookedAt = true;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (open == true)
            {
                UIOff();
            }
        }

        highlight.SetActive(false);
    }

    private void LateUpdate()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if ((open == false) && (canChange == true) && (beingLookedAt == true))
            {
                UIOn();
            }
            else if (open == true)
            {
                UIOff();
            }
        }

        beingLookedAt = false;
    }

    public void UIOn()
    {
        open = true;
        upgradeMenu.SetActive(true);
        playerMovement.pauseMovement = true;
        openUpgrades.Play();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UIOff()
    {
        open = false;
        upgradeMenu.SetActive(false);
        playerMovement.pauseMovement = false;
        openUpgrades.Play();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
