using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    private static World instance;
    public static World Instance
    {
        get { return instance; }
    }

    public List<GameObject> tiles = new List<GameObject>();
    public Material tilePreviewMaterial;
    public int tileSize;

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

            int j = 0;
            while (j < newMaterials.Length)
            {
                if (oldMaterials[j] != material)
                {
                    newMaterials[j] = oldMaterials[j];
                    j++;
                }
            }

            mr[i].materials = newMaterials;
        }
    }
}
