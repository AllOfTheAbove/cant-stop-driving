using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {

    void OnCollisionEnter(Collision col)
    {
        if(Vector3.Distance(transform.position, GameObject.FindGameObjectsWithTag("Driver")[0].transform.position) < 30)
        {
            GameScene.Instance.collisionTileSound.Play();
        }
    }
}
