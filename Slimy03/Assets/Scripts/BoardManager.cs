using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.AI;


public class BoardManager : MonoBehaviour
{
    // [Serializable]
    // public class Count
    // {
    //     public int maximum;
    //     public int minimum;

    //     public Count(int min, int max)
    //     {
    //         maximum = max;
    //         minimum = min;
    //     }
    // }

    public int potMax;
    public int potMin;
    public int rockMax;
    public int rockMin;
    public int torchMax;
    public int torchMin;
    public NavMeshSurface navMeshSurface;
    public GameObject[] floorTiles;
    public GameObject[] PotTiles;
    public GameObject[] RockTiles;
    public GameObject[] enemyTiles;
    public GameObject[] demonTiles;
    public GameObject[] torchTiles;
    public GameObject outerWall;
    public GameObject outerWallW;
    public GameObject outerWallA;
    public GameObject outerWallS;
    public GameObject outerWallD;
    public GameObject outerWallWA;
    public GameObject outerWallAS;
    public GameObject outerWallSD;
    public GameObject outerWallWD;
    public GameObject[] outerWallWS;
    public GameObject[] outerWallAD;
    public GameObject outerWallAWD;
    public GameObject outerWallWAS;
    public GameObject outerWallASD;
    public GameObject outerWallWDS;
    public GameObject outerWallWASD;
    public GameObject outerOuterWall;
    public GameObject outerOuterWallCliff;
    public GameObject outerOuterWallCliffL;
    public GameObject outerOuterWallCliffR;
    public GameObject outerOuterWallCliffLR;
    public GameObject enter;
    public GameObject exit;
    public GameObject darkTile;
    public GameObject blurTile;
    public GameObject boss;

    private Transform boardHolder;
    private GameObject ground;
    private Vector3 enterPositon;
    private Vector3 exitPositon;
    private List<Vector3> gridPositions = new List<Vector3>();

    const int OUTEROUTERWALL = 1;
    const int FLOOR = 2;
    const int OUTERWALL = 3;
    const int ENTER = 4;
    const int EXIT = 5;

    //MapGenerator start from here

    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    int[,] map;
    Tile[,] tileMap;

    //-----------------------------------

    void InitialiseList()//获取可用tile的列表
    {
        gridPositions.Clear();

        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == 0)
                    {
                        gridPositions.Add(new Vector3(x, 0.0f, y));
                    }
                }
            }
        }
    }

    void CameraSetup()
    {
        //cam.transform.position = new Vector3(width / 2.0f - 0.5f, 10.0f, height / 2.0f - 0.5f);
        //cam.orthographicSize = height / 2.0f;
    }

    //放置地板、围墙、外围
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.SetParent(boardHolder);
        ground.transform.position = new Vector3(width / 2.0f - 0.5f, -0.1f, height / 2.0f - 0.5f);
        ground.transform.localScale = new Vector3(width / 10.0f, 1.0f, height / 10.0f);

        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject toInstantiate;
                    GameObject instance;
                    if (tileMap[x, y].type == FLOOR)
                    {
                        toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                        instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                        instance.transform.SetParent(boardHolder);
                    }
                    else if (tileMap[x, y].type == OUTERWALL)
                    {
                        if (x > 0 && x < width - 1 && y > 0 && y < height - 1 && tileMap[x + 1, y].type == OUTERWALL && tileMap[x - 1, y].type == OUTERWALL && tileMap[x, y + 1].type == OUTERWALL && tileMap[x, y - 1].type == OUTERWALL)
                        {
                            toInstantiate = outerWallWASD;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x > 0 && x < width - 1 && y < height - 1 && tileMap[x + 1, y].type == OUTERWALL && tileMap[x - 1, y].type == OUTERWALL && tileMap[x, y + 1].type == OUTERWALL)
                        {
                            toInstantiate = outerWallAWD;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x > 0 && x < width - 1 && y > 0 && tileMap[x + 1, y].type == OUTERWALL && tileMap[x - 1, y].type == OUTERWALL && tileMap[x, y - 1].type == OUTERWALL)
                        {
                            toInstantiate = outerWallASD;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x < width - 1 && y > 0 && y < height - 1 && x > 0 && x < width - 1 && y > 0 && y < height - 1 && tileMap[x + 1, y].type == OUTERWALL && tileMap[x, y + 1].type == OUTERWALL && tileMap[x, y - 1].type == OUTERWALL)
                        {
                            toInstantiate = outerWallWDS;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x > 0 && y > 0 && y < height - 1 && tileMap[x - 1, y].type == OUTERWALL && tileMap[x, y + 1].type == OUTERWALL && tileMap[x, y - 1].type == OUTERWALL)
                        {
                            toInstantiate = outerWallWAS;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x < width - 1 && y < height - 1 && tileMap[x + 1, y].type == OUTERWALL && tileMap[x, y + 1].type == OUTERWALL)
                        {
                            toInstantiate = outerWallWD;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x < width - 1 && y > 0 && tileMap[x + 1, y].type == OUTERWALL && tileMap[x, y - 1].type == OUTERWALL)
                        {
                            toInstantiate = outerWallSD;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x > 0 && y > 0 && tileMap[x - 1, y].type == OUTERWALL && tileMap[x, y - 1].type == OUTERWALL)
                        {
                            toInstantiate = outerWallAS;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x > 0 && y < height - 1 && tileMap[x - 1, y].type == OUTERWALL && tileMap[x, y + 1].type == OUTERWALL)
                        {
                            toInstantiate = outerWallWA;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x > 0 && x < width - 1 && tileMap[x + 1, y].type == OUTERWALL && tileMap[x - 1, y].type == OUTERWALL)
                        {
                            toInstantiate = outerWallAD[Random.Range(0, outerWallAD.Length)];
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (y > 0 && y < height - 1 && tileMap[x, y + 1].type == OUTERWALL && tileMap[x, y - 1].type == OUTERWALL)
                        {
                            toInstantiate = outerWallWS[Random.Range(0, outerWallWS.Length)];
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x < width - 1 && tileMap[x + 1, y].type == OUTERWALL)
                        {
                            toInstantiate = outerWallD;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x > 0 && tileMap[x - 1, y].type == OUTERWALL)
                        {
                            toInstantiate = outerWallA;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (y < height - 1 && tileMap[x, y + 1].type == OUTERWALL)
                        {
                            toInstantiate = outerWallW;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (y > 0 && tileMap[x, y - 1].type == OUTERWALL)
                        {
                            toInstantiate = outerWallS;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else
                        {
                            toInstantiate = outerWall;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                    }

                    else if (tileMap[x, y].type == OUTEROUTERWALL)
                    {
                        if(x>0&&x<width-1&&y < height - 1 && (tileMap[x, y+1].type == OUTERWALL || tileMap[x, y + 1].type == ENTER || tileMap[x, y + 1].type == EXIT) && tileMap[x+1, y + 1].type == OUTEROUTERWALL && tileMap[x - 1, y + 1].type == OUTEROUTERWALL)
                        {
                            toInstantiate = outerOuterWallCliffLR;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x<width-1 && y < height - 1 && x > 0 && (tileMap[x, y + 1].type == OUTERWALL || tileMap[x, y + 1].type == ENTER|| tileMap[x, y + 1].type == EXIT) && tileMap[x+1, y + 1].type == OUTEROUTERWALL)
                        {
                            toInstantiate = outerOuterWallCliffR;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (x>0&&y < height - 1 &&( tileMap[x, y + 1].type == OUTERWALL || tileMap[x, y + 1].type == ENTER|| tileMap[x, y + 1].type == EXIT) && tileMap[x - 1, y + 1].type == OUTEROUTERWALL)
                        {
                            toInstantiate = outerOuterWallCliffL;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else if (y < height - 1 && (tileMap[x, y + 1].type == OUTERWALL || tileMap[x, y + 1].type == ENTER || tileMap[x, y + 1].type == EXIT))
                        {
                            toInstantiate = outerOuterWallCliff;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                        else
                        {
                            toInstantiate = outerOuterWall;
                            instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                            instance.transform.SetParent(boardHolder);
                        }
                    }

                    else if (tileMap[x, y].type == ENTER)
                    {
                        toInstantiate = enter;
                        instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                        instance.transform.SetParent(boardHolder);

                        toInstantiate = outerOuterWall;
                        instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                        instance.transform.SetParent(boardHolder);
                    }

                    else if (tileMap[x, y].type == EXIT)
                    {
                        toInstantiate = exit;
                        instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                        instance.transform.SetParent(boardHolder);

                        toInstantiate = outerOuterWall;
                        instance = Instantiate(toInstantiate, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                        instance.transform.SetParent(boardHolder);
                    }
                    if(GameObject.Find("GameManager(Clone)").GetComponent<GameManager>().level <= 3)
                    {
                        //create dark layer
                        instance = Instantiate(darkTile, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        //create blur layer
                        instance = Instantiate(blurTile, new Vector3(x, 0.0f, y), Quaternion.Euler(90, 0, 0)) as GameObject;
                        instance.transform.SetParent(boardHolder);
                    }
                }
            }
        }
    }

    //返回一个随机的可用空间并将其从list删除
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    //将指定类型和个数范围的Tile随机放置于可用空间中
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            randomPosition.y = 0.5f;
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            GameObject newObject = Instantiate(tileChoice, randomPosition, Quaternion.Euler(90, 0, 0));
            if (newObject.tag == "Enemy")
                GameManager.instance.enemies.Add(newObject);
        }
    }

    void LayoutPlayer()
    {
        //player
        int index = 0;
        for(int i=0; i< gridPositions.Count; i++)
        {
            if (Mathf.Abs(gridPositions[i].x - enterPositon.x) < 0.1f && Mathf.Abs(gridPositions[i].z - enterPositon.z) < 1.1f)
            {
                index = i;
                break;
            }
        }
        Vector3 playerPosition = gridPositions[index];
        gridPositions.RemoveAt(index);

        GameObject.Find("Player").transform.position = new Vector3(playerPosition.x, 0.5f, playerPosition.z);
    }

    void LayoutFire()
    {
        int index = 0;
        for(int i=0; i< gridPositions.Count; i++)
        {
            if (Mathf.Abs(gridPositions[i].x - enterPositon.x) < 0.1f && Mathf.Abs(gridPositions[i].z - enterPositon.z) < 1.1f)
            {
                index = i;
                break;
            }
        }
        Vector3 playerPosition = gridPositions[index];
        gridPositions.RemoveAt(index);

        GameObject.Find("Player").transform.position = new Vector3(playerPosition.x, 0.5f, playerPosition.z);
    }

    public void SetupScene(int level)
    {
        GenerateMap();  //fill a int[] with 0 and 1
        GenerateTileMap();  //fill a Tile[] with FLOOR, OUTERWALL and OUTEROUTERWALL
        BoardSetup();
        CameraSetup();
        InitialiseList();
        LayoutPlayer();//放置玩家和出口的位置
        LayoutObjectAtRandom(PotTiles, potMin, potMax);
        LayoutObjectAtRandom(RockTiles, rockMin, rockMax);
        LayoutObjectAtRandom(torchTiles, torchMin, torchMax);
        //LayoutObjectAtRandom(Tiles, foodCount.minimum, foodCount.maximum);
        //int enemyCount = (int)Mathf.Log(level, 2f);
        int enemyCount = level;
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount+1);

        navMeshSurface.BuildNavMesh();
    }

    public void SetupBossScene()
    {
        GenerateBossMap();  //fill a int[] with 0 and 1
        GenerateTileMap();  //fill a Tile[] with FLOOR, OUTERWALL and OUTEROUTERWALL
        BoardSetup();
        CameraSetup();
        InitialiseList();
        LayoutPlayer();//放置玩家和出口的位置

        GameObject boss0 = Instantiate(boss, new Vector3(11f, 0, 9f), Quaternion.Euler(90, 0, 0));

        GameObject torch0 = Instantiate(torchTiles[0], new Vector3(8,0,11), Quaternion.Euler(90, 0, 0));
        GameObject torch1 = Instantiate(torchTiles[0], new Vector3(6,0,10), Quaternion.Euler(90, 0, 0));
        GameObject torch2 = Instantiate(torchTiles[0], new Vector3(4,0,9), Quaternion.Euler(90, 0, 0));
        GameObject torch3 = Instantiate(torchTiles[0], new Vector3(2,0,8), Quaternion.Euler(90, 0, 0));
        GameObject torch5 = Instantiate(torchTiles[0], new Vector3(14,0,11), Quaternion.Euler(90, 0, 0));
        GameObject torch6 = Instantiate(torchTiles[0], new Vector3(16,0,10), Quaternion.Euler(90, 0, 0));
        GameObject torch7 = Instantiate(torchTiles[0], new Vector3(18,0,9), Quaternion.Euler(90, 0, 0));
        GameObject torch8 = Instantiate(torchTiles[0], new Vector3(20,0,8), Quaternion.Euler(90, 0, 0));

        //LayoutObjectAtRandom(demonTiles, 4, 6);


        navMeshSurface.BuildNavMesh();
    }

    void GenerateTileMap()
    {
        tileMap = new Tile[width, height];
        List<Tile> doorTiles = new List<Tile>();

        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tileMap[x, y].positionX = x;
                    tileMap[x, y].positionY = y;

                    if (map[x, y] == 0)
                    {
                        tileMap[x, y].type = FLOOR;
                    }

                    if (map[x, y] == 1)
                    {
                        if ((x > 0 && map[x - 1, y] == 0) || (x < width - 1 && map[x + 1, y] == 0) || (y > 0 && map[x, y - 1] == 0) || (y < height - 1 && map[x, y + 1] == 0) || (x > 0 && y > 0 && map[x - 1, y - 1] == 0) || (x > 0 && y < height - 1 && map[x - 1, y + 1] == 0) || (x < width - 1 && y > 0 && map[x + 1, y - 1] == 0) || (x < width - 1 && y < height - 1 && map[x + 1, y + 1] == 0))
                        {
                            tileMap[x, y].type = OUTERWALL;
                        }
                        else
                        {
                            tileMap[x, y].type = OUTEROUTERWALL;
                        }
                    }
                }
            }

            //决定出入口
            if(GameObject.Find("GameManager(Clone)").GetComponent<GameManager>().level > 3)
            {
                tileMap[11,0].type = ENTER;
                enterPositon = new Vector3(11f, 0.0f, 0f);

                tileMap[11,12].type = EXIT;
                exitPositon = new Vector3(11f, 0.0f, 12f);
            }
            else
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (x > 1 && x < width - 2 && tileMap[x + 1, y].type == OUTERWALL && tileMap[x - 1, y].type == OUTERWALL && tileMap[x + 2, y].type == OUTERWALL && tileMap[x - 2, y].type == OUTERWALL && ((y < height - 1 && tileMap[x, y + 1].type == FLOOR) || (y > 0 && tileMap[x, y - 1].type == FLOOR)))
                            doorTiles.Add(tileMap[x, y]);
                    }
                }

                int index = Random.Range(0, doorTiles.Count);
                int enterX;
                int exitX;

                tileMap[doorTiles[index].positionX, doorTiles[index].positionY].type = ENTER;
                enterPositon = new Vector3(doorTiles[index].positionX, 0.0f, doorTiles[index].positionY);
                enterX = doorTiles[index].positionX;
                doorTiles.RemoveAt(index);

                index = Random.Range(0, doorTiles.Count);
                exitX = doorTiles[index].positionX;
                while (Mathf.Abs(enterX - exitX) < 1.1f)
                {
                    doorTiles.RemoveAt(index);
                    index = Random.Range(0, doorTiles.Count - 1);
                    exitX = doorTiles[index].positionX;
                }
                tileMap[doorTiles[index].positionX, doorTiles[index].positionY].type = EXIT;
                exitPositon = new Vector3(doorTiles[index].positionX, 0.0f, doorTiles[index].positionY);
            }
        }
    }

    void NavMeshSetup()
    {

    }

    struct Tile
    {
        public int type;
        public int positionX;
        public int positionY;

        public Tile(int x, int y, int t)
        {
            type = t;
            positionX =x;
            positionY = y;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void GenerateBossMap()
    {
        map = new int[23, 13] 
        {
            {1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,0,1,1,1},
            {1,0,0,0,0,0,0,0,0,0,1,1,1},
            {1,0,0,0,0,0,0,0,0,0,0,1,1},
            {1,0,0,0,0,0,0,0,0,0,0,1,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,1,1},
            {1,0,0,0,0,0,0,0,0,0,0,1,1},
            {1,0,0,0,0,0,0,0,0,0,1,1,1},
            {1,0,0,0,0,0,0,0,0,0,1,1,1},
            {1,0,0,0,0,0,0,0,0,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,1,1,1,1},
            {1,0,0,0,0,0,0,0,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1},
        };
    }

    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        //ProcessMap0();
        //ProcessMap1();
    }

    //Type为1(黑)的区域的集合中，对于Size小于阈值的区域，将这些区域的所有tile的Type置为0
    void ProcessMap0()
    {
        List<List<Coord>> roomRegions = GetRegions(1);

        int roomThresholdSize = 50;
        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }
    }

    //同上，只是白黑的处理逆转
    void ProcessMap1()
    {
        List<List<Coord>> roomRegions = GetRegions(0);

        int roomThresholdSize = 30;
        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    map[tile.tileX, tile.tileY] = 1;
                }
            }
        }
    }

    //返回了指定Type的所有连通区域的集合
    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)//遍历List的简单方法
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    //返回了一个包括着startX,startY这个点的连通区域的所有tiles
    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];
        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (x == tile.tileX || y == tile.tileY))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;

            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }
}
