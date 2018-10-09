using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {

    [Header("Set in Inspector")]
    public GameObject cloudSphere;
    public int numSpheresMin = 6;
    public int numSpheresMax = 10;
    public Vector3 sphereOffsetScale = new Vector3(5, 2, 1);
    //really just min and maxes
    public Vector2 sphereScaleRangeX = new Vector2(4, 8);
    public Vector2 sphereScaleRangeY = new Vector2(3, 4);
    public Vector2 sphereScaleRangeZ = new Vector2(2, 4);
    public float scaleYMin = 2f;

    private List<GameObject> spheres;

	// Use this for initialization
	void Start () {
        spheres = new List<GameObject>();

        //generate number of spheres in a cloud
        int num = Random.Range(numSpheresMin, numSpheresMax);
        for(int i = 0; i < num; i++)
        {
            //create cloud sphere and add to list
            GameObject sp = Instantiate<GameObject>(cloudSphere);
            spheres.Add(sp);

            //set parent to this
            Transform spTrans = sp.transform;
            spTrans.SetParent(this.transform);

            //randomly assign location
            Vector3 offset = Random.insideUnitSphere;
            offset.x *= sphereOffsetScale.x;
            offset.y *= sphereOffsetScale.y;
            offset.z *= sphereOffsetScale.z;
            spTrans.localPosition = offset;

            //randomly assign a scale
            Vector3 scale = Vector3.one;
            scale.x = Random.Range(sphereScaleRangeX.x, sphereScaleRangeX.y);
            scale.y = Random.Range(sphereScaleRangeY.x, sphereScaleRangeY.y);
            scale.z = Random.Range(sphereScaleRangeZ.x, sphereScaleRangeZ.y);

            //The further away the cloud sphere is from the center, the flatter it is.
            scale.y *= 1 - (Mathf.Abs(offset.x) / sphereOffsetScale.x);
            //But not too flat
            scale.y = Mathf.Max(scale.y, scaleYMin);

            spTrans.localScale = scale;

        }
	}
	
	// Update is called once per frame
	void Update () {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Restart();
        }
        */
	}

    void Restart()
    {
        foreach(GameObject sp in spheres)
        {
            Destroy(sp);
        }
        Start();
    }
}
