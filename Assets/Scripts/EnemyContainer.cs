using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int maxEnemies { get; set; }
    public int totalCoins { get; set; }
    
    void SpawnEnemies(string []levelParams)
    {
        ParseLevelParams(levelParams);
        int numberOfEnemies = Random.Range(1, maxEnemies);
        for(int i = 0; i < numberOfEnemies; i++)
        {
            Enemy enemy = Instantiate(enemyPrefab, transform).GetComponent<Enemy>();
            
            typeof(Enemy).GetProperty("coinsToSpawn").SetValue(enemy, (int)totalCoins / numberOfEnemies);
        }
    }

    void ParseLevelParams(string []levelParams)
    {
        foreach(string param in levelParams)
        {
            string []paramParts = param.Split(':');
            
            int p = int.Parse(paramParts[1]);
            typeof(EnemyContainer).GetProperty(paramParts[0]).SetValue(this, p);
        }
    }
}
