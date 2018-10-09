using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {

    public float easing = 0.05f;
    public Vector2 minXY = Vector2.zero;

    static public GameObject POI;
    public float camZ;

    private void Awake()
    {
        camZ = this.transform.position.z;
    }

    private void FixedUpdate()
    {
        //get projetile position
        Vector3 destination;

        if(POI == null)
        {
            destination = Vector3.zero;
        }
        else
        {
            destination = POI.transform.position;
            if(POI.tag == "Projectile")
            {
                if (POI.GetComponent<Rigidbody>().IsSleeping() || destination.x >= MissionDemolition.MaxView().x)
                {
                    POI = null;
                    return;
                }
            }
        }

        //limit the camera not to go further down or left of the min
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);

        //from point A to point B, set the destination to a weighted value between the two (easing which is a %)
        destination = Vector3.Lerp(transform.position, destination, easing);

        //keep camera same distance away
        destination.z = camZ;

        //set camera position to projectile position
        transform.position = destination;

        Camera.main.orthographicSize = destination.y + 10;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
