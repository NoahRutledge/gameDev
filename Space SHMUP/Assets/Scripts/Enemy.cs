using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10f;
    public int score = 100;

    private BoundsCheck bndCheck;

    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }

    // Update is called once per frame
    void Update () {
        Move();

        if(bndCheck != null && bndCheck.offDown)
        {       
            Destroy(gameObject);
        }
	}

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    public void OnTriggerEnter(Collider collision)
    {
        GameObject otherGO = collision.gameObject;
        if(otherGO.tag == "ProjectileHero")
        {
            Destroy(otherGO);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
