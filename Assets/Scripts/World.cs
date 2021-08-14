using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public AudioManager audioManager;
    public GameObject DummyPrefab; 
    public List<MonoBehaviour> objectsToDisableOnLevelEnd;
    public Transform enemyContainerTransform;

    void Awake()
    {
        foreach(Transform enemyTransform in enemyContainerTransform)
        {
            objectsToDisableOnLevelEnd.Add(enemyTransform.gameObject.GetComponent<MonoBehaviour>());
        }
    }

    public void Test(Vector3 location = new Vector3(), Color color = default(Color))
    {
        Instantiate(DummyPrefab, (Vector3)location, Quaternion.identity);
    }
}
