using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    private void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        // Ensure the directory exists where the save file will be written
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }
    }

    public void SaveGameData(GameData data)
    {
        string jsonData = JsonUtility.ToJson(data);

        // Check if file does not exist and create an empty file if necessary
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("Save file does not exist. Creating a new one.");
            File.Create(saveFilePath).Dispose(); // Create and close the file to flush it to disk
        }

        // Write the json data to the file
        File.WriteAllText(saveFilePath, jsonData);
        Debug.Log("Game saved successfully at : " + saveFilePath);
    }

    public GameData LoadGameData()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<GameData>(jsonData);
        }
        else
        {
            Debug.LogWarning("Save file not found.");
            return null;
        }
    }
}
