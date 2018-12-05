using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum action
{
    idle,
    waiting,
    moving,
    start,
    end
}

public class Player : MonoBehaviour {
    public static Player S;

    public BlockStack currentBlock;
    public action state; 
    public float movementTime;

    private float moveStartTime;
    private BlockStack moveTo;
    private Vector3 midPoint;
    private Vector3 endPoint;
    private bool callOnce;

	// Use this for initialization
	void Awake () {
        S = this;
        state = action.start;
        currentBlock = IceHopping.S.getStart();
	}

    public void init()
    {
        state = action.start;
        if(currentBlock == null)
        {
            currentBlock = IceHopping.S.getStart();
        }
        transform.position = new Vector3(currentBlock.transform.localPosition.x, currentBlock.transform.position.y + currentBlock.blocks.Count - (1 - Block.heightOffset), currentBlock.transform.localPosition.z);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    // Update is called once per frame
    void Update () {
        if (currentBlock.isEnd && IceHopping.S.stacksLeft() == 1 && state == action.idle)
        {
            Debug.Log("End reached");
            state = action.end;
            Invoke("nextLevel", 0.7f);
        }

        //check movement keys
        if(state == action.idle)
        {
            GetComponent<Rigidbody>().isKinematic = false;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentBlock.left != null)
                {
                    moveTo = currentBlock.left;
                    movementStart();
                }
                else
                {
                    midPoint = new Vector3(transform.position.x - .75f, transform.position.y + 0.5f, transform.position.z);
                    endPoint = new Vector3(transform.position.x - 1.25f, -.13f, transform.position.z);
                    state = action.moving;
                    moveStartTime = Time.time;
                }
            }
            else if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                //if moving right is valid...
                if(currentBlock.right != null)
                {
                    moveTo = currentBlock.right;
                    movementStart();
                }
                //if its not valid, set the point yourself and start the timer
                else
                {
                    midPoint = new Vector3(transform.position.x + .75f, transform.position.y + 0.5f, transform.position.z);
                    endPoint = new Vector3(transform.position.x + 1.25f, -.13f, transform.position.z);
                    state = action.moving;
                    moveStartTime = Time.time;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentBlock.down != null)
                {
                    moveTo = currentBlock.down;
                    movementStart();
                }
                else
                {
                    midPoint = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z - .75f);
                    endPoint = new Vector3(transform.position.x, -.13f, transform.position.z - 1.25f);
                    state = action.moving;
                    moveStartTime = Time.time;
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentBlock.up != null)
                {
                    moveTo = currentBlock.up;
                    movementStart();
                }
                else
                {
                    midPoint = new Vector3(transform.position.x , transform.position.y + 0.5f, transform.position.z + .75f);
                    endPoint = new Vector3(transform.position.x, -.13f, transform.position.z + 1.25f);
                    state = action.moving;
                    moveStartTime = Time.time;
                }
            }
        }

        if(state == action.moving)
        {
            float u = (Time.time - moveStartTime) / movementTime;
            if(u > 1)
            {
                //state = action.idle;
                currentBlock = moveTo;
                callOnce = false;
                moveTo = null;
                if(state == action.waiting)
                {
                    state = action.idle;
                }
                else
                {
                    state = action.waiting;
                }
                return;
            }
            else if(u < 0)
            {
                return;
            }
            else
            {
                if(u > .25 && !callOnce)
                {
                    //dont lower more if there are none left to lower
                    if (currentBlock.blocks.Count > 0)
                    {
                        currentBlock.lower();
                        callOnce = true;
                    }
                }
                Vector3 p1, p2;
                p1 = (1 - u) * transform.position + u * midPoint;
                p2 = (1 - u) * midPoint + u * endPoint;

                transform.position = (1 - u) * p1 + u * p2;
            }
        }
	}

    private void movementStart()
    {
        state = action.moving;

        float x = (currentBlock.transform.localPosition.x + moveTo.transform.localPosition.x) / 2;
        float z = (currentBlock.transform.localPosition.z + moveTo.transform.localPosition.z) / 2;
        float y = Mathf.Max(transform.position.y, moveTo.blocks.Count) + 1f;
        midPoint = new Vector3(x, y, z);
        endPoint = new Vector3(moveTo.transform.localPosition.x, moveTo.blocks.Count - (1 - Block.heightOffset), moveTo.transform.localPosition.z);

        moveStartTime = Time.time;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Water")
        {
            Debug.Log("Hit");
            state = action.end;
            Invoke("Restart", 0.7f);
        }
    }

    public void Restart()
    {
        IceHopping.S.Restart();
    }

    public void nextLevel()
    {
        Debug.Log("Starting next level");
        IceHopping.S.nextLevel();
    }
    
}
