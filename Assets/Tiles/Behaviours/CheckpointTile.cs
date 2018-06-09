using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CheckpointTile : NetworkBehaviour {

    public GameObject flag;
    public GameObject pillar;
    public Material flagValidated;
    public int scorePoints = 10;

    private bool alreadyUsed = false;

    public void Checkpoint()
    {
        if (!alreadyUsed)
        {
            Material[] m = new Material[1];
            m[0] = flagValidated;
            flag.GetComponent<MeshRenderer>().materials = m;
            pillar.GetComponent<MeshRenderer>().materials = m;
            if(isServer)
            {
                GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<Driver>().IncreaseScore(scorePoints);
            }
            alreadyUsed = true;
        }
    }
}
