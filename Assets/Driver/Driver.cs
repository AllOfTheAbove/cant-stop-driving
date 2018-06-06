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
    public AudioSource engineSound;
    public AudioSource idleSound;
    public AudioSource brakeSound;
    public float minSpeedToBeInDanger = 8f;
    public float secondsAtMinSpeedBeforeDeath = 3f;
    public float durationRewind = 5f;
    public bool checkForDeath = true;
    public int vehicleId;

    public List<Vector3> lastPositions;
    public List<Quaternion> lastRotations;
    private float secondsLeftBeforeDeath = 0;
    private Coroutine checkForDeathCoroutine;
    private int[] maximumPositionsReached = new int[3];


    [Command] public void CmdSpawnVehicle(int vehicleId)
    {
        GameObject _vehicle = Instantiate(Game.Instance.vehicles[vehicleId]);
        NetworkServer.SpawnWithClientAuthority(_vehicle, gameObject);
        RpcSpawnVehicle(_vehicle, vehicleId);
    }
    [ClientRpc] public void RpcSpawnVehicle(GameObject _vehicle, int _vehicleId)
    {
        vehicle = _vehicle;
        _vehicle.transform.parent = gameObject.transform;
        vehicleId = _vehicleId;
    }
    public override void OnStartLocalPlayer()
    {
        Game.Instance.ChangeState(-1);
        CmdSpawnVehicle(Game.Instance.currentVehicleId);
        GetComponentInChildren<AudioListener>().enabled = true;
        CmdChangeGameState(0);
        checkForDeathCoroutine = StartCoroutine(CheckForDeath());
        lastPositions = new List<Vector3>();
        lastRotations = new List<Quaternion>();
    }

    private void Start()
    {
        if (isSingleplayer)
        {
            vehicle = Instantiate(Game.Instance.vehicles[Game.Instance.currentVehicleId]);
            vehicle.transform.parent = gameObject.transform;
            vehicleId = Game.Instance.currentVehicleId;
            Game.Instance.ChangeState(0);
            checkForDeathCoroutine = StartCoroutine(CheckForDeath());
            lastPositions = new List<Vector3>();
            lastRotations = new List<Quaternion>();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(Game.Instance.state == 1 && (col.gameObject.name.Contains("ramp") || col.gameObject.name.Contains("pillars")))
        {
            GameScene.Instance.collisionTileSound.Play();
        }
    }

    private void FixedUpdate()
    {
        if (Game.Instance.state == 1)
        {
            if (lastPositions.Count > durationRewind / Time.fixedDeltaTime)
            {
                lastPositions.RemoveAt(0);
                lastRotations.RemoveAt(0);
            }
            lastPositions.Add(transform.position);
            lastRotations.Add(transform.rotation);
        }
    }

    void Update()
    {
        if(vehicle == null)
        {
            return;
        }

        if(!Game.Instance.paused && Game.Instance.state == 1)
        {
            GameScene.Instance.speedLabel.GetComponent<TextMeshProUGUI>().SetText(Mathf.Floor(GetComponent<Rigidbody>().velocity.sqrMagnitude) + "");
        }

        engineSound.pitch = 0.5f + Mathf.Floor(GetComponent<Rigidbody>().velocity.sqrMagnitude) / 1500;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            brakeSound.Play();
        }

        // Pause
        if (Input.GetKeyDown(KeyCode.Escape) && !isSingleplayer && isLocalPlayer)
        {
            Game.Instance.Pause();
        }

        if (Game.Instance.paused)
        {
            vehicle.GetComponent<Vehicle>().enabled = false;
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
            IncreaseScore(1);
        }

        // Death
        if (transform.position.y < -7)
        {
            GameOver();
        }
    }



    public void IncreaseScore(int points)
    {
        if (!isSingleplayer)
        {
            CmdIncreaseScore(points);
        }
        else
        {
            Game.Instance.score += points;
            GameScene.Instance.scoreLabel.GetComponent<Text>().text = Game.Instance.score + "";
        }
    }
    [Command] public void CmdIncreaseScore(int points)
    {
        Game.Instance.score += points;
        RpcIncreaseScore(Game.Instance.score);
    }
    [ClientRpc] public void RpcIncreaseScore(int score)
    {
        Game.Instance.score = score;
        GameScene.Instance.scoreLabel.GetComponent<Text>().text = score + "";
    }

    public void EnableCheckForDeath(bool state)
    {
        if(isSingleplayer)
        {
            checkForDeath = state;
        } else
        {
            CmdEnableCheckForDeath(state);
        }
    }
    [Command] public void CmdEnableCheckForDeath(bool state)
    {
        RpcEnableCheckForDeath(state);
    }
    [ClientRpc] public void RpcEnableCheckForDeath(bool state)
    {
        checkForDeath = state;
    }
    public IEnumerator CheckForDeath()
    {
        yield return new WaitForSecondsRealtime(10);

        secondsLeftBeforeDeath = secondsAtMinSpeedBeforeDeath;

        while (secondsLeftBeforeDeath > 0)
        {
            if (Game.Instance.state == 1 && checkForDeath)
            {
                if (GetComponent<Rigidbody>().velocity.sqrMagnitude < minSpeedToBeInDanger)
                {
                    if(isSingleplayer)
                    {
                        GameScene.Instance.warningLabel.SetActive(true);
                        GameScene.Instance.warningLabel.GetComponent<TextMeshProUGUI>().SetText("Exploding in " + secondsLeftBeforeDeath);
                        GameScene.Instance.dangerSound.Play();
                    } else
                    {
                        CmdExplodeSoon(true, secondsLeftBeforeDeath);
                    }
                    secondsLeftBeforeDeath--;
                }
                else
                {
                    secondsLeftBeforeDeath = secondsAtMinSpeedBeforeDeath;
                    if(isSingleplayer)
                    {
                        GameScene.Instance.warningLabel.SetActive(false);
                    } else
                    {
                        CmdExplodeSoon(false, 0f);
                    }
                }
            }
            yield return new WaitForSecondsRealtime(1);
        }

        if(secondsLeftBeforeDeath <= 0)
        {
            if(isSingleplayer)
            {
                Game.Instance.state = 2;
                StartCoroutine(Game.Instance.FadeOutAudio(Game.Instance.gameMusics[Game.Instance.currentMusicId]));
                engineSound.Stop();
                idleSound.Stop();
                vehicle.GetComponent<Vehicle>().Explode();
                GameScene.Instance.explosionSound.Play();
            } else
            {
                CmdExplodeSoon(true, -1);
            }
            GameScene.Instance.warningLabel.GetComponent<TextMeshProUGUI>().SetText("");
            if (vehicleId == 0)
            {
                yield return new WaitForSecondsRealtime(2);
            }
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
            if(seconds == -1)
            {
                Game.Instance.state = 2;
                StartCoroutine(Game.Instance.FadeOutAudio(Game.Instance.gameMusics[Game.Instance.currentMusicId]));
                engineSound.Stop();
                idleSound.Stop();
                vehicle.GetComponent<Vehicle>().Explode();
                GameScene.Instance.explosionSound.Play();
                return;
            }
            GameScene.Instance.warningLabel.SetActive(true);
            GameScene.Instance.warningLabel.GetComponent<TextMeshProUGUI>().SetText("Explode in " + seconds);
            GameScene.Instance.dangerSound.Play();
        } else
        {
            GameScene.Instance.warningLabel.SetActive(false);
        }
    }

    public void GameOver()
    {
        if(Game.Instance.state >= 1)
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
