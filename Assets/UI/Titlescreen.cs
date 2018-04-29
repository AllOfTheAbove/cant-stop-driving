using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Titlescreen : MonoBehaviour {

    private GameObject vehiclePreview;

    void OnGUI()
    {
        if (Game.Instance.debug)
        {
            if (GUILayout.Button("Host local game"))
            {
                Game.Instance.GetServer().StartHost();
            }
            if (GUILayout.Button("Join local game"))
            {
                Game.Instance.GetServer().StartClient();
            }
        }
    }

    void Start()
    {
        Game.Instance.SwitchAudio("title");
        Game.Instance.Reset();
        vehiclePreview = Instantiate(Game.Instance.vehicles[Game.Instance.currentVehicleId], new Vector3(0, 27, 0), Quaternion.identity);
        vehiclePreview.GetComponent<Vehicle>().enabled = false;
        vehiclePreview.AddComponent<Rigidbody>();
        vehiclePreview.GetComponent<Rigidbody>().mass = 1000;
    }

    public void Singleplayer()
    {
        Game.Instance.GetServer().singleplayer = true;
        Game.Instance.GetServer().StartHost();
    }

    public void About()
    {
        Application.OpenURL("http://cant-stop-driving.com/");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SwitchVehicle()
    {
        Game.Instance.currentVehicleId = (Game.Instance.currentVehicleId + 1) % Game.Instance.vehicles.Length;
        Destroy(vehiclePreview);
        vehiclePreview = Instantiate(Game.Instance.vehicles[Game.Instance.currentVehicleId], new Vector3(0, 26, 0), Quaternion.identity);
        vehiclePreview.GetComponent<Vehicle>().enabled = false;
        vehiclePreview.AddComponent<Rigidbody>();
        vehiclePreview.GetComponent<Rigidbody>().mass = 1000;
    }

    private void Update()
    {
        vehiclePreview.transform.Rotate(0, 30 * Time.deltaTime, 0);
    }

}
