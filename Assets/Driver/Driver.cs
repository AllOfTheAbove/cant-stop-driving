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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        base.Update();

        if (!isSingleplayer && !isLocalPlayer)
        {
            GetComponent<CarController>().enabled = false;
            return;
        }
        GetComponent<CarController>().enabled = true;

        //currentSpeedModifier = Input.GetAxis("Vertical") * maxSpeedModifier;
        //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, defaultSpeed + currentSpeedModifier);

        if (Input.GetKey(KeyCode.R) || transform.position.y < -8) // || (rb.velocity.z / defaultSpeed) < 0.85
        {
            Death();
        }
    }

    private void Death()
    {
        NetworkManager.singleton.ServerChangeScene("game");
        if (!isSingleplayer)
        {
            CmdDeath();
        }
    }
    private void Death2()
    {
        this.rb.velocity = Vector3.zero;
        this.rb.angularVelocity = Vector3.zero;
        this.rb.isKinematic = true;
        this.transform.position = new Vector3(0, 4, -20);
        this.transform.rotation = Quaternion.identity;
        this.rb.isKinematic = false;
    }
    [Command] public void CmdDeath()
    {
        RpcDeath();
    }
    [ClientRpc] public void RpcDeath()
    {
        Death2();
    }

}
