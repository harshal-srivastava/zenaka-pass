using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    public delegate void GameLoadFailedEvent();
    public static GameLoadFailedEvent GameSaveDataLoadFailedCB;

    private void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        // Ensure the directory exists where the save file will be written
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }
    }

    public void ReadGameData(GameData data)
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
        GameData data = new GameData();
        try
        {
            data = LoadData();
        }
        catch(FileNotFoundException ex)
        {
            Debug.LogError("Could not find saved data " + ex.Message);
            GameSaveDataLoadFailedCB?.Invoke();
            return null;
        }
        finally
        {
            Debug.LogError("loading save data failed");
        }
        return data;
    }

    GameData LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<GameData>(jsonData);
        }
        else
        {
            Debug.Log("Save file not found.");
            throw new FileNotFoundException("Saved file not found");
        }
    }
}
