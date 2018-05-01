using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RoofTile : NetworkBehaviour {

    public int scorePoints = 50;

    private bool alreadyUsed = false;

    public void IncreaseScore()
    {
        if(isServer && !alreadyUsed) {
            GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<Driver>().IncreaseScore(scorePoints);
            alreadyUsed = true;
        }
    }
}
