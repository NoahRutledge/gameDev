using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

    public void gameStart()
    {
        SceneManager.LoadScene("IceHopping");
    }

    public void gameQuit()
    {
        Debug.Log("ran game quit");
        Application.Quit();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
