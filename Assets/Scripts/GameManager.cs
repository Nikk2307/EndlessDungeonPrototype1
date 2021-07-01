using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    Warrior,
    Ranger,
    Tank,
    None
}

[System.Serializable]
public class PlayerPrefabs
{
    public GameObject warriorPrefab;
    public GameObject rangerPrefab;
    public GameObject tankPrefab;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    GameObject safeZoneTile;

    [SerializeField]
    GameObject junctionTile;

    [SerializeField]
    Transform worldTransform;

    [SerializeField]
    Transform playersCentre;

    [SerializeField]
    Transform[] playerPositions;

    [SerializeField]
    int noOfDepthsToBuild = 1;

    [SerializeField]
    PlayerType leftStartingPlayer;

    [SerializeField]
    PlayerType frontStartingPlayer;

    [SerializeField]
    PlayerType rightStartingPlayer;

    [SerializeField]
    EnemyDataConfig baseSkeletonWarrior;
    
    [SerializeField]
    EnemyDataConfig baseSkeletonArcher;

    [SerializeField]
    PlayerPrefabs playerPrefabs;

    [SerializeField]
    Dictionary<float, Player> players;

    Dictionary<float, Enemy> enemies;

    [SerializeField]
    float playerRotateSpeed;

    [SerializeField]
    float stoppingDistance;

    int depth = 0;
    int nextDepthToBuild = 0;
    float zGenerationOffsetIncrement = 60f;

    bool canMove = true;

    bool inCombat = false;

    int rotating;

    PlayerType playerType;

    Player tempPlayerToSpawn;

    Vector3 targetPosition;

    GameObject playerPrefabToSpawn, tempGO;

    Player targetPlayerToMove;

    float timeSinceCombat, combatStartTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        SpawnPlayers();
        GenerateDungeon();
    }

    void SpawnPlayers()
    {
        players = new Dictionary<float, Player>();

        for (int i = 0; i < playerPositions.Length; i++)
        {
            switch (i)
            {
                case 0:
                    playerType = leftStartingPlayer;
                    break;

                case 1:
                    playerType = frontStartingPlayer;
                    break;

                case 2:
                    playerType = rightStartingPlayer;
                    break;
            }

            switch (playerType)
            {
                case PlayerType.Warrior:
                    playerPrefabToSpawn = playerPrefabs.warriorPrefab;
                    break;

                case PlayerType.Ranger:
                    playerPrefabToSpawn = playerPrefabs.rangerPrefab;
                    break;

                case PlayerType.Tank:
                    playerPrefabToSpawn = playerPrefabs.tankPrefab;
                    break;
            }

            tempGO = Instantiate(playerPrefabToSpawn,playerPositions[i].position,playerPositions[i].rotation,playersCentre);
            tempPlayerToSpawn = tempGO.GetComponent<Player>();
            tempPlayerToSpawn.health = tempPlayerToSpawn.playerData.health;
            players.Add(i, tempPlayerToSpawn);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GenerateDungeon();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && canMove)
        {
            AdvanceDungeon();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && inCombat && rotating == 0)
        {
            Rotate();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && inCombat && rotating == 0)
        {
            Rotate(-1);
        }

        if (inCombat)
            TickCombat();
    }

    void TickCombat()
    {
        if (rotating == 0)
        {
            for (int i = 0; i < 3;i++)
            {
                if (enemies.ContainsKey(i) && enemies[i].health > 0) //Enemy exists and is alive
                {
                    //print(players[i].playerData.playerType.ToString() + " is fighting " + enemies[i].enemyData.enemyType.ToString());
                    if (players[i].playerData.playerType!=PlayerType.None && players[i].health > 0) //Player exists (Not Benched) and is alive
                    {
                        timeSinceCombat = Time.time - combatStartTime;

                        if ((players[i].lastTimeAttacked == 0 || (Mathf.Round((timeSinceCombat - players[i].lastTimeAttacked) * 100) / 100) >= players[i].playerData.attackRate)&&players[i].playerData.attackRate!=0)
                        {
                            players[i].lastTimeAttacked = timeSinceCombat;
                            DealDamage(players[i], enemies[i]);
                        }

                        if (enemies[i].lastTimeAttacked == 0 ||(Mathf.Round((timeSinceCombat - enemies[i].lastTimeAttacked)*100)/100)>=enemies[i].enemyData.attackRate)
                        {
                            enemies[i].lastTimeAttacked = timeSinceCombat;
                            DealDamage(enemies[i],players[i]);
                        }
                    }
                }
            }
        }
    }

    void DealDamage(Enemy enemy, Player targetPlayer)
    {
        targetPlayer.health -= enemy.enemyData.attackDamage;
        if (targetPlayer.health < 0)
            targetPlayer.health = 0;
        print(enemy.enemyData.enemyType+" attacked "+targetPlayer.playerData.playerType + "with " +enemy.enemyData.attackDamage+ "damage" + " health left: "+targetPlayer.health);
    }

    void DealDamage(Player player, Enemy targetEnemy)
    {
        targetEnemy.health -= player.playerData.attackDamage;
        if (targetEnemy.health < 0)
            targetEnemy.health = 0;
        print(player.playerData.playerType + " attacked " + targetEnemy.enemyData.enemyType + "with" +player.playerData.attackDamage + "damage" + " health left: " + targetEnemy.health);
    }

    void Rotate(int dir = 1)
    {
        rotating = 3;

        for (int i = 0; i < playerPositions.Length; i++)
        {
            if (dir > 0)
            {
                targetPosition = (i + dir == playerPositions.Length) ? playerPositions[0].position : playerPositions[i + dir].position;
            }
            else
            {
                targetPosition = (i + dir < 0) ? playerPositions[playerPositions.Length-1].position : playerPositions[i + dir].position;
            }
                
            targetPlayerToMove = players[i];
            players.Remove(i);
            StartCoroutine(MovePlayerToTargetLocation(i, targetPlayerToMove, targetPosition, dir));
        }
    }

    IEnumerator MovePlayerToTargetLocation(int index, Player targetPlayer, Vector3 targetPosition, int dir = 1)
    {
        while (Vector3.Distance(targetPlayer.transform.position,targetPosition)>stoppingDistance)
        {
            yield return new WaitForEndOfFrame();
            targetPlayer.transform.position = Vector3.Lerp(targetPlayer.transform.position, targetPosition, Time.deltaTime * playerRotateSpeed);
        }

        targetPlayer.transform.position = targetPosition;

        int newIndex = (dir>0) ? ((index + 1 == playerPositions.Length) ? 0 : index + 1 ) : ((index == 0) ? playerPositions.Length-1 : index - 1);

        players.Add(newIndex, targetPlayer);

        //print("Current Index: "+index+" Target Index: "+newIndex+" Player Name: "+targetPlayer.transform.name);

        rotating--;
    }

    void GenerateDungeon()
    {
        for (int i = 0; i < noOfDepthsToBuild; i++)
        {
            GameObject tile1 = Instantiate(safeZoneTile, worldTransform.forward * (worldTransform.position.z + nextDepthToBuild * zGenerationOffsetIncrement * 3), transform.rotation, worldTransform);

            GameObject tile2 = Instantiate(junctionTile, worldTransform.forward * (worldTransform.position.z + zGenerationOffsetIncrement + nextDepthToBuild * zGenerationOffsetIncrement * 3), transform.rotation, worldTransform);

            tile2.GetComponent<Junction>().PrepareJunction();

            GameObject tile3 = Instantiate(junctionTile, worldTransform.forward * (worldTransform.position.z + zGenerationOffsetIncrement * 2 + nextDepthToBuild * zGenerationOffsetIncrement * 3), transform.rotation, worldTransform);

            tile3.GetComponent<Junction>().PrepareJunction();

            nextDepthToBuild++;
        }
    }

    public void AdvanceDungeon()
    {
        canMove = false;
        inCombat = false;
        worldTransform.GetComponent<Animator>().SetTrigger("MakeProgress");
        StartCoroutine(EnableMove());
    }

    IEnumerator EnableMove()
    {
        yield return new WaitForSeconds(2f);
        canMove = true;

        Collider collHit = (Physics.OverlapSphere(playersCentre.position, 10f, 1<<6).Length>0) ? Physics.OverlapSphere(playersCentre.position, 10f, 1 << 6)[0] : null;

        if (collHit)
        {
            InitiateCombat(collHit.GetComponent<Junction>());
        }
    }

    void InitiateCombat(Junction junction)
    {
        enemies = junction.GetEnemies();

        foreach(KeyValuePair<float, Enemy> kvp in enemies)
        {
            kvp.Value.health = kvp.Value.enemyData.health;
        }

        inCombat = true;
        combatStartTime = Time.time;
    }

    public EnemyDataConfig[] GenerateEnemies(int count)
    {
        EnemyDataConfig[] enemiesToSpawn = new EnemyDataConfig[count];

        for (int i = 0; i < count; i++)
        {
            EnemyType enemyType = (Random.Range(0, 2) == 0) ? EnemyType.SkeletonWarrior : EnemyType.SkeletonArcher;
            EnemyDataConfig enemyData = (enemyType == EnemyType.SkeletonWarrior) ? baseSkeletonWarrior : baseSkeletonArcher;
            //Process for Depth and Difficulty
            enemiesToSpawn[i] = enemyData;
        }

        return enemiesToSpawn;
    }
}