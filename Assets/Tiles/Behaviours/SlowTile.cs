using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTile : MonoBehaviour {

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
