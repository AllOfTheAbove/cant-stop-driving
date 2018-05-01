using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTile : MonoBehaviour {

    private bool alreadyUsed = false;

    void OnTriggerEnter(Collider collider)
    {
        if (!alreadyUsed)
        {
            GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<Driver>().checkForDeath = false;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (!alreadyUsed)
        {
            GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<Driver>().checkForDeath = true;
            alreadyUsed = true;
        }
    }
}
