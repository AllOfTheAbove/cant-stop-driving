using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ButtonsManager : MonoBehaviour {

    public NetworkManager networkManager;

    public void changeToScene (string id)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(id);
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
