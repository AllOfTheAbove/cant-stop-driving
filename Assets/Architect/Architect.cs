using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tile
{
    public GameObject gameObject;
    public int x;
    public int y;
    public int z;
    public int rotation;
    public int direction;

    public Tile(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

public class Architect : Player
{

    public Camera architectCamera;
    public List<GameObject> tiles = new List<GameObject>();
    public int distanceToPlaceTile;

    private Tile currentTile;
    private Tile lastTile;
    private ArchitectAI AI;
    private Vector3 architectDestination;

    private System.Random random = new System.Random();



    void Start()
    {
        currentTile = new Tile(0, 0, 0);
        AI = this.gameObject.GetComponent<ArchitectAI>();
        architectDestination = new Vector3(0, 0, 0);

        if (isSingleplayer)
        {
            AI.enabled = true;
        }
    }



    [Command] public void CmdSpawnTile()
    {
        currentTile = new Tile(lastTile.x, 0, lastTile.z);
        if (currentTile.direction == 0)
        {
            currentTile.z = lastTile.z + Tiles.tileSize;
        }
        else if (currentTile.direction == 1)
        {
            currentTile.x = lastTile.x - Tiles.tileSize;
        }
        else if (currentTile.direction == 2)
        {
            currentTile.x = lastTile.x + Tiles.tileSize;
        }

        int randomTileId = random.Next(0, tiles.Count);
        currentTile.gameObject = Instantiate(
            tiles[randomTileId],
            new Vector3(currentTile.x, tiles[randomTileId].transform.position.y, currentTile.z),
            Quaternion.identity
        );
        currentTile.gameObject.name = tiles[randomTileId].name;
        NetworkServer.SpawnWithClientAuthority(currentTile.gameObject, this.gameObject);

        RpcSetCurrentTileType(currentTile.gameObject, currentTile.x, currentTile.z, 0);
    }

    [Command] private void CmdCreateTile()
    {
        // Increase score and make tile solid
        RpcSetCurrentTileType(currentTile.gameObject, currentTile.x, currentTile.z, 1);
        CmdAddScore();

        // Prepare next tile
        lastTile = currentTile;
        currentTile = null;
    }

    [ClientRpc] public void RpcSetCurrentTileType(GameObject tile, int x, int z, int type)
    {
        if (type == 0)
        {
            Tiles.Solid(tile, false);
            Tiles.AddMaterial(tile, Tiles.tilePreviewMaterial);
        }
        else if(type == 1)
        {
            Tiles.Solid(tile, true);
            Tiles.RemoveMaterial(tile, Tiles.tilePreviewMaterial);
            Tiles.tileSpawnSound.Play();
            architectDestination = new Vector3(x, 0, z);
        }
    }



    void Update()
    {
        base.Update();

        transform.position = Vector3.Lerp(transform.position, architectDestination, 0.1f);

        if (!isLocalPlayer || !GameObject.Find("Driver") || gamePaused)
        {
            return;
        }

        if (currentTile != null)
        {
            updatePreview();

            if (Input.GetMouseButtonDown(0) || (AI.enabled && AI.Place()))
            {
                CmdCreateTile();
            }
        }
        else
        {
            Vector3 carPosition = GameObject.Find("Driver").transform.position;
            float distance = Vector3.Distance(
                new Vector3(lastTile.x, 0, lastTile.z),
                new Vector3(carPosition.x, 0, carPosition.z)
            );

            if (distance < distanceToPlaceTile)
            {
                CmdSpawnTile();
            }
        }
        
    }



    void updatePreview()
    {
        Vector3 pos = currentTile.gameObject.transform.position;

        /**if (AI.enabled)
        {
            direction = AI.Position(ref pos.x, ref pos.z, currentTile.x, currentTile.z, lastDirection);
            currentTile.gameObject.transform.position = pos;
            currentTile.gameObject.transform.Rotate(0, AI.Rotation(ref currentTile.rotation), 0);

            return;
        }**/

        // Update position
        Vector3 point = architectCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, architectCamera.transform.position.y));
        if (point.x < currentTile.x && lastTile.direction != 2)
        {
            pos.x = currentTile.x - Tiles.tileSize;
            pos.z = currentTile.z;
            currentTile.direction = 1;
        }
        if (point.x > currentTile.x && lastTile.direction != 1)
        {
            pos.x = currentTile.x + Tiles.tileSize;
            pos.z = currentTile.z;
            currentTile.direction = 2;
        }
        if (Math.Abs(point.z - currentTile.z) > Math.Abs(point.x - currentTile.x))
        {
            pos.x = currentTile.x;
            pos.z = currentTile.z + Tiles.tileSize;
            currentTile.direction = 0;
        }
        currentTile.gameObject.transform.position = pos;

        // Update rotation
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentTile.rotation = (currentTile.rotation + 1) % 4;
            currentTile.gameObject.transform.Rotate(0, 90, 0);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentTile.rotation = (currentTile.rotation - 1) % 4;
            currentTile.gameObject.transform.Rotate(0, -90, 0);
        }
    }

}
