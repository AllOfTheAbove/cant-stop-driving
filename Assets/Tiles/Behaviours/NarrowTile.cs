using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrowTile : MonoBehaviour {

    public int speed = 2;

    private int direction = -1;

	void Update () {
        if(transform.position.x < -4)
        {
            direction = -1;
        }
        if (transform.position.x > 4)
        {
            direction = 1;
        }
        transform.Translate(direction * speed * Time.deltaTime, 0, 0);
    }

}
