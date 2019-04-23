using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class SaveAndLoad : MonoBehaviour
{
    public Inventory inventory; //Save content
    public Storage storage; //Save content
    public ItemDatabase itemDatabase; //Save fluctiating prices
    public TickAll tickAll; //For day and season
    public PlotCollection[] plotCollections; //For plot nutrients and crops
    public Bank bank; //For money

    public string gameDataFileName = "data.json";
    public string path;

    public Button loadGameButton;

    private void Start()
    {
        path = Application.dataPath;

        string filePath = path + "/" + gameDataFileName;

        if (File.Exists(filePath))
        {
            loadGameButton.interactable = true;
        }
        else
        {
            loadGameButton.interactable = false;
        }
    }

    public void SaveGame()
    {
        List<string> dataArray = new List<string>();

        List<SlotSaveData> inventorySaveData = inventory.SaveInfo();//Inventory Save Data
        for (int i = 0; i < inventorySaveData.Count; i++)
        {
            string inventoryStringData = JsonUtility.ToJson(inventorySaveData[i]);
            dataArray.Add(inventoryStringData);
        }

        dataArray.Add("BREAK");

        List<SlotSaveData> storageSaveData = storage.SaveInfo();//Storage Save Data
        for (int i = 0; i < storageSaveData.Count; i++)
        {
            string storageStringData = JsonUtility.ToJson(storageSaveData[i]);
            dataArray.Add(storageStringData);
        }

        dataArray.Add("BREAK");

        List<FluctuationSaveData> itemDatabaseSaveData = itemDatabase.SaveInfo();//Item Save Data
        for (int i = 0; i < itemDatabaseSaveData.Count; i++)
        {
            string itemDatabaseStringData = JsonUtility.ToJson(itemDatabaseSaveData[i]);
            dataArray.Add(itemDatabaseStringData);
        }

        dataArray.Add("BREAK");

        TickSaveData tickSaveData = tickAll.SaveInfo();//Day Save Data
        string tickStringData = JsonUtility.ToJson(tickSaveData);
        //Debug.Log(tickStringData);
        dataArray.Add(tickStringData);

        dataArray.Add("BREAK");

        int tempBank = bank.SaveGame();
        dataArray.Add(tempBank.ToString());

        dataArray.Add("BREAK");

        for (int i = 0; i < plotCollections.Length; i++)
        {
            CollectionSaveData collectionSaveData = plotCollections[i].SaveInfo();//Plot Save Data

            string plotStringData = JsonUtility.ToJson(collectionSaveData);
            dataArray.Add(plotStringData);
            //Debug.Log(plotStringData);

            dataArray.Add("BREAK");
        }

        //write to file
        string tempPath = path + "/" + gameDataFileName;

        File.WriteAllLines(tempPath, dataArray);

        loadGameButton.interactable = true;
    }

    public void NewGame()
    {
        itemDatabase.NewGame();
        storage.NewGame();
        inventory.NewGame();
        tickAll.NewGame();

        for (int i = 0; i < plotCollections.Length; i++)
        {
            plotCollections[i].NewGame();
        }
    }

    public void LoadGame()
    {
        //load from file
        string filePath = path + "/" + gameDataFileName;

        if (File.Exists(filePath))
        {
            string[] dataAsJson = File.ReadAllLines(filePath);

            //Stage 1 get Save Data

            List<SlotSaveData> inventorySaveData = new List<SlotSaveData>();//Inventory Save Data
            List<SlotSaveData> storageSaveData = new List<SlotSaveData>();//Storage Save Data
            List<FluctuationSaveData> itemDatabaseSaveData = new List<FluctuationSaveData>();//Item Save Data
            TickSaveData tickSaveData = new TickSaveData();//Day Save Data
            List<CollectionSaveData> collectionSaveData = new List<CollectionSaveData>();
            int bankData = 0;

            int phase = 0;

            for (int i = 0; i < dataAsJson.Length; i++)
            {
                string line = dataAsJson[i];

                if (line != "BREAK")
                {
                    switch (phase)
                    {
                        case 0://invent
                            SlotSaveData tempI = JsonUtility.FromJson<SlotSaveData>(line);
                            inventorySaveData.Add(tempI);
                            break;
                        case 1://storage
                            SlotSaveData tempS = JsonUtility.FromJson<SlotSaveData>(line);
                            storageSaveData.Add(tempS);
                            break;
                        case 2://items
                            FluctuationSaveData tempIt = JsonUtility.FromJson<FluctuationSaveData>(line);
                            itemDatabaseSaveData.Add(tempIt);
                            break;
                        case 3://tick
                            tickSaveData = JsonUtility.FromJson<TickSaveData>(line);
                            break;
                        case 4://tick
                            bankData = int.Parse(line);
                            break;
                        default://Plots e.g. anything thats not the first 4 fixed things
                            CollectionSaveData tempC = JsonUtility.FromJson<CollectionSaveData>(line);
                            collectionSaveData.Add(tempC);
                            break;
                    }
                }
                else
                {
                    phase = phase + 1;
                }
            }//End for

            //Stage 2, load
            inventory.LoadInfo(inventorySaveData);

            storage.LoadInfo(storageSaveData);

            itemDatabase.LoadInfo(itemDatabaseSaveData);

            tickAll.LoadInfo(tickSaveData);

            bank.LoadGame(bankData);

            for (int i = 0; i < plotCollections.Length; i++)
            {
                plotCollections[i].LoadInfo(collectionSaveData[i]);
            }
        }
    }
}
