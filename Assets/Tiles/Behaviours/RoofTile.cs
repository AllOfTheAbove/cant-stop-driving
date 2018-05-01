using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofTile : MonoBehaviour {

    public int scorePoints = 50;

    private bool alreadyUsed = false;

    void OnTriggerEnter(Collider collider)
    {
        if(!alreadyUsed) {
            GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<Driver>().IncreaseScore(scorePoints);
            alreadyUsed = true;
        }
    }
}
