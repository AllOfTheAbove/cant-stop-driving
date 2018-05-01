using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverCamera : MonoBehaviour {

    public GameObject vehicle;
    public float rotationDamping = 3f;
    public float distance = 10f;
    public float heightDamping = 2f;
    public float height = 100f;

    private bool endOfStartAnimation = false;

    public void EndOfStartAnimation()
    {
        endOfStartAnimation = true;
        GetComponent<Animator>().enabled = false;
        transform.parent = null;
    }

    private void LateUpdate()
    {
        if (endOfStartAnimation)
        {
            if(vehicle == null)
            {
                return;
            }

            float wantedRotationAngle = vehicle.transform.eulerAngles.y;
            float wantedHeight = vehicle.transform.position.y + height;

            float currentRotationAngle = transform.localEulerAngles.y;
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
            var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            float currentHeight = transform.position.y;
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
            transform.position = vehicle.transform.position;
            transform.position -= currentRotation * Vector3.forward * distance;
            transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
            transform.LookAt(vehicle.transform);
        }
    }
}
