using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    public bool isSingleplayer = false;

    [SyncVar] public int gameState = 0;
    [SyncVar] public int score = 0;

    IEnumerator Countdown()
    {
        int time = 3;

        while(true)
        {
            yield return new WaitForSeconds(1);
            if (time == 0)
            {
                GameScene.Instance.countdownEndSound.Play();
                GameScene.Instance.countdownLabel.GetComponent<TextMeshProUGUI>().SetText("GO");
            } else if (time < 0) {
                GameScene.Instance.countdownLabel.SetActive(false);
                StopCoroutine(Countdown());
            } else
            {
                GameScene.Instance.countdownBeepSound.Play();
                GameScene.Instance.countdownLabel.GetComponent<TextMeshProUGUI>().SetText("" + time);
            }
            time--;
        }
    }

    public void Ready()
    {
        GameScene.Instance.waitingUI.SetActive(false);
        StartCoroutine(Countdown());
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
        GameScene.Instance.scoreLabel.GetComponent<Text>().text = newScore.ToString();
    }

}
