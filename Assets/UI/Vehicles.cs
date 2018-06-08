using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Vehicles : MonoBehaviour {

    public static GameObject vehiclePreview;
    public TextMeshProUGUI score;
    public GameObject truck;
    public GameObject van;
    public int scoreForTruck;
    public int scoreForVan;

    public void Start()
    {
        int highscore = PlayerPrefs.GetInt("scoreboard_highscore");
        score.SetText("Your score: " + highscore);
        if(highscore <= scoreForTruck)
        {
            truck.GetComponent<Image>().enabled = false;
            truck.GetComponent<Button>().enabled = false;
            truck.GetComponent<ChangeTextColorOnHoverTransition>().enabled = false;
        }
        if (highscore <= scoreForVan)
        {
            van.GetComponent<Image>().enabled = false;
            van.GetComponent<Button>().enabled = false;
            van.GetComponent<ChangeTextColorOnHoverTransition>().enabled = false;
        }
    }

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
