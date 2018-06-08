using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicles : MonoBehaviour {

    public static GameObject vehiclePreview;

    public void SelectVehicle(int id)
    {
        Game.Instance.currentVehicleId = id;
        Destroy(vehiclePreview);
        SpawnPreviewVehicle();
    }

    public static void SpawnPreviewVehicle()
    {
        vehiclePreview = Instantiate(Game.Instance.vehicles[Game.Instance.currentVehicleId], new Vector3(0, -1.7f, -1), Quaternion.identity);
        vehiclePreview.transform.Rotate(0, -180, 0);
        vehiclePreview.GetComponent<Vehicle>().enabled = false;
        vehiclePreview.AddComponent<Rigidbody>();
        vehiclePreview.GetComponent<Rigidbody>().mass = 2000;
    }
}
