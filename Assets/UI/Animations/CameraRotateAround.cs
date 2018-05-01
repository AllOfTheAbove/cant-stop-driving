using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateAround : MonoBehaviour {

    public int speed = 5;
    public Vector3 target;

    void Update()
    {
        transform.Translate(speed * Vector3.right * Time.deltaTime);
        transform.LookAt(target);
    }
}
