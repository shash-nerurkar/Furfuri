using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class World : MonoBehaviour
{
    public HUD HUD;
    public AudioManager audioManager;
    public GameObject DummyPrefab; 
    [SerializeField]
    private List<MonoBehaviour> objectsToDisableOnLevelEnd;
    public Transform enemyContainerTransform;
    static private string saveFilePath;
    public Map map;
    public int level;
    public int score = 0;

    void Start()
    {
        Transform[] transforms = (Transform[])GameObject.FindObjectsOfType(typeof(Transform));
        foreach(Transform t in transforms)
        {
            if(t.GetComponent<World>() != null && t != transform)
            {
                t.GetComponent<World>().StartLevel();
                Destroy(gameObject);
                return;
            }
        }

        saveFilePath = Application.persistentDataPath + "/FurfuriSaveFile.fun";

        SaveData saveData = World.LoadGame();
        if(saveData != null)
        {
            parseSaveData(saveData);
            GameObject []nodesToLoad = new GameObject[] {map.gameObject};
            foreach(GameObject node in nodesToLoad)
            {
                node.SendMessage("SetSaveData", saveData);
            }
        }
        else
        {
            level = 1;
        }

        DontDestroyOnLoad(gameObject);

        StartLevel();
    }

    public void StartLevel()
    {
        HUD = (HUD)GameObject.FindObjectOfType(typeof(HUD));
        audioManager = (AudioManager)GameObject.FindObjectOfType(typeof(AudioManager));
        map = (Map)GameObject.FindObjectOfType(typeof(Map));
        enemyContainerTransform = GameObject.Find("Enemy Container").transform;


        CalculateLevelParameters(out string []MapLevelParams, out string []enemyContainerLevelParams);
        map.SendMessage("GenerateMap", MapLevelParams);
        enemyContainerTransform.gameObject.SendMessage("SpawnEnemies", enemyContainerLevelParams);


        objectsToDisableOnLevelEnd.Add(GameObject.Find("Player").GetComponent<MonoBehaviour>());
        objectsToDisableOnLevelEnd.Add(GameObject.Find("Player HUD").GetComponent<MonoBehaviour>());
        objectsToDisableOnLevelEnd.Add(GameObject.Find("Weapon").GetComponent<MonoBehaviour>());
        foreach(Transform enemyTransform in enemyContainerTransform)
        {
            objectsToDisableOnLevelEnd.Add(enemyTransform.gameObject.GetComponent<MonoBehaviour>());
        }
    }

    void CalculateLevelParameters(out string []MapLevelParams, 
                                  out string []enemyContainerLevelParams)
    {
        // PARAMETERIZE LEVELS HERE
        MapLevelParams = new string[] {"mapDimensions:Vector3(30,30)"};
        enemyContainerLevelParams = new string[] {"maxEnemies:3", "totalCoins:30"};
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(saveFilePath, FileMode.Create);
        SaveData saveData = new SaveData(map, this);
        binaryFormatter.Serialize(fileStream, saveData);
        fileStream.Close();
    }

    public static SaveData LoadGame()
    {
        if(File.Exists(saveFilePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(saveFilePath, FileMode.Open);

            SaveData saveData = binaryFormatter.Deserialize(fileStream) as SaveData;
            fileStream.Close();

            return saveData;
        } 
        else
        {
            Debug.Log("SAVE FILE NOT FOUND AT: " + saveFilePath);
            return null;
        }
    }

    void parseSaveData(SaveData saveData)
    {
        level = saveData.level;
        score = saveData.score;
    }

    public void DisableObjectsOnLevelEnd(bool isLevelRestarting)
    {
        if(!isLevelRestarting)
        {
            ++level;
        }

        score = Int32.Parse(HUD.HUDScore.text);

        foreach(MonoBehaviour obj in objectsToDisableOnLevelEnd)
        {
            obj.enabled = false;
        }
        objectsToDisableOnLevelEnd = new List<MonoBehaviour>();        
    }
}

[System.Serializable]
public class SaveData
{
    public float[] mapDimensions;
    public bool[,] buildingFlags;
    public float[,] startArchOrientation, endArchOrientation;
    public int level;
    public int score;

    public SaveData(Map map, World world)
    {
        #region mapdata
        try
        {
            mapDimensions = new float[] {map.mapDimensions.x, map.mapDimensions.y};

            buildingFlags = map.buildingFlags;

            startArchOrientation = new float[,] {{map.startArch.transform.position.x, 
                                                  map.startArch.transform.position.y, 
                                                  map.startArch.transform.position.z}, 
                                                 {map.startArch.transform.rotation.eulerAngles.x, 
                                                  map.startArch.transform.rotation.eulerAngles.y, 
                                                  map.startArch.transform.rotation.eulerAngles.z}};

            endArchOrientation = new float[,] {{map.endArch.transform.position.x, 
                                                map.endArch.transform.position.y, 
                                                map.endArch.transform.position.z}, 
                                               {map.endArch.transform.rotation.eulerAngles.x, 
                                                map.endArch.transform.rotation.eulerAngles.y, 
                                                map.endArch.transform.rotation.eulerAngles.z}};
        }
        catch(NullReferenceException)
        {
            Debug.Log("SAVING MAP DATA FAILED: SAVE STATE DOES NOT HAVE MAP.");
        }
        #endregion

        #region world
        level = world.level;
        score = world.score;
        #endregion
    }
}