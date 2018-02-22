using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : NetworkManager
{
    public int playerId = 0;
    public int gameId = 0;
    public GameObject[] prefabs;

    private NetworkConnection architectConnection;
    private GameObject architect;
    private short architectControllerId;

    private NetworkConnection driverConnection;
    private GameObject driver;
    private short driverControllerId;

    public override void OnClientConnect(NetworkConnection conn)
    {
        ClientScene.AddPlayer(conn, 0);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        this.StopHost();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (gameId != 0)
        {
            NetworkServer.Destroy(architect);
            architect = Instantiate(prefabs[0]);
            NetworkServer.ReplacePlayerForConnection(architectConnection, architect, architectControllerId);

            NetworkServer.Destroy(driver);
            driver = Instantiate(prefabs[1]);
            NetworkServer.ReplacePlayerForConnection(driverConnection, driver, driverControllerId);
        }

        gameId++;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if(playerId == 0)
        {
            Debug.Log("Architect joined!");
            architectConnection = conn;
            architectControllerId = playerControllerId;
            architect = Instantiate(prefabs[0]);
            NetworkServer.AddPlayerForConnection(architectConnection, architect, architectControllerId);
        } else if(playerId == 1)
        {
            Debug.Log("Driver joined!");
            driverConnection = conn;
            driverControllerId = playerControllerId;
            driver = Instantiate(prefabs[1]);
            NetworkServer.AddPlayerForConnection(driverConnection, driver, driverControllerId);
        } else
        {
            return;
        }

        playerId++;
    }

}
