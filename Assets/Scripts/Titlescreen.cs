using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Titlescreen : MonoBehaviour {

    public void playSameComputer()
    {
        Server nm = GameObject.Find("Server").GetComponent<Server>();
        nm.StartHost();
        nm.StartClient();
    }

    public void createLocalGame()
    {
        Server nm = GameObject.Find("Server").GetComponent<Server>();
        nm.StartHost();
    }

    public void joinLocalGame()
    {
        Server nm = GameObject.Find("Server").GetComponent<Server>();
        nm.StartClient();
    }

    public void quit()
    {
        Application.Quit();
    }

}
