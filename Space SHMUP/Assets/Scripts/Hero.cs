using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    static public Hero S;

    [Header("Set in Inspector")]
    public float speed = 30f;
    public float rollMult = -45f;
    public float pitchMult = 30f;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40f;

    [Header("Set Dynamically")]
    public float _shieldLevel = 1;

    private GameObject lastTriggerGo = null;

    private void Awake()
    {
        if(S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S");
        }
    }
	
	// Update is called once per frame
	void Update () {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TempFire();
        }
	}

    public void TempFire()
    {
        GameObject projGo = Instantiate<GameObject>(projectilePrefab);
        projGo.transform.position = transform.position;
        Rigidbody rigidBd = projGo.GetComponent<Rigidbody>();
        rigidBd.velocity = Vector3.up * projectileSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //Debug.Log("Triggered: " + go.name);

        //if the last time we were hit was from this enemy, then ignore it
        if(go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;

        if(go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }
        else
        {
            Debug.Log("Triggered by non-enemy: " + go.name);
        }
    }

    public float shieldLevel
    {
        get
        {
            return _shieldLevel;
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if(value < 0)
            {
                Destroy(this.gameObject);

                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }
}
