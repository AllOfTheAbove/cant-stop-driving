using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SlowTile : NetworkBehaviour {

    private bool alreadyUsed = false;

    public GameObject grandeAiguille;
    public GameObject petiteAiguille;
    public float speed;

    private void Update()
    {
        grandeAiguille.transform.Rotate(0, speed * Time.deltaTime, 0);
        petiteAiguille.transform.Rotate(0, 0, speed / 12 * Time.deltaTime);
    }

    public void Enter()
    {
        if (!alreadyUsed)
        {
            GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<Driver>().EnableCheckForDeath(false);
        }
    }

    public void Exit()
    {
        if (!alreadyUsed)
        {
            GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<Driver>().EnableCheckForDeath(true);
            alreadyUsed = true;
        }
    }
}
