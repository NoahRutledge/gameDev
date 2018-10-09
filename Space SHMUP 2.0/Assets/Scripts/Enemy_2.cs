using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy {
    [Header("Set in Inspector: Enemy_2")]
    public float sinEccentricity = 0.6f;
    public float lifeTime = 10f;

    [Header("Set Dynamically: Enemy_2")]
    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;

	// Use this for initialization
	void Start () {
        //pick a random point on the left side of the screen
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //pick a random point on the right side of the screen
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //possibly swap sides
        if (Random.value > 0.5f)
        {
            p0.x *= -1;
            p1.x *= -1;
        }

        birthTime = Time.time;
	}

    public override void Move()
    {
        //bezier curves work with a u 0 - 1
        float u = (Time.time - birthTime) / lifeTime;
        if(u > 1)
        {
            //u has become bigger than the lifecycle of the object set
            Destroy(this.gameObject);
            return;
        }
        //adjust u by adding a U curve based on a sin wave
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));

        //interpolate the two linear points
        pos = (1 - u) * p0 + u * p1;
    }
}
