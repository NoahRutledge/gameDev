using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCrafter : MonoBehaviour {

    [Header("Set in Inspector")]
    public int numClouds = 40;
    public GameObject cloudPrefab;
    public Vector3 cloudPosMin = new Vector3(-50, -5, 10);
    public Vector3 cloudPosMax = new Vector3(150, 100, 10);
    public float cloudScaleMin = 1f;
    public float cloudScaleMax = 3f;
    public float cloudSpeedMult = 0.5f;

    private GameObject[] cloudInstances;

    private void Awake()
    {
        cloudInstances = new GameObject[numClouds];

        GameObject anchor = GameObject.Find("CloudAnchor");

        GameObject cloud;
        for(int i = 0; i < numClouds; i++)
        {
            //create new cloud
            cloud = Instantiate<GameObject>(cloudPrefab);
            //position the cloud
            Vector3 cPos = Vector3.zero;
            cPos.x = Random.Range(cloudPosMin.x, cloudPosMax.x);
            cPos.y = Random.Range(cloudPosMin.y, cloudPosMax.y);

            //scale cloud
            float scaleU = Random.value;
            float scaleVal = Mathf.Lerp(cloudScaleMin, cloudScaleMax, scaleU);
            //smaller clouds (scaleU) should be lower to the ground
            cPos.y = Mathf.Lerp(cloudPosMin.y, cPos.y, scaleU);

            //smaller clouds should also be farther away
            cPos.z = 100 - 90 * scaleU;

            cloud.transform.position = cPos;
            cloud.transform.localScale = Vector3.one * scaleVal;
            cloud.transform.SetParent(anchor.transform);
            cloudInstances[i] = cloud;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach(GameObject cloud in cloudInstances)
        {
            float scaleVal = cloud.transform.localScale.x;
            Vector3 cPos = cloud.transform.position;

            //move cloud pos based on scale
            cPos.x -= scaleVal * Time.deltaTime * cloudSpeedMult;

            //if gone too far left
            if(cPos.x <= cloudPosMin.x)
            {
                cPos.x = cloudPosMax.x;
            }

            cloud.transform.position = cPos;
        }
	}
}
