using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTile : MonoBehaviour {

    private bool alreadyUsed = false;

	void OnTriggerEnter(Collider collider)
    {
        if (!alreadyUsed)
        {
            GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<Vehicle>().Boost();
            alreadyUsed = true;
        }
    }

}
