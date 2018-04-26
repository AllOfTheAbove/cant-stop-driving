using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour {

    private static GameScene instance;
    public static GameScene Instance
    {
        get { return instance; }
    }

    [Header("Tiles")]
    public List<GameObject> tiles = new List<GameObject>();
    public Material tilePreviewMaterial;
    public int tileSize;

    [Header("UIs")]
    public GameObject waitingUI;
    public GameObject pauseUI;
    public GameObject scoreLabel;
    public GameObject countdownLabel;

    [Header("SFXs")]
    public AudioSource countdownBeepSound;
    public AudioSource countdownEndSound;

    private void Awake()
    {
        instance = this;
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
            Material[] oldMaterials = mr[i].materials;
            Material[] newMaterials = new Material[oldMaterials.Length + 1];
            for (int j = 0; j < oldMaterials.Length; j++)
            {
                newMaterials[j] = oldMaterials[j];
            }
            newMaterials[oldMaterials.Length] = material;
            mr[i].materials = newMaterials;
        }
    }

    public void RemoveMaterial(GameObject gameObject, Material material)
    {
        MeshRenderer[] mr = gameObject.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < mr.Length; i++)
        {
            Material[] oldMaterials = mr[i].materials;
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
            }
        }
    }
}
