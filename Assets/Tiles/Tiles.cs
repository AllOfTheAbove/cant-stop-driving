using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour {

    public static Material tilePreviewMaterial;
    public static AudioSource tileSpawnSound;
    public static int tileSize;

    public static void Solid(GameObject gameObject, bool solid)
    {
        MeshCollider[] bc = gameObject.GetComponentsInChildren<MeshCollider>();
        for (int i = 0; i < bc.Length; i++)
        {
            bc[i].enabled = solid;
        }
    }

    public static void AddMaterial(GameObject gameObject, Material material)
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

    public static void RemoveMaterial(GameObject gameObject, Material material)
    {
        MeshRenderer[] mr = gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < mr.Length; i++)
        {
            Material[] oldMaterials = mr[i].materials;
            Material[] newMaterials = new Material[oldMaterials.Length - 1];

            int j = 0;
            while (j < oldMaterials.Length)
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
