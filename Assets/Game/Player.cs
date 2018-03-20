using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    protected Game game;
    public bool isSingleplayer = false;

    [SyncVar] public int gameState = 0;
    [SyncVar] public int score = 0;

    public void Awake()
    {
        game = Game.Instance;
    }

    public void Ready()
    {
        game.GetScene().waitingUI.SetActive(false);
    }
    [Command] public void CmdReady()
    {
        RpcReady();
    }
    [ClientRpc] public void RpcReady()
    {
        Ready();
    }

    [Command] public void CmdAddScore()
    {
        score += 5;
        RpcUpdateScore(score);
    }
    [ClientRpc] void RpcUpdateScore(int newScore)
    {
        game.GetScene().scoreLabel.GetComponent<Text>().text = newScore.ToString();
    }

}
