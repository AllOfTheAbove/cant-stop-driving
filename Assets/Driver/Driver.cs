using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Driver : Player
{
    public GameObject vehicle;
    public Camera driverCamera;
    public float minSpeedToBeInDanger = 8f;
    public float secondsAtMinSpeedBeforeDeath = 3f;

    private float secondsLeftBeforeDeath = 0;
    private Coroutine checkForDeathCoroutine;
    private int[] maximumPositionsReached = new int[3];



    public override void OnStartLocalPlayer()
    {
        GetComponentInChildren<AudioListener>().enabled = true;
        CmdChangeGameState(0);
    }

    void Start()
    {
        Game.Instance.ChangeState(0);
        checkForDeathCoroutine = StartCoroutine(CheckForDeath());
    }

    void Update()
    {
        // Quit if not localPlayer or if countdown
        if ((!isSingleplayer && !isLocalPlayer) || Game.Instance.state == 0)
        {
            vehicle.GetComponent<CarController>().enabled = false;
            return;
        }
        vehicle.GetComponent<CarController>().enabled = true;

        // Pause
        if (Input.GetKeyDown(KeyCode.Escape) && !isSingleplayer)
        {
            Game.Instance.Pause();
        }

        // Score
        bool scoreIncreased = false;
        if(Math.Floor(transform.position.z) > maximumPositionsReached[1])
        {
            maximumPositionsReached[1] += 1;
            scoreIncreased = true;
        }
        if(Math.Floor(transform.position.x) > maximumPositionsReached[2])
        {
            maximumPositionsReached[2] += 1;
            scoreIncreased = true;
        }
        if (Math.Ceiling(transform.position.x) < maximumPositionsReached[0])
        {
            maximumPositionsReached[0] -= 1;
            scoreIncreased = true;
        }
        if(scoreIncreased)
        {
            IncreaseScore();
        }

        // Death
        if (Input.GetKey(KeyCode.R) || transform.position.y < -8)
        {
            GameOver();
        }
    }



    public void IncreaseScore()
    {
        if (!isSingleplayer)
        {
            CmdIncreaseScore();
        }
        else
        {
            Game.Instance.score += 1;
            GameScene.Instance.scoreLabel.GetComponent<Text>().text = Game.Instance.score + "";
        }
    }
    [Command] public void CmdIncreaseScore()
    {
        Game.Instance.score += 1;
        RpcIncreaseScore(Game.Instance.score);
    }
    [ClientRpc] public void RpcIncreaseScore(int score)
    {
        Game.Instance.score = score;
        GameScene.Instance.scoreLabel.GetComponent<Text>().text = score + "";
    }


    public IEnumerator CheckForDeath()
    {
        secondsLeftBeforeDeath = secondsAtMinSpeedBeforeDeath;

        while (secondsLeftBeforeDeath > 0)
        {
            if (Game.Instance.state == 1)
            {
                if (GetComponent<Rigidbody>().velocity.sqrMagnitude < minSpeedToBeInDanger)
                {
                    // alert beeeeeep
                    secondsLeftBeforeDeath--;
                }
                else
                {
                    secondsLeftBeforeDeath = secondsAtMinSpeedBeforeDeath;
                }
            }
            yield return new WaitForSeconds(1);
        }

        if(secondsLeftBeforeDeath <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        StopCoroutine(checkForDeathCoroutine);
        if (!isSingleplayer)
        {
            CmdChangeGameState(2);
        } else
        {
            Game.Instance.ChangeState(2);
        }
    }

}
