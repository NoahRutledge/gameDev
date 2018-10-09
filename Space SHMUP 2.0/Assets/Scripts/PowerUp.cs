using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    [Header("Set in Inspector")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f;
    public float fadeTime = 4f;

    [Header("Set Dynamically")]
    public WeaponType type;
    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    private void Awake()
    {
        cube = transform.Find("Cube").gameObject;
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();

        //set random velocity
        Vector3 vel = Random.onUnitSphere;
        vel.z = 0;
        vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        //set to no rotation
        transform.rotation = Quaternion.identity;

        //set rotPerSecond for the cube child using rotMinMax
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y));

        birthTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        if(u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        if(u > 0)
        {
            Color c = cubeRend.material.color;
            //reduce alpha by proportional time passed
            c.a = 1f - u;
            cubeRend.material.color = c;

            //fade the letter too
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }

        if (!bndCheck.isOnScreen)
        {
            Destroy(gameObject);
        }
	}

    public void SetType(WeaponType wt)
    {
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        cubeRend.material.color = def.color;
        letter.color = def.color;
        letter.text = def.letter;
        type = wt;
    }

    public void AbsorbedBy(GameObject target)
    {
        //this function is called by the hero class when a powerup is collected
        //we could use this to modify the transform when collected or to make special effects
        Destroy(this.gameObject);
    }
}
