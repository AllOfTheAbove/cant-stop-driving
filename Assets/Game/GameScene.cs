using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileAndFrequency
{
    public GameObject tile;
    [Range(0, 10)] public int frequency;
}

public class GameScene : MonoBehaviour {

    private System.Random random = new System.Random();

    private static GameScene instance;
    public static GameScene Instance
    {
        get { return instance; }
    }

    public GameObject camera;
    public bool night = false;
    public Light light;
    public Light nightLight;

    [Header("World")]
    public int maxNumberOfBoats = 3;
    public int currentNumberOfBoats = 0;
    public GameObject[] boats;

    [Header("Tiles")]
    public GameObject tilesContainer;
    public List<TileAndFrequency> tiles = new List<TileAndFrequency>();
    public Material tilePreviewMaterial;
    public int tileSize;
    public Material tileWoodMaterial;
    private List<int> pickTileId = new List<int>();

    [Header("Pathfinding")]
    public GameObject pathfindingContainer;
    public GameObject Goal;
    public GameObject Path;

    [Header("UIs")]
    public GameObject driverTutorial;
    public GameObject architectTutorial;
    public GameObject speed;
    public GameObject speedLabel;
    public GameObject nextTile;
    public GameObject nextTileLabel;
    public GameObject currentStack;
    public GameObject currentStackLabel;
    public GameObject warningLabel;
    public GameObject waitingUI;
    public GameObject pauseUI;
    public GameObject scoreLabel;
    public GameObject score;
    public GameObject countdownLabel;
    public GameObject gameoverUI;
    public GameObject gameoverScoreLabel;
    public GameObject gameoverTimeLabel;
    public GameObject gameoverHighscore;

    [Header("SFXs")]
    public AudioSource countdownBeepSound;
    public AudioSource countdownEndSound;
    public AudioSource dangerSound;
    public AudioSource explosionSound;
    public AudioSource fallInWaterSound;
    public AudioSource collisionTileSound;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            for (int j = 0; j < tiles[i].frequency; j++)
            {
                pickTileId.Add(i);
            }
        }

        StartCoroutine(DayAndNight());
    }

    public IEnumerator DayAndNight()
    {
        yield return new WaitForSecondsRealtime(random.Next(3, 7));
        GameObject.FindGameObjectsWithTag("Architect")[0].GetComponent<Architect>().CmdToggleNight(!night);
        StartCoroutine(DayAndNight());
    }

    public int GetRandomTileId(int oldId)
    {
        int newId = pickTileId[random.Next(0, pickTileId.Count)];
        while(newId == oldId)
        {
            newId = pickTileId[random.Next(0, pickTileId.Count)];
        }
        return newId;
    }

    public void ToggleNight(bool state)
    {
        night = state;
        if(night)
        {
            light.enabled = false;
            nightLight.enabled = true;
        } else
        {
            light.enabled = true;
            nightLight.enabled = false;
        }
        
        //GameObject.FindGameObjectsWithTag("Architect")[0].GetComponentInChildren<Camera>().backgroundColor = new Color(197, 220, 255);
        //GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<Camera>().backgroundColor = new Color(255, 255, 255);
    }

    public void SetNight()
    {
        //GameObject.FindGameObjectsWithTag("Architect")[0].GetComponentInChildren<Camera>().backgroundColor = new Color(53, 82, 126);
        //GameObject.Find("DriverCamera").GetComponentInChildren<Camera>().backgroundColor = new Color(34, 34, 34);
    }

    public void Solid(GameObject gameObject, bool solid)
    {
        MeshCollider[] bc = gameObject.GetComponentsInChildren<MeshCollider>();
        for (int i = 0; i < bc.Length; i++)
        {
            bc[i].enabled = solid;
        }
    }

    public void AddMaterial(GameObject gameObject, Material material)
    {
        MeshRenderer[] mr = gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < mr.Length; i++)
        {
            mr[i].materials = new Material[] { mr[i].materials[0], tilePreviewMaterial };
        }

        /**for (int i = 0; i < mr.Length; i++)
        {
            Material[] oldMaterials = mr[i].materials;
            Material[] newMaterials = new Material[oldMaterials.Length + 1];
            for (int j = 0; j < oldMaterials.Length; j++)
            {
                newMaterials[j] = oldMaterials[j];
            }
            newMaterials[oldMaterials.Length] = material;
            mr[i].materials = newMaterials;
        }**/
    }

    public void RemoveMaterial(GameObject gameObject, Material material)
    {
        MeshRenderer[] mr = gameObject.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < mr.Length; i++)
        {
            if(mr[i].materials[0].name.Contains("No Name"))
            {
                mr[i].materials = new Material[] { tileWoodMaterial };
            } else
            {
                mr[i].materials = new Material[] { mr[i].materials[0] };
            }
            
            /**Material[] oldMaterials = mr[i].materials;
            Material[] newMaterials = new Material[oldMaterials.Length - 1];

            int newMaterialId = 0;
            int oldMaterialId = 0;
            while (newMaterialId < newMaterials.Length)
            {
                if (oldMaterials[oldMaterialId] != material)
                {
                    newMaterials[newMaterialId] = oldMaterials[oldMaterialId];
                    newMaterialId++;
                }
                oldMaterialId++;
            }

            if (oldMaterials.Length == 1 && oldMaterials[0] != material)
            {
                mr[i].materials = oldMaterials;
            }
            else
            {
                mr[i].materials = newMaterials;
            }**/
        }
    }
}
