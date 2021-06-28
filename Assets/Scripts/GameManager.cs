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
    int noOfDepthsToBuild = 1;

    int depth = 0;
    int nextDepthToBuild = 0;
    float zGenerationOffset = -15f;
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
            GameObject tile3 = Instantiate(junctionTile, worldTransform.forward * (worldTransform.position.z + zGenerationOffsetIncrement * 2 + nextDepthToBuild * zGenerationOffsetIncrement * 3), transform.rotation, worldTransform);
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
    }
}