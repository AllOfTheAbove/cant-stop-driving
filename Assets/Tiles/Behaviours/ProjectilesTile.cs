using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProjectilesTile : MonoBehaviour {

    public GameObject projectile;
    public int delay = 4;

    private bool started = false;
    private System.Random random = new System.Random();

    private void Update()
    {
        if(!started && GetComponentsInChildren<MeshCollider>()[0].enabled)
        {
            started = true;
            StartCoroutine(SpawnProjectile());
        }
    }

    IEnumerator SpawnProjectile()
    {
        while (true)
        {
            GameObject _projectile = Instantiate(projectile, new Vector3(transform.position.x + random.Next(-6, 6), 10, transform.position.z + random.Next(-6, 6)), Quaternion.identity);
            NetworkServer.SpawnWithClientAuthority(_projectile, GameObject.FindGameObjectsWithTag("Architect")[0]);
            yield return new WaitForSecondsRealtime(delay);
        }
    }

}
