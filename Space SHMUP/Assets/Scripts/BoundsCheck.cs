using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps a GameObject on screen.
/// This ONLY works on an orthographic Main Camera at [0, 0, 0]
/// </summary>

public class BoundsCheck : MonoBehaviour {
    [Header("Set in Inspector")]
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Set Dynamically")]
    public bool isOnScreen = true;
    public float camWidth;
    public float camHeight;
    public bool offRight, offLeft, offUp, offDown;

    private void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

	// Update is called once per frame
	void LateUpdate () {
        Vector3 pos = transform.position;
        isOnScreen = true;
        offRight = offLeft = offUp = offDown = false;

        /*BORDER PATROL*/
        //I didn't like that when enemies touched the bottom of the screen they were destroyed
        //But I couldn't simply just set the radius of enemies to a negative or else they would instantiate on screen.
        //So if the script is on an enemy (specified by keepOnScreen *assuming*) then when it checks and sets the off* booleans, it will feed it a negative radius

        //If it is the player then change nothing
        if (keepOnScreen)
        {
            pos = checkOnScreen(pos, radius);
        }
        else
        {
            pos = checkOnScreen(pos, -radius);
        }

        isOnScreen = !(offLeft || offRight || offUp || offDown);

        if(keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            isOnScreen = true;
            offLeft = offRight = offUp = offDown = false;
        }
	}

    private Vector3 checkOnScreen(Vector3 pos, float radius)
    {
        if (pos.x > camWidth - radius)
        {
            pos.x = camWidth - radius;
            offRight = true;
        }
        if (pos.x < -camWidth + radius)
        {
            pos.x = -camWidth + radius;
            offLeft = true;
        }
        if (pos.y > camHeight - radius)
        {
            pos.y = camHeight - radius;
            offUp = true;
        }
        if (pos.y < -camHeight + radius)
        {
            pos.y = -camHeight + radius;
            offDown = true;
        }

        return pos;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        //Draws the boundary in the scene inspector when the game is playing
        Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}
