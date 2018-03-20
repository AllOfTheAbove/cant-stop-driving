using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Driver : Player
{
    private Rigidbody rb;
    private float currentSpeedModifier = 0;

    public int defaultSpeed = 10;
    public float maxSpeedModifier = 1.2f;

    public override void OnStartLocalPlayer()
    {
        CmdReady();
    }

    void Start()
    {
        Ready();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isSingleplayer && !isLocalPlayer)
        {
            GetComponent<CarController>().enabled = false;
            return;
        }
        GetComponent<CarController>().enabled = true;

        if (Input.GetKeyDown(KeyCode.Escape) && !isSingleplayer)
        {
            game.Pause();
        }

        //currentSpeedModifier = Input.GetAxis("Vertical") * maxSpeedModifier;
        //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, defaultSpeed + currentSpeedModifier);

        if (Input.GetKey(KeyCode.R) || transform.position.y < -8) // || (rb.velocity.z / defaultSpeed) < 0.85
        {
            if (!isSingleplayer)
            {
                CmdDeath();
            } else
            {
                NetworkManager.singleton.ServerChangeScene("game");
            }
        }
    }

    [Command] public void CmdDeath()
    {
        NetworkManager.singleton.ServerChangeScene("game");
    }

}
