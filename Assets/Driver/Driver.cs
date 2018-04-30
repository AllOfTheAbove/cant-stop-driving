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
    public float durationRewind = 5f;

    private int currentRewindPosition = 0;
    private List<Vector3> lastPositions;
    private List<Quaternion> lastRotations;
    private float secondsLeftBeforeDeath = 0;
    private Coroutine checkForDeathCoroutine;
    private int[] maximumPositionsReached = new int[3];



    public override void OnStartLocalPlayer()
    {
        GetComponentInChildren<AudioListener>().enabled = true;
        CmdChangeGameState(0);
        checkForDeathCoroutine = StartCoroutine(CheckForDeath());
        lastPositions = new List<Vector3>();
        lastRotations = new List<Quaternion>();
    }

    private void Start()
    {
        Game.Instance.ChangeState(-1);
        if (isSingleplayer)
        {
            Game.Instance.ChangeState(0);
            checkForDeathCoroutine = StartCoroutine(CheckForDeath());
            lastPositions = new List<Vector3>();
            lastRotations = new List<Quaternion>();
        }
    }

    private void FixedUpdate()
    {
        if(isLocalPlayer)
        {
            if(Game.Instance.state == 1)
            {
                if (lastPositions.Count > durationRewind / Time.fixedDeltaTime)
                {
                    lastPositions.RemoveAt(0);
                    lastRotations.RemoveAt(0);
                }
                lastPositions.Add(transform.position);
                lastRotations.Add(transform.rotation);
            } else if(Game.Instance.state == 2)
            {
                transform.position = lastPositions[currentRewindPosition];
                transform.rotation = lastRotations[currentRewindPosition];
                currentRewindPosition++;
                if (currentRewindPosition >= lastPositions.Count)
                {
                    currentRewindPosition = 0;
                }
            }
        }
    }

    void Update()
    {
        if(!Game.Instance.paused && Game.Instance.state == 1)
        {
            GameScene.Instance.speedLabel.GetComponent<TextMeshProUGUI>().SetText(Mathf.Floor(GetComponent<Rigidbody>().velocity.sqrMagnitude) + "");
        }

        // Pause
        if (Input.GetKeyDown(KeyCode.Escape) && !isSingleplayer)
        {
            Game.Instance.Pause();
        }

        // Quit if not localPlayer or if countdown
        if ((!isSingleplayer && !isLocalPlayer) || Game.Instance.state == 0)
        {
            vehicle.GetComponent<Vehicle>().enabled = false;
            return;
        }
        vehicle.GetComponent<Vehicle>().enabled = true;

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
                    secondsLeftBeforeDeath--;
                    CmdExplodeSoon(true, secondsLeftBeforeDeath);
                }
                else
                {
                    secondsLeftBeforeDeath = secondsAtMinSpeedBeforeDeath;
                    CmdExplodeSoon(false, 0f);
                }
            }
            yield return new WaitForSecondsRealtime(1);
        }

        if(secondsLeftBeforeDeath <= 0)
        {
            GameOver();
        }
    }
    [Command] public void CmdExplodeSoon(bool state, float seconds)
    {
        RpcExplodeSoon(state, seconds);
    }
    [ClientRpc] public void RpcExplodeSoon(bool state, float seconds)
    {
        if(state)
        {
            GameScene.Instance.warningLabel.SetActive(true);
            GameScene.Instance.warningLabel.GetComponent<TextMeshProUGUI>().SetText("Explode in " + seconds);
        } else
        {
            GameScene.Instance.warningLabel.SetActive(false);
        }
    }

    public void GameOver()
    {
        if(Game.Instance.state == 1)
        {
            StopCoroutine(checkForDeathCoroutine);
            GetComponent<Rigidbody>().isKinematic = true;
            if (!isSingleplayer)
            {
                CmdChangeGameState(2);
            }
            else
            {
                Game.Instance.ChangeState(2);
            }
        }
    }

}
