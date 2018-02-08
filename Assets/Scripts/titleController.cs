using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class titleController : MonoBehaviour {

    public NetworkManager networkManager;

    public void playSameComputer()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("game_SameComputer");
    }

    public void createLocalGame()
    {
        networkManager.StartHost();
    }

    public void joinLocalGame()
    {
        networkManager.StartClient();
    }

}
