using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTile : MonoBehaviour {

    private bool alreadyUsed = false;

	public void Boost()
    {
        if (!alreadyUsed && GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<Vehicle>() != null)
        {
            GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<Vehicle>().Boost();
            alreadyUsed = true;
        }
    }

}
