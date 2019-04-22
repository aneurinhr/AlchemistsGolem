using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveAndLoad : MonoBehaviour
{
    public Inventory inventory; //Save content
    public Storage storage; //Save content
    public ItemDatabase itemDatabase; //Save fluctiating prices
    public TickAll tickAll; //For day and season
    public PlotCollection[] plotCollections; //For plot nutrients and crops

    public string gameDataFileName = "data.json";
    public string path;

    private void Start()
    {
        path = Application.dataPath;
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
        Debug.Log(tempPath);

        File.WriteAllLines(tempPath, dataArray);
    }

    public void LoadGame()
    {
        //load from file

    }
}
