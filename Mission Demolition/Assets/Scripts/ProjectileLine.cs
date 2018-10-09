using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLine : MonoBehaviour {
    static public ProjectileLine S;

    public float minDist = 0.1f;

    private LineRenderer line;
    private GameObject _poi;
    private List<Vector3> points;

    private void Awake()
    {
        S = this;

        //get reference to the line renderer
        line = GetComponent<LineRenderer>();
        //disable it until needed
        line.enabled = false;

        points = new List<Vector3>();
    }

    public GameObject poi
    {
        get
        {
            return _poi;
        }
        set
        {
            _poi = value;
            if(_poi != null)
            {
                //when it is set to something new, it resets everything
                line.enabled = false;
                points = new List<Vector3>();
                AddPoint();
            }
        }
    }

    public void Clear()
    {
        _poi = null;
        line.enabled = false;
        points = new List<Vector3>();
    }

    public void AddPoint()
    {
        Vector3 pt = _poi.transform.position;

        //if there already exists a point and the current point minus the previous is large enough of a distance
        if(points.Count > 0 && (pt - lastPoint).magnitude < minDist)
        {
            return;
        }

        if(points.Count == 0) //if this is the launch point
        {
            Vector3 launchPosDiff = pt - Slingshot.LAUNCH_POS;

            points.Add(pt + launchPosDiff);
            points.Add(pt);

            line.positionCount = 2;
            line.SetPosition(0, points[0]);
            line.SetPosition(1, points[1]);
            line.enabled = true;
        }
        else
        {
            points.Add(pt);
            line.positionCount = points.Count;
            line.SetPosition(points.Count - 1, lastPoint);
            line.enabled = true;
        }
    }
    

    public Vector3 lastPoint
    {
        get
        {
            if (points == null)
            {
                return Vector3.zero;
            }
            return points[points.Count - 1];
        }
    }

    private void FixedUpdate()
    {
        if(poi == null)
        {
            //if there is no poi then search for one
            if(FollowCam.POI != null)
            {
                if(FollowCam.POI.tag == "Projectile")
                {
                    poi = FollowCam.POI;
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        

        AddPoint();
        if(FollowCam.POI == null)
        {
            //once the followcam poi is null, make the local poi null too
            poi = null;
        }
    }
}
