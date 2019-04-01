﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Upgrades : MonoBehaviour
{
    public PlotCollection plotCollection;
    public Bank wallet;
    public PlayerMovement playerMovement;

    public AudioSource openUpgrades;
    public AudioSource upgrade;
    public AudioSource notEnoughMoney;

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

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UIOff()
    {
        open = false;
        upgradeMenu.SetActive(false);
        playerMovement.pauseMovement = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}