using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    Warrior,
    Ranger,
    Tank
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

    [SerializeField]
    float playerRotateSpeed;

    [SerializeField]
    float stoppingDistance;

    int depth = 0;
    int nextDepthToBuild = 0;
    float zGenerationOffsetIncrement = 60f;

    bool canMove = true;

    bool inCombat = true;

    int rotating;

    PlayerType playerType;

    Vector3 targetPosition;

    GameObject playerPrefabToSpawn, tempGO;

    Player targetPlayerToMove;

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
            players.Add(i, tempGO.GetComponent<Player>());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
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
            //Initiate Combat
        }
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