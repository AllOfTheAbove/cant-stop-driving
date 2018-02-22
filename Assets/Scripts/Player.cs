using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    [SyncVar]
    public int gameState = 0;

    [SyncVar]
    public int score = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    [Command]
    public void CmdDriverJoined()
    {
        gameState = 1;
        RpcDriverJoined(gameState);
    }
    [ClientRpc]
    public void RpcDriverJoined(int newGameState)
    {
        Debug.Log("Game can start!");
        if(GameObject.Find("Waiting"))
        {
            GameObject.Find("Waiting").SetActive(false);
        }
    }

    [Command]
    public void CmdAddScore()
    {
        score += 5;
        RpcUpdateScore(score);
    }
    [ClientRpc]
    void RpcUpdateScore(int newScore)
    {
        GameObject.Find("ScoreLabel").GetComponent<Text>().text = newScore.ToString();
    }

}
