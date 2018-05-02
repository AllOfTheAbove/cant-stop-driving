using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproduceMovements : MonoBehaviour {

    public int currentPosition;
    public List<Vector3> positions;
    public List<Quaternion> rotations;

    void FixedUpdate ()
    {
        transform.position = positions[currentPosition];
        transform.rotation = rotations[currentPosition];
        currentPosition++;
        if (currentPosition >= positions.Count)
        {
            currentPosition = 0;
        }
    }
}
