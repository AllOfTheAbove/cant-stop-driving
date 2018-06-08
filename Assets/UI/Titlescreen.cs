using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Titlescreen : MonoBehaviour {

    void Start()
    {
        Game.Instance.SwitchAudio("title");
        Game.Instance.Reset();
        Vehicles.SpawnPreviewVehicle();
    }

    public void Singleplayer()
    {
        Game.Instance.GetServer().singleplayer = true;
        Game.Instance.GetServer().StartHost();
    }

    public void About()
    {
        Application.OpenURL("http://cant-stop-driving.com/");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
