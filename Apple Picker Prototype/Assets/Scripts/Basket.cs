using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Basket : MonoBehaviour {

    public Text scoreGT;

	// Use this for initialization
	void Start () {
        GameObject scoreGO = GameObject.Find("ScoreCounter");
        scoreGT = scoreGO.GetComponent<Text>();
        scoreGT.text = "0";
	}
	
	// Update is called once per frame
	void Update () {
        //get current mouse position
        Vector3 mousePos2D = Input.mousePosition;

        //sets the z position (which is the depth) to a static amount.  Can be set anywhere.
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
        Vector3 pos = this.transform.position;
        pos.x = mousePos3D.x;
        this.transform.position = pos;
	}

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collideWith = collision.gameObject;
        if(collideWith.tag == "Apple")
        {
            Destroy(collideWith);

            int score = int.Parse(scoreGT.text);
            score += 100;
            scoreGT.text = score.ToString();

            if(score > HighScore.score)
            {
                HighScore.score = score;
            }
        }
    }
}
