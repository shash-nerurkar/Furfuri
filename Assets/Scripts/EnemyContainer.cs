using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour
{
    public GameObject enemyPrefab;
    void SpawnEnemies(int level)
    {
        for(int i = 0; i < Random.Range(0, 3); i++)
        {
            Instantiate(enemyPrefab, transform);
        }
    }
}
