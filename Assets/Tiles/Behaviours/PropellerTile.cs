using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerTile : MonoBehaviour {

    public int spinSpeed = 30;

    void Update()
    {
        var pales = transform.Find("Pales");
        pales.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }
}
