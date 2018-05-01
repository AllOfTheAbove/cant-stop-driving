using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrowTile : MonoBehaviour {

    public GameObject container;
    public int speed = 2;

    private int direction = -1;

    void Update () {
        if(GetComponentsInChildren<MeshCollider>()[0].enabled)
        {
            if (transform.eulerAngles.y == 90 || transform.eulerAngles.y == 270) // horizontal
            {
                if (container.transform.localPosition.x < -4)
                {
                    direction = 1;
                }
                if (container.transform.localPosition.x > 4)
                {
                    direction = -1;
                }
                container.transform.Translate(new Vector3(direction * speed * Time.deltaTime, 0, 0));
            }
            else // vertical
            {
                if (container.transform.localPosition.x < -4)
                {
                    direction = 1;
                }
                if (container.transform.localPosition.x > 4)
                {
                    direction = -1;
                }
                container.transform.Translate(new Vector3(direction * speed * Time.deltaTime, 0, 0));
            }
        }
    }

}
