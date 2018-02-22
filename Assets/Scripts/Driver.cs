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
        CmdDriverJoined();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        currentSpeedModifier = Input.GetAxis("Vertical") * maxSpeedModifier;
        rb.velocity = new Vector3(Input.GetAxis("Horizontal"), rb.velocity.y, defaultSpeed + currentSpeedModifier);

        if (transform.position.y < -8 || (rb.velocity.z / defaultSpeed) < 0.85)
        {
            CmdDeath();
        }
    }

    [Command]
    public void CmdDeath()
    {
        RpcDeath();
        NetworkManager.singleton.ServerChangeScene("game");
    }
    [ClientRpc]
    public void RpcDeath()
    {
        this.rb.velocity = Vector3.zero;
        this.rb.angularVelocity = Vector3.zero;
        this.rb.isKinematic = true;
        this.transform.position = new Vector3(0, 4, -20);
        this.transform.rotation = Quaternion.identity;
        this.rb.isKinematic = false;
        
    }

}
