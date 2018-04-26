using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTitlescreenAnimation : MonoBehaviour {

    public int rotationSpeed = 30;

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
