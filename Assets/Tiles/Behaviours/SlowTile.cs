using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SlowTile : NetworkBehaviour {

    private bool alreadyUsed = false;

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
