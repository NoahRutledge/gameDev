using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour {
    static private Slingshot S;

    [Header("Set in Inspector")]
    public GameObject prefabProjectile;
    public float velocityMult = 8f;

    [Header("Set Dynamically")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    private Rigidbody projectileRigidBody;

    static public Vector3 LAUNCH_POS
    {
        get
        {
            if (S == null) return Vector3.zero;
            return S.launchPos;
        }
    }

    private void Awake()
    {
        S = this;

        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }

    //activate and deactivate halo
    private void OnMouseEnter()
    {
        launchPoint.SetActive(true);
    }

    private void OnMouseExit()
    {
        launchPoint.SetActive(false);
    }

    //The user has clicked while over the collider
    private void OnMouseDown()
    {
        aimingMode = true;
        projectile = Instantiate(prefabProjectile) as GameObject;
        projectile.transform.position = launchPos;
        projectileRigidBody = projectile.GetComponent<Rigidbody>();
        projectileRigidBody.isKinematic = true;
    }

    private void Update()
    {
        //if we aren't aiming don't do anything
        if (!aimingMode) return;

        //get mouse position and do weird stuff with the z that I don't get
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        //calculate delta for mouse position to launch position
        Vector3 mouseDelta = mousePos3D - launchPos;

        //get magnitude of radius
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        //if the mouse delta magnitude is larger than the max then normalize and multiply it by the max magnitude
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        //set projectile to where mouse is
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        if (Input.GetMouseButtonUp(0))
        {
            //set camera point of interest to the current bullet
            FollowCam.POI = projectile;
            //no longer aiming
            aimingMode = false;
            //is affected by physics
            projectileRigidBody.isKinematic = false;
            //set velocity to the opposite angle in which the mouse in multiplied by a set value
            projectileRigidBody.velocity = -mouseDelta * velocityMult;
            //remove the projectile from memory
            projectile = null;

            //set shot fired in main script and new projectile line
            MissionDemolition.ShotFired();
            ProjectileLine.S.poi = projectile;
        }
    }
}
