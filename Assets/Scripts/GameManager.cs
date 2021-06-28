using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    #region Variables

    [Header("Tile Properties")]
    [SerializeField]
    private GameObject[] _tilePrefabs;                                                                                      //Insert Tile Prefabs Variable

    [SerializeField]
    private List<GameObject> _activeTiles = new List<GameObject>();                                                         //Variable to create list of active tiles in game

    [SerializeField]
    private float _spawnPoint = -15f;                                                                                         //Z-axis Spawn point of tiles

    [SerializeField]
    private int _currentIndex = 1;                                                                                          //Current Tile Index

    [SerializeField]
    private bool _inSafeZone = true;

    [SerializeField]
    private bool _isRotating = false;

    public bool _combatOn = false;

    [Header("Enemy Properties")]
    [SerializeField]
    private List<GameObject> _enemiesToKillList = new List<GameObject>();

    [SerializeField]
    private GameObject[] _meleeEnemiesPrefabs;

    [SerializeField]
    private GameObject[] _rangeEnemiesPrefabs;

    public GameObject[] _EnemySpawnPoint;

    [Header("Level Properties")]
    [SerializeField]
    private int _depthLevel = 0;                                                                                            //Level of Player

    [Header("References")]
    public GameObject _world;                                                                                               //Gameobject Reference of World

    public Animator _worldAnimator;                                                                                         //Animator reference of World

    #endregion

    void Awake()
    {
        SpawnDungeon(0);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))                                                                                    //Input to activate gameplay function
        {
            AdvanceDungeon();                                                                                               //Function to progress game
        }

        if (!_inSafeZone & !_isRotating & _worldAnimator.GetCurrentAnimatorStateInfo(0).IsName("TilesIdle"))
        {
            TickCombat();
        }
        else
        {
            _combatOn = false;
        }

        if(_enemiesToKillList.Count <= 0 & !_inSafeZone)
        {
            AdvanceDungeon();
        }
    }

    public void SpawnDungeon(int index)
    {
        GameObject tile;                                                                                                    //Creating temporary gameobject of spawned tile
        tile = Instantiate(_tilePrefabs[index], _world.transform.forward * _spawnPoint, _world.transform.rotation);         //Spawn Tile with exact position and rotation
        _activeTiles.Add(tile);                                                                                             //Adding spawned tile to list of active tiles
        tile.transform.SetParent(_world.transform);                                                                         //Setting parent of spawned tile to world gameobject
        _spawnPoint = 45;                                                                                                   //Adding offest to Spawn point(Z-axis)
    }

    public void AdvanceDungeon()
    {
        _worldAnimator.SetTrigger("MakeProgress");                                                                          //Moving Forward(Play Animation)
        TileManagement();                                                                                                   //Function that manages tile.
    }

    void TileManagement()
    {
        #region TileSwitch                                                                                                  

        switch (_currentIndex)                                                                                              //Switch Case that manages spawn of tiles
        {
            case 0:
            {
                SpawnDungeon(0);
                _currentIndex++;
                _depthLevel++;
                _inSafeZone = true;
                break;
            }
            case 1:
            {
                SpawnDungeon(1);
                _currentIndex++;
                SpawnEnemies();
                _inSafeZone = false;
                break;
            }
            case 2:
            {
                SpawnDungeon(1);
                _currentIndex = 0;
                SpawnEnemies();
                _inSafeZone = false;
                break;
            }
        }
        #endregion 

        #region DeleteTiles

        if (_activeTiles.Count >= 3)                                                                                        //Deleting Worthless Tiles
        {
            GameObject.Destroy(_activeTiles[0]);
            _activeTiles.RemoveAt(0);
        }

        #endregion
    }

    void SpawnEnemies()
    {
        _EnemySpawnPoint = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");

        foreach(GameObject SpawnPoint in _EnemySpawnPoint)
        {

            GameObject m_enemy;
            GameObject r_enemy;
            m_enemy = Instantiate(_meleeEnemiesPrefabs[0], SpawnPoint.transform.position, SpawnPoint.transform.rotation);
            r_enemy = Instantiate(_rangeEnemiesPrefabs[0], SpawnPoint.transform.position, SpawnPoint.transform.rotation);

            _enemiesToKillList.Add(m_enemy);
            _enemiesToKillList.Add(r_enemy);

            if(SpawnPoint.transform.name == "FrontSpawn")
            {
                m_enemy.transform.tag = "FrontEnemy";
                r_enemy.transform.tag = "FrontEnemy";
            }
            else if(SpawnPoint.transform.name == "LeftSpawn")
            {
                m_enemy.transform.tag = "LeftEnemy";
                r_enemy.transform.tag = "LeftEnemy";
            }
            else
            {
                m_enemy.transform.tag = "RightEnemy";
                r_enemy.transform.tag = "RightEnemy";
            }

            m_enemy.transform.SetParent(SpawnPoint.transform.parent);
            r_enemy.transform.SetParent(SpawnPoint.transform.parent);
            GameObject.Destroy(SpawnPoint);
        }
    }

    void TickCombat()
    {
        _combatOn = true;
    }
}