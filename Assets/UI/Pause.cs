using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pause : MonoBehaviour {

    public void Resume()
    {
        GameObject.Find("Game").GetComponent<Game>().Pause();
    }

    public void Menu()
    {
        GameObject.Find("Server").GetComponent<Server>().Disconnect();
    }

    public void Quit()
    {
        Application.Quit();
    }

}
