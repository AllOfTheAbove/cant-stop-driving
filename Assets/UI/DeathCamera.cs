using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCamera : MonoBehaviour {

    public GameObject target;

    private Vector3 distance;

    private void Start()
    {
        distance = transform.position;
    }

    private void Update()
    {
        transform.position = new Vector3(target.transform.position.x + distance.x, target.transform.position.y + distance.y, target.transform.position.z + distance.z);
    }

}
