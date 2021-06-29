using UnityEngine;

public class Junction : MonoBehaviour
{

    [SerializeField]
    Transform[] spawnPoints;

    EnemyDataConfig[] enemyDatas;
    GameObject[] enemies;

    public void PrepareJunction()
    {
        //count will depend on difficulty and depth
        enemyDatas = GameManager.instance.GenerateEnemies(Random.Range(1, 4));
        enemies = new GameObject[enemyDatas.Length];

        for (int i = 0; i < enemies.Length; i++)
        {
            //Randomise spawn point later
            GameObject tempEnemy = Instantiate(enemyDatas[i].enemyPrefab,spawnPoints[i].position,spawnPoints[i].rotation,transform);
            enemies[i] = tempEnemy;
        }
    }

}