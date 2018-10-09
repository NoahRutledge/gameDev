using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour {
    static private MissionDemolition S;

    [Header("Set in Inspector")]
    public Text uitLevel;
    public Text uitShots;
    public Text uitButton;
    public Vector3 castlePos;
    public GameObject[] castles;

    [Header("Set Dynamically")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";

	// Use this for initialization
	void Start () {
        S = this;

        level = 0;
        levelMax = castles.Length;
        StartLevel();
	}

    void StartLevel()
    {
        //if there is already a castle set then destroy all pieces of it
        if (castle != null)
        {
            Destroy(castle);
        }

        //destroy all projectiles
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }

        //instantiate the next castle and put it at the pos set in inspector
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;
        shotsTaken = 0;

        //switch view to show both and clear the projectile lines
        SwitchView("Show Both");
        ProjectileLine.S.Clear();

        //set goalMet to false
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;
    }

    void UpdateGUI()
    {
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;
    }
	
	// Update is called once per frame
	void Update () {
        //update the gui each frame
        UpdateGUI();

        //if the game is being played and the goal has been met
        if((mode == GameMode.playing) && Goal.goalMet)
        {
            mode = GameMode.levelEnd;
            SwitchView("Show Both");
            Invoke("NextLevel", 2f);
        }
	}

    void NextLevel()
    {
        //increase level count and start next level
        level++;
        if(level == levelMax)
        {
            level = 0;
        }
        StartLevel();
    }

    public void SwitchView(string eView = "")
    {
        if(eView == "")
        {
            eView = uitButton.text;
        }

        showing = eView;
        switch (showing)
        {
            case "Show Slingshot":
                FollowCam.POI = null;
                uitButton.text = "Show Castle";
                break;

            case "Show Castle":
                FollowCam.POI = S.castle;
                uitButton.text = "Show Both";
                break;

            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                uitButton.text = "Show Slingshot";
                break;
        }
    }

    public static void ShotFired()
    {
        S.shotsTaken++;
    }

    public static Vector3 MaxView()
    {
        return GameObject.Find("StopPoint").transform.position;
    }
}
