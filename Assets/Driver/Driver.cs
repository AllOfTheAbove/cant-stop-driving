using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Driver : Player
{
    public GameObject car;
    public GameObject cameraContainer;
    private float currentSpeedModifier = 0;

    public int defaultSpeed = 10;
    public float maxSpeedModifier = 1.2f;

    public float rotationDamping = 3f;
    public float distance = 10f;

    public override void OnStartLocalPlayer()
    {
        CmdReady();
    }

    void Start()
    {
        Ready();
        cameraContainer.transform.parent = null;
    }

    private void LateUpdate()
    {
        float wantedRotationAngle = car.transform.eulerAngles.y;
        float currentRotationAngle = cameraContainer.transform.localEulerAngles.y;
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        cameraContainer.transform.position = car.transform.position;
        cameraContainer.transform.position -= currentRotation * Vector3.forward * distance;
        cameraContainer.transform.position = new Vector3(cameraContainer.transform.position.x, 10f, cameraContainer.transform.position.z);
        cameraContainer.transform.LookAt(car.transform);
    }

    void Update()
    {
        if (!isSingleplayer && !isLocalPlayer)
        {
            car.GetComponent<CarController>().enabled = false;
            return;
        }
        car.GetComponent<CarController>().enabled = true;

        if (Input.GetKeyDown(KeyCode.Escape) && !isSingleplayer)
        {
            Game.Instance.Pause();
        }

        //currentSpeedModifier = Input.GetAxis("Vertical") * maxSpeedModifier;
        //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, defaultSpeed + currentSpeedModifier);

        if (Input.GetKey(KeyCode.R) || transform.position.y < -8) // || (rb.velocity.z / defaultSpeed) < 0.85
        {
            if (!isSingleplayer)
            {
                CmdDeath();
            } else
            {
                NetworkManager.singleton.ServerChangeScene("game");
            }
        }
    }

    [Command] public void CmdDeath()
    {
        NetworkManager.singleton.ServerChangeScene("game");
    }

}
