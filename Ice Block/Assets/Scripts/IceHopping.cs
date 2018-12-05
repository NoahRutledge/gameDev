using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class IceHopping : MonoBehaviour {

    public static IceHopping S;

    public GameObject blockStackPrefab;
    public GameObject playerPrefab;
    public int currentLevel = 1;
    public Transform blockAnchor;
    public List<BlockStack> blockStacks = new List<BlockStack>();
    public BlockStack[,] blockGrid;
    public float loadTime = 1f;

    private float loadLife;
    private Player player;
    private bool load = false;
    private bool unload = false;

    private void Awake()
    {
        S = this;
    }

    // Use this for initialization
    void Start() {
        LevelInfo.INIT();
    }

    public void InitDone()
    {
        loadLevel(currentLevel);
    }

    public void loadLevel(int n)
    {
        blockAnchor = new GameObject("BlockAnchor").transform;
        blockAnchor.position = new Vector3(0, -15, 0);

        Level level = LevelInfo.S.getLevel(n);
        blockGrid = new BlockStack[level.size, level.size];
        for (int i = 0; i < level.size; i++)
        {
            for (int j = 0; j < level.size; j++)
            {
                if (level.field[i, j] > 0)
                {
                    //create stack object and set parent
                    GameObject go = Instantiate<GameObject>(blockStackPrefab);
                    go.transform.parent = blockAnchor;
                    BlockStack bs = go.GetComponent<BlockStack>();
                    bs.transform.localPosition = new Vector3(i * 1.25f, 0, j * 1.25f);

                    //check if start or end before adding blocks
                    if (i == level.start.x && Mathf.Sqrt(level.field.Length) - 1 - j == level.start.y)
                    {
                        bs.isStart = true;
                    }
                    if (i == level.end.x && Mathf.Sqrt(level.field.Length) - 1 - j == level.end.y)
                    {
                        bs.isEnd = true;
                    }
                    //create ice blocks
                    bs.createBlocks(level.field[i, j]);

                    //save
                    blockStacks.Add(bs);
                    blockGrid[i, j] = bs;
                }
            }
        }

        //printGrid();

        SetNeighbors();
        
        Camera.main.transform.position = new Vector3(level.start.x + 2, 8, -6);

        GameObject pGO = Instantiate<GameObject>(playerPrefab);
        player = pGO.GetComponent<Player>();
        player.transform.parent = blockAnchor;
        player.init();

        load = true;
        loadLife = Time.time;
    }

    public void printGrid()
    {
        for (int i = 0; i < Mathf.Sqrt(blockGrid.Length); i++)
        {
            string line = "Line #" + i + ": ";
            for (int j = 0; j < Mathf.Sqrt(blockGrid.Length); j++)
            {
                if (blockGrid[i, j] == null)
                {
                    line += "0 ";
                }
                else
                {
                    line += blockGrid[i, j].blocks.Count + " ";
                }
            }
            Debug.Log(line);
        }
    }

    public void SetNeighbors()
    {
        for (int i = 0; i < (int)Mathf.Sqrt(blockGrid.Length); i++)
        {
            for (int j = 0; j < (int)Mathf.Sqrt(blockGrid.Length); j++)
            {
                //if the spot is a valid spot
                if (blockGrid[i, j] != null)
                {
                    //check left if valid
                    if (i - 1 >= 0 && i - 1 < Mathf.Sqrt(blockGrid.Length))
                    {
                        if (blockGrid[i - 1, j] != null)
                        {
                            blockGrid[i, j].left = blockGrid[i - 1, j];
                        }
                    }

                    //check right if valid
                    if (i + 1 >= 0 && i + 1 < Mathf.Sqrt(blockGrid.Length))
                    {
                        if (blockGrid[i + 1, j] != null)
                        {
                            blockGrid[i, j].right = blockGrid[i + 1, j];
                        }
                    }

                    //check down if valid
                    if (j - 1 >= 0 && j - 1 < Mathf.Sqrt(blockGrid.Length))
                    {
                        if (blockGrid[i, j - 1] != null)
                        {
                            blockGrid[i, j].down = blockGrid[i, j - 1];
                        }
                    }

                    //check up if valid
                    if (j + 1 >= 0 && j + 1 < Mathf.Sqrt(blockGrid.Length))
                    {
                        if (blockGrid[i, j + 1] != null)
                        {
                            blockGrid[i, j].up = blockGrid[i, j + 1];
                        }
                    }
                }
            }
        }
    }

    public BlockStack getStart()
    {
        foreach(BlockStack bs in blockStacks)
        {
            if (bs.isStart)
            {
                return bs;
            }
        }
        return null;
    }

    public void removeStack(BlockStack bs)
    {
        blockStacks.Remove(bs);
    }

    public int stacksLeft()
    {
        return blockStacks.Count;
    }

	// Update is called once per frame
	void Update () {
        if (unload)
        {
            float u = (Time.time - loadLife) / loadTime;
            if(u > 1)
            {
                unload = false;
                //once the level is unloaded, reset the data for next level
                reloadLevel();
            }
            else
            {
                blockAnchor.position = (1 - u) * blockAnchor.position + u * (new Vector3(0, -15, 0));
                //player.transform.position = (1 - u) * player.transform.position + u * (new Vector3(0, -15, 0));
            }
        }
        else if (load)
        {
            float u = (Time.time - loadLife) / loadTime;
            if(u > .4)
            {
                load = false;
                player.state = action.idle;
                //player.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                blockAnchor.position = (1 - u) * blockAnchor.position + u * (new Vector3(0, 0, 0));
            }
        }
	}

    public void reloadLevel()
    {
        Destroy(blockAnchor.gameObject);
        blockStacks = new List<BlockStack>();
        loadLevel(currentLevel);
    }

    public void unloadLevel()
    {
        loadLife = Time.time;
        unload = true;
    }

    public void Restart()
    {
        unloadLevel();
    }

    public void nextLevel()
    {
        Debug.Log("Incrementing level");
        currentLevel++;
        Restart();
    }
}
