using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapManager : NetworkBehaviour {

    public GameObject oldOcean;
    public GameObject newOcean;
    public int oceanSize = 1000;
    public bool limit = false;

    void Start()
    {
        oldOcean = Instantiate(
            GameScene.Instance.ocean,
            new Vector3(oceanSize / 2, GameScene.Instance.ocean.transform.position.y, oceanSize / 2),
            Quaternion.identity
        );
        NetworkServer.SpawnWithClientAuthority(oldOcean, this.gameObject);
    }

	void Update () {
        float x = transform.position.x;
        float z = transform.position.z;

        if (transform.position.x < 0)
        {
            x -= oceanSize;
        }
        if (transform.position.z < 0)
        {
            z -= oceanSize;
        }

        Vector3 pos = new Vector3((int)(x / oceanSize), 0, (int)(z / oceanSize));
        Vector3 posFloat = new Vector3((x / oceanSize), 0, (z / oceanSize));

        //Debug.Log(pos);

        float xLimit = 0;
        if(posFloat.x > 0)
        {
            xLimit = posFloat.x - (int)posFloat.x;
        } else
        {
            xLimit = 1 + posFloat.x - (int)posFloat.x;
        }

        float zLimit = 0;
        if (posFloat.z > 0)
        {
            zLimit = posFloat.z - (int)posFloat.z;
        }
        else
        {
            zLimit = 1 + posFloat.z - (int)posFloat.z;
        }

        if (xLimit > 0.9)
        {
            Debug.Log("bottom");

            if (!limit)
            {
                newOcean = Instantiate(
                    GameScene.Instance.ocean,
                    new Vector3((pos.x + 1) * oceanSize + oceanSize / 2, GameScene.Instance.ocean.transform.position.y, pos.z * oceanSize + oceanSize / 2),
                    Quaternion.identity
                );
            }
            limit = true;
        }
        else if (xLimit < 0.1)
        {
            Debug.Log("top");

            if (!limit)
            {
                Debug.Log(new Vector3((pos.x - 1) * oceanSize + oceanSize / 2, GameScene.Instance.ocean.transform.position.y, pos.z * oceanSize + oceanSize / 2));
                newOcean = Instantiate(
                    GameScene.Instance.ocean,
                    new Vector3((pos.x - 1) * oceanSize + oceanSize / 2, GameScene.Instance.ocean.transform.position.y, pos.z * oceanSize + oceanSize / 2),
                    Quaternion.identity
                );
            }
            limit = true;
        }
        else if (zLimit > 0.9)
        {
            Debug.Log("right");

            if (!limit)
            {
                newOcean = Instantiate(
                    GameScene.Instance.ocean,
                    new Vector3(pos.x * oceanSize + oceanSize / 2, GameScene.Instance.ocean.transform.position.y, (pos.z + 1) * oceanSize + oceanSize / 2),
                    Quaternion.identity
                );
            }
            limit = true;
        }
        else if (zLimit < 0.1)
        {
            Debug.Log("left");

            if (!limit)
            {
                newOcean = Instantiate(
                    GameScene.Instance.ocean,
                    new Vector3(pos.x * oceanSize + oceanSize / 2, GameScene.Instance.ocean.transform.position.y, (pos.z - 1) * oceanSize + oceanSize / 2),
                    Quaternion.identity
                );
                NetworkServer.SpawnWithClientAuthority(newOcean, this.gameObject);
            }
            limit = true;
        }
        else
        {
            limit = false;
            if (oldOcean.transform.position.x != pos.x * oceanSize + oceanSize / 2 || oldOcean.transform.position.z != pos.z * oceanSize + oceanSize / 2)
            {
                /**Debug.Log("I need to spawn an ocean at " + pos.x * oceanSize + " " + pos.z * oceanSize);
                GameObject newOcean = Instantiate(
                    GameScene.Instance.ocean,
                    new Vector3(pos.x * oceanSize, GameScene.Instance.ocean.transform.position.y, pos.z * oceanSize),
                    Quaternion.identity
                );**/
                NetworkServer.Destroy(oldOcean);
                oldOcean = newOcean;
            }
        }
    }
}