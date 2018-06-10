using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproduceMovements : MonoBehaviour {

    public int currentPosition;
    public List<Vector3> positions;
    public List<Quaternion> rotations;
    public int vehicleId;
    public bool fallInWater = false;

    private bool exploding = false;

    void FixedUpdate ()
    {
        if (currentPosition >= positions.Count)
        {
            if(!exploding)
            {
                if(!fallInWater)
                {
                    GetComponent<Vehicle>().Explode();
                }
                exploding = true;
                StartCoroutine(Reset());
            }
        } else
        {
            transform.position = positions[currentPosition];
            transform.rotation = rotations[currentPosition];
            currentPosition++;
        }
    }

    public IEnumerator Reset()
    {
        yield return new WaitForSecondsRealtime(4);
        currentPosition = 0;
        GameObject vehicle = Instantiate(Game.Instance.vehicles[vehicleId], positions[0], rotations[0]);
        vehicle.GetComponent<Vehicle>().rewind = true;
        vehicle.AddComponent<ReproduceMovements>();
        vehicle.GetComponent<ReproduceMovements>().positions = positions;
        vehicle.GetComponent<ReproduceMovements>().rotations = rotations;
        vehicle.GetComponent<ReproduceMovements>().vehicleId = vehicleId;
        vehicle.GetComponent<ReproduceMovements>().fallInWater = fallInWater;
        GameScene.Instance.camera.GetComponent<DeathCamera>().target = vehicle;
        Destroy(this.gameObject);
        exploding = false;
    }
}
