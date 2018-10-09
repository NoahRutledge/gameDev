using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10f;
    public int score = 100;
    public float showDamageDuration = 0.1f;
    public float powerUpDropChance = 1f;

    [Header("Set Dynamically")]
    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage = false;
    public float damageDoneTime;
    public bool notifiedOfDestruction = false;

    protected BoundsCheck bndCheck;

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

        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for(int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    // Update is called once per frame
    void Update () {
        Move();

        if(showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        if(bndCheck != null && bndCheck.offDown)
        {       
            Destroy(gameObject);
        }
	}

    //This function is declared as virtual because as the partent object, with classes that override functions, the parent function needs to be declared as virtual.
    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    public void OnTriggerEnter(Collider collision)
    {
        GameObject otherGO = collision.gameObject;
        switch (otherGO.tag) {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                //if the enemy is off screen then destroy the bullet
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                ShowDamage();
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if(health <= 0)
                {
                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    Destroy(gameObject);
                }
                Destroy(otherGO);
                break;

            default:
                Debug.Log("Enemy hit by non-ProjectileHero: " + otherGO.name);
                break;
            
        }
    }

    void ShowDamage()
    {
        foreach(Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage()
    {
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}