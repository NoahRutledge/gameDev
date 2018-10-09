using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part
{
    public string name;
    public float health;
    public string[] protectedBy;

    [HideInInspector]
    public GameObject go;
    [HideInInspector]
    public Material mat;
}

public class Enemy_4 : Enemy {

    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts;

    private Vector3 p0, p1;
    private float timeStart;
    private float duration = 4;

	// Use this for initialization
	void Start () {
        p0 = p1 = pos;

        InitMovement();

        Transform t;
        foreach(Part prt in parts)
        {
            t = transform.Find(prt.name);
            if(t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
	}
	
    void InitMovement()
    {
        //set p0 to the old point
        p0 = p1;
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;

        //get a new point to interpolate to 
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        //reset lifetime time
        timeStart = Time.time;
    }

    public override void Move()
    {
        //check if the time that has passed is longer than the duration
        float u = (Time.time - timeStart) / duration;

        //if it is then get a new point to interpolate to and reset 0
        if(u >= 1)
        {
            InitMovement();
            u = 0;
        }

        //applies a non-linear movement
        u = 1 - Mathf.Pow(1 - u, 2);
        pos = (1 - u) * p0 + u * p1;
    }

    Part FindPart(string n)
    {
        foreach(Part prt in parts)
        {
            if(prt.name == n)
            {
                return prt;
            }
        }
        return null;
    }

    Part FindPart(GameObject go)
    {
        foreach(Part prt in parts)
        {
            if(prt.go == go)
            {
                return prt;
            }
        }
        return null;
    }

    bool Destroyed(GameObject go)
    {
        return Destroyed(FindPart(go));
    }

    bool Destroyed(string n)
    {
        return Destroyed(FindPart(n));
    }

    bool Destroyed(Part prt)
    {
        if(prt == null)
        {
            return true;
        }
        return prt.health <= 0;
    }

    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }
}
