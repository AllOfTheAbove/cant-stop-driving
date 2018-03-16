using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : NetworkManager
{
    public bool singleplayer = false;
    public int playerId = 0;
    public int gameId = 0;
    public GameObject[] prefabs;

    protected NetworkConnection architectConnection;
    protected GameObject architect;
    protected short architectControllerId;

    protected NetworkConnection driverConnection;
    protected GameObject driver;
    protected short driverControllerId;

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
            architect.name = prefabs[0].name;
            NetworkServer.ReplacePlayerForConnection(architectConnection, architect, architectControllerId);
            if (singleplayer)
            {
                NetworkServer.Destroy(driver);
                driver = Instantiate(prefabs[1]);
                driver.name = prefabs[1].name;

                driver.GetComponent<Player>().isSingleplayer = true;
                architect.GetComponent<Player>().isSingleplayer = true;
            }
            else
            {
                NetworkServer.Destroy(driver);
                driver = Instantiate(prefabs[1]);
                driver.name = prefabs[1].name;
                NetworkServer.ReplacePlayerForConnection(driverConnection, driver, driverControllerId);
            }
        }

        gameId++;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if(singleplayer)
        {
            SpawnArchitect(conn, playerControllerId);
            driver = Instantiate(prefabs[1]);

            driver.GetComponent<Player>().isSingleplayer = true;
            architect.GetComponent<Player>().isSingleplayer = true;
            driver.GetComponent<Player>().Ready();
            architect.GetComponent<Player>().Ready();
            return;
        }

        if(playerId == 0)
        {
            SpawnArchitect(conn, playerControllerId);
        } else if(playerId == 1)
        {
            SpawnDriver(conn, playerControllerId);
            driver.GetComponent<Player>().Ready();
            architect.GetComponent<Player>().Ready();
        } else
        {
            return;
        }

        playerId++;
    }

    private GameObject SpawnArchitect(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Architect joined!");
        architectConnection = conn;
        architectControllerId = playerControllerId;
        architect = Instantiate(prefabs[0]);
        architect.name = prefabs[0].name;
        NetworkServer.AddPlayerForConnection(architectConnection, architect, architectControllerId);
        return architect;
    }

    private GameObject SpawnDriver(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Driver joined!");
        driverConnection = conn;
        driverControllerId = playerControllerId;
        driver = Instantiate(prefabs[1]);
        driver.name = prefabs[1].name;
        NetworkServer.AddPlayerForConnection(driverConnection, driver, driverControllerId);
        return driver;
    }

}
