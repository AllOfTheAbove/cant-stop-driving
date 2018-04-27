using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Titlescreen : MonoBehaviour {

    void OnGUI()
    {
        if (Game.Instance.debug)
        {
            if (GUILayout.Button("Host local game"))
            {
                Game.Instance.GetServer().StartHost();
            }
            if (GUILayout.Button("Join local game"))
            {
                Game.Instance.GetServer().StartClient();
            }
        }
    }

    void Start()
    {
        Game.Instance.SwitchAudio("title");
        Game.Instance.Reset();
    }

    public void Singleplayer()
    {
        Game.Instance.GetServer().singleplayer = true;
        Game.Instance.GetServer().StartHost();
    }

    public void Quit()
    {
        Application.Quit();
    }

}
