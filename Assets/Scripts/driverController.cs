using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class driverController : MonoBehaviour
{

    private Rigidbody rb;
    private float currentSpeedModifier = 0;

    public int defaultSpeed = 10;
    public float maxSpeedModifier = 1.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Detect and manage driver death
        if(transform.position.y < -10)
        {
            Debug.Log("death");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Can't stop driving ;-)
        currentSpeedModifier = Input.GetAxis("Vertical") * maxSpeedModifier;
        //rb.velocity = new Vector3(Input.GetAxis("Horizontal"), rb.velocity.y, defaultSpeed + currentSpeedModifier);
    }

}
