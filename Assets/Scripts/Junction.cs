using UnityEngine;
using System.Collections.Generic;

public class Junction : MonoBehaviour
{

    [SerializeField]
    Transform[] spawnPoints;

    EnemyDataConfig[] enemyDatas;

    int[] spawnPointsUsed;

    [SerializeField]
    Dictionary<float, Enemy> enemies;

    int finalSpawnIndex;

    public void PrepareJunction()
    {
        enemies = new Dictionary<float, Enemy>();

        //count will depend on difficulty and depth
        enemyDatas = GameManager.instance.GenerateEnemies(Random.Range(1, 4));

        spawnPointsUsed = new int[]{0,0,0};

        for (int i = 0; i < enemyDatas.Length; i++)
        {
            Transform spawnPointToUse = GetRandomSpawnPoint();
            GameObject tempEnemy = Instantiate(enemyDatas[i].enemyPrefab,spawnPointToUse.position,spawnPoints[i].rotation,transform);
            enemies.Add(finalSpawnIndex, tempEnemy.GetComponent<Enemy>());
        }
    }

    public Dictionary<float,Enemy> GetEnemies()
    {
        return enemies;
    }

    Transform GetRandomSpawnPoint()
    {
        int spawnPointIndex = Random.Range(0, 3);

        finalSpawnIndex = 0;

        Transform tempSpawnPointTransform = null;

        if (spawnPointsUsed[spawnPointIndex]==0)
        {
            tempSpawnPointTransform = spawnPoints[spawnPointIndex];
            spawnPointsUsed[spawnPointIndex] = 1;
            finalSpawnIndex = spawnPointIndex;
        }
        else
        {
            tempSpawnPointTransform = GetRandomSpawnPoint();
        }

        return tempSpawnPointTransform;
    }

}