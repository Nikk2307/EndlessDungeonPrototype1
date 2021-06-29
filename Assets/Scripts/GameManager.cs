using System.Collections;
using UnityEngine;

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
    Transform players;

    [SerializeField]
    int noOfDepthsToBuild = 1;

    [SerializeField]
    EnemyDataConfig baseSkeletonWarrior;
    
    [SerializeField]
    EnemyDataConfig baseSkeletonArcher;

    int depth = 0;
    int nextDepthToBuild = 0;
    float zGenerationOffsetIncrement = 60f;

    bool canMove = true;

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
        GenerateDungeon();
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

        Collider collHit = (Physics.OverlapSphere(players.position, 10f, 1<<6).Length>0) ? Physics.OverlapSphere(players.position, 10f, 1 << 6)[0] : null;

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