using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockStack: MonoBehaviour{
    [Header("Set Manually")]
    public GameObject blockPrefab;
    public GameObject finishTilePrefab;
    public float lowerSpeed;

    [Header("Set Dynamically")]
    public List<Block> blocks;
    public BlockStack right = null;
    public BlockStack up = null;
    public BlockStack down = null;
    public BlockStack left = null;
    public bool isStart = false;
    public bool isEnd = false;
    public Vector3 targetPos;

    private bool moving = false;
    private float moveLife;


    public void Start()
    {
        targetPos = transform.localPosition;
    }

    public void createBlocks(int num)
    {
        for(int i =0; i < num; i++)
        {
            //create and set position
            GameObject go = Instantiate<GameObject>(blockPrefab);
            go.transform.parent = this.transform;
            go.transform.localPosition = new Vector3(0, i, 0);

            blocks.Add(go.GetComponent<Block>());
        }
        //set color if end block
        if (isEnd)
        {
            //do something
            //go.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 255);
            GameObject tile = Instantiate<GameObject>(finishTilePrefab);
            tile.transform.parent = this.transform;
            tile.transform.localPosition = new Vector3(0, num-0.49f, 0);
        }
    }

    public void Update()
    {
        if(blocks.Count == 0)
        {
            IceHopping.S.removeStack(this);
        }

        if(moving)
        {
            float u = (Time.time - moveLife) / lowerSpeed;
            if(u > 1 && blocks.Count > 0)
            {
                blocks.RemoveAt(0);
                moving = false;
                if(Player.S.state == action.waiting)
                {
                    Player.S.state = action.idle;
                }
                else
                {
                    Player.S.state = action.waiting;
                }
            }
            else
            {
                transform.localPosition = (1 - u) * transform.localPosition + u * targetPos;
            }
        }
    }

    public void lower()
    {
        targetPos = new Vector3(transform.localPosition.x, transform.localPosition.y - 1.1f, transform.localPosition.z);
        moving = true;
        moveLife = Time.time;
    }
}
