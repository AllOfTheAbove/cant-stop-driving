using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerAnimation : MonoBehaviour {

    public int spinSpeed = 30;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var pales = transform.Find("Pales");
        pales.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }
}
