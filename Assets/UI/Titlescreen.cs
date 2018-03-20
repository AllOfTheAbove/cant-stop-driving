using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Titlescreen : MonoBehaviour {

    void OnGUI()
    {
        /**Server s = GameObject.Find("Server").GetComponent<Server>();
        if (GUILayout.Button("Host local game"))
        {
            s.StartHost();
        }
        if (GUILayout.Button("Join local game"))
        {
            s.StartClient();
        }**/
    }

    void Start()
    {
        GameObject.Find("Game").GetComponent<Game>().gamePaused = false;
        Time.timeScale = 1f;
    }

    public void Singleplayer()
    {
        Server s = GameObject.Find("Server").GetComponent<Server>();
        s.singleplayer = true;
        s.StartHost();
    }

    public void Quit()
    {
        Application.Quit();
    }

}
