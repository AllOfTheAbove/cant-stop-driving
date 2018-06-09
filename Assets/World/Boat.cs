using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Boat : NetworkBehaviour {

    public Vector3 direction;
    public float velocity = 5;
    public float distanceToDespawn = 100;

    private System.Random random = new System.Random();

    public void Start() {
        if(!isServer)
        {
            return;
        }

        Vector3 driverPosition = GameObject.FindGameObjectsWithTag("Driver")[0].transform.position;
        int d = random.Next(3);
        int shift = 0;
        if(d == 0)
        {
            direction = new Vector3(0, 0, velocity);
            shift = random.Next(30) + 16;
        } else if(d == 1)
        {
            direction = new Vector3(0, 0, -velocity);
            shift = random.Next(30) + 16;
            transform.Rotate(0, 180, 0);
        } else if(d == 2)
        {
            direction = new Vector3(0, 0, velocity);
            shift = -random.Next(30) - 16;
        } else if(d == 3)
        {
            direction = new Vector3(0, 0, -velocity);
            shift = -random.Next(30) - 16;
            transform.Rotate(0, 180, 0);
        }

        transform.position = new Vector3(driverPosition.x + shift, transform.position.y, driverPosition.z);;

        CmdTellDirection(direction, transform.position);
    }

    [Command] public void CmdTellDirection(Vector3 direction, Vector3 position)
    {
        RpcTellDirection(direction, position);
    }
    [ClientRpc] public void RpcTellDirection(Vector3 direction, Vector3 position)
    {
        transform.position = position;
        this.direction = direction;
    }
	
	void Update () {
        if(direction == null)
        {
            return;
        }

        GetComponent<Rigidbody>().velocity = direction;

        float distance = Vector3.Distance(GameObject.FindGameObjectsWithTag("Driver")[0].transform.position, transform.position);
        if(distance > distanceToDespawn)
        {
            GameScene.Instance.currentNumberOfBoats--;
            NetworkServer.Destroy(gameObject);
        }
    }
}
