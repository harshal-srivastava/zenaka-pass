using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Class responsible for saving and loading the user progress data
/// Mechanism for user progress saved and loaded is via converting the user progress in JSON format
/// Then writing the json string to a file
/// For loading, read the json file and convert it back to GameData
/// </summary>
public class GameSaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    /// <summary>
    /// delegate event in case the save file is not present
    /// </summary>
    public delegate void GameLoadFailedEvent();
    public static GameLoadFailedEvent GameSaveDataLoadFailedCB;

    private void Start()
    {
        CheckDirectory();
    }

    /// <summary>
    /// Check if the particular directory at saveFilePath exists
    /// If doesn't exist, create one
    /// </summary>
    private void CheckDirectory()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        // Ensure the directory exists where the save file will be written
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }
    }

    /// <summary>
    /// Function to convert the GameData object passed to a json format 
    /// Then write it to a text file
    /// </summary>
    /// <param name="data"></param>
    public void SaveGameData(GameData data)
    {
        string jsonData = JsonUtility.ToJson(data);

        // Check if file does not exist and create an empty file if necessary
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("Save file does not exist. Creating a new one.");
            // Create and close the file to flush it to disk
            File.Create(saveFilePath).Dispose(); 
        }

        // Write the json data to the file
        File.WriteAllText(saveFilePath, jsonData);
        Debug.Log("Game saved successfully at : " + saveFilePath);
    }

    /// <summary>
    /// Function to make the call to load the game data
    /// Added error handling here in case the save file is not present
    /// </summary>
    /// <returns></returns>
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
        return data;
    }

    /// <summary>
    /// Function to read the json file from text
    /// Convert it back to GameData object and return it
    /// </summary>
    /// <returns></returns>
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
