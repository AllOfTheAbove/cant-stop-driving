using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    public bool isSingleplayer = false;

    public void RestartGame()
    {
        if (!isSingleplayer)
        {
            CmdRestartGame();
        }
        else
        {
            NetworkManager.singleton.ServerChangeScene("game");
            Game.Instance.Reset();
        }
    }
    [Command] public void CmdRestartGame()
    {
        NetworkManager.singleton.ServerChangeScene("game");
        Game.Instance.Reset();
    }

    [Command] public void CmdChangeGameState(int state)
    {
        RpcChangeGameState(state);
    }
    [ClientRpc] public void RpcChangeGameState(int state)
    {
        Game.Instance.ChangeState(state);
    }

}
