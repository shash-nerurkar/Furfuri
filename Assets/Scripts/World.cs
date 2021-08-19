using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class World : MonoBehaviour
{
    public AudioManager audioManager;
    public GameObject DummyPrefab; 
    public List<MonoBehaviour> objectsToDisableOnLevelEnd;
    public Transform enemyContainerTransform;
    static private string saveFilePath;
    public Map map;
    public int level = 1;

    void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/FurfuriSaveFile.fun";

        foreach(Transform enemyTransform in enemyContainerTransform)
        {
            objectsToDisableOnLevelEnd.Add(enemyTransform.gameObject.GetComponent<MonoBehaviour>());
        }

        enemyContainerTransform.gameObject.SendMessage("SpawnEnemies", level);
    }

    public void Test(Vector3 location = new Vector3(), Color color = default(Color))
    {
        Instantiate(DummyPrefab, (Vector3)location, Quaternion.identity);
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(saveFilePath, FileMode.Create);
        SaveData saveData = new SaveData(map);
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
}

[System.Serializable]
public class SaveData
{
    public float[] mapDimensions;
    public bool[,] buildingFlags;
    public float[,] startArchOrientation, endArchOrientation;

    public SaveData(Map map)
    {
        #region mapdata
        mapDimensions = new float[] {map.mapDimensions.x, map.mapDimensions.y};

        buildingFlags = new bool[map.buildingFlags.GetLength(0), map.buildingFlags.GetLength(1)];
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
        #endregion
    }
}