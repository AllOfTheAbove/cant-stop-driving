using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Tile
{
    private GameScene world;

    public GameObject gameObject;

    public bool ready;

    public int x;
    public int y;
    public int z;
    public int rotation;
    public int direction;

    public bool init;

    public Tile(int x = 0, int y = 0, int z = 0)
    {
        this.world = GameScene.Instance;

        this.ready = false;
        this.x = x;
        this.y = y;
        this.z = z;
        this.direction = 0;
        this.init = false;
    }

    public void SetDirection(int direction)
    {
        Vector3 pos = this.gameObject.transform.position;

        if (direction == this.direction)
        {
            return;
        }

        if (direction == 0)
        {
            if (this.direction == 1)
            {
                pos.x = this.x + world.tileSize;
                pos.z = this.z + world.tileSize;
            }
            else if (this.direction == 2)
            {
                pos.x = this.x - world.tileSize;
                pos.z = this.z + world.tileSize;
            }
        }
        else if (direction == 1)
        {
            if (this.direction == 0)
            {
                pos.x = this.x - world.tileSize;
                pos.z = this.z - world.tileSize;
            }
            else if (this.direction == 2)
            {
                pos.x = this.x - 2 * world.tileSize;
                pos.z = this.z;
            }
        }
        else if (direction == 2)
        {
            if (this.direction == 0)
            {
                pos.x = this.x + world.tileSize;
                pos.z = this.z - world.tileSize;
            }
            else if (this.direction == 1)
            {
                pos.x = this.x + 2 * world.tileSize;
                pos.z = this.z;
            }
        }

        this.x = (int)pos.x;
        this.y = (int)pos.y;
        this.z = (int)pos.z;
        this.gameObject.transform.position = pos;
        this.direction = direction;
    }

    public void Rotate(int orientation)
    {
        this.rotation = (this.rotation + orientation) % 4;
        this.gameObject.transform.Rotate(0, orientation * 90, 0);
    }
}

public class Architect : Player
{

    public Camera architectCamera;
    public int distanceToPlaceTile;
    public int maxTilesPlaced;
    public AudioSource tileSpawnSound;
    public ParticleSystem tileSpawnAnimation;

    public Tile currentTile;
    public Tile lastTile;
    private ArchitectAI AI;
    private Vector3 architectDestination;

    private Queue<Tile> tilesPlaced = new Queue<Tile>();
    private int currentStack = 0;
    public int[] nextTileId = new int[2];
    private System.Random random = new System.Random();



    public override void OnStartLocalPlayer()
    {
        Game.Instance.ChangeState(-1);
        GetComponentInChildren<AudioListener>().enabled = true;
    }

    void Start()
    {
        nextTileId = new int[2];
        AI = this.gameObject.GetComponent<ArchitectAI>();
        architectDestination = new Vector3(0, 0, 0);
        currentTile = new Tile(0, 0, GameScene.Instance.tileSize);
        lastTile = new Tile(0, 0, 0);
        lastTile.init = true;
        if (isSingleplayer)
        {
            AI.enabled = true;
        }
        if (isLocalPlayer || isSingleplayer)
        {
            GetComponent<Goals>().SetGoal(lastTile);
            GetComponent<Pathfinding>().SetPath(lastTile);
        }
    }



    [Command]
    public void CmdSpawnTile()
    {
        currentTile.gameObject = Instantiate(
            GameScene.Instance.tiles[nextTileId[currentStack]].tile,
            new Vector3(currentTile.x, GameScene.Instance.tiles[nextTileId[currentStack]].tile.transform.position.y, currentTile.z),
            GameScene.Instance.tiles[nextTileId[currentStack]].tile.transform.rotation
        );
        currentTile.gameObject.name = GameScene.Instance.tiles[nextTileId[currentStack]].tile.name;

        if (AI.enabled)
        {
            currentTile.SetDirection(AI.Position(currentTile, lastTile));
            currentTile.gameObject.transform.Rotate(0, AI.Rotation(currentTile, lastTile), 0);
        }

        NetworkServer.SpawnWithClientAuthority(currentTile.gameObject, this.gameObject);

        // Delete too old tiles
        tilesPlaced.Enqueue(currentTile);
        if (tilesPlaced.Count > maxTilesPlaced)
        {
            Tile t = tilesPlaced.Dequeue();
            NetworkServer.Destroy(t.gameObject);
        }

        // Fix AI is not prepared for corners
        if (AI.enabled && GameScene.Instance.tiles[nextTileId[currentStack]].tile.name == "CornerTile")
        {
            nextTileId[currentStack] = 0;
        }

        RpcSetCurrentTileType(currentTile.gameObject, currentTile.x, currentTile.z, false, nextTileId[currentStack]);


    }

    [Command]
    private void CmdCreateTile()
    {
        nextTileId[currentStack] = GameScene.Instance.GetRandomTileId(nextTileId[currentStack]);

        // Make tile solid
        RpcSetCurrentTileType(currentTile.gameObject, currentTile.x, currentTile.z, true, nextTileId[currentStack]);

        // Prepare next tile
        if (GetComponent<Goals>().CheckGoal(currentTile))
        {
            lastTile = new Tile(Goals.goal.x, Goals.goal.y, Goals.goal.z);
            lastTile.gameObject = Instantiate(GameScene.Instance.tiles[3].tile, new Vector3(lastTile.x, lastTile.y - 60, lastTile.z), Quaternion.identity);
            lastTile.gameObject.transform.localScale = new Vector3(0, 0, 0);
            GetComponent<Goals>().SetGoal(lastTile);
            GetComponent<Pathfinding>().SetPath(lastTile);
        }
        else
        {
            lastTile = currentTile;
        }
        currentTile = new Tile(lastTile.x, 0, lastTile.z + GameScene.Instance.tileSize);
    }

    [ClientRpc]
    public void RpcSetCurrentTileType(GameObject tile, int x, int z, bool placed, int tileTypeId)
    {
        if (placed)
        {
            GameScene.Instance.Solid(tile, true);
            GameScene.Instance.RemoveMaterial(tile, GameScene.Instance.tilePreviewMaterial);
            tileSpawnSound.Play();
            StartCoroutine(architectCamera.GetComponent<CameraShake>().Shake());
            StartCoroutine(GameObject.FindGameObjectsWithTag("Driver")[0].GetComponent<Driver>().driverCamera.GetComponent<CameraShake>().Shake());
            ParticleSystem animation = Instantiate(tileSpawnAnimation);
            animation.transform.position = new Vector3(x, tileSpawnAnimation.transform.position.y, z);
            animation.transform.parent = tile.transform;

            architectDestination = new Vector3(x, 0, z);
        }
        else
        {
            tile.transform.parent = GameScene.Instance.tilesContainer.transform;
            GameScene.Instance.Solid(tile, false);
            GameScene.Instance.AddMaterial(tile, GameScene.Instance.tilePreviewMaterial);
        }
        GameScene.Instance.nextTileLabel.GetComponent<TextMeshProUGUI>().SetText(GameScene.Instance.tiles[tileTypeId].tile.name);
    }




    [Command]
    public void CmdSpawnBoat()
    {
        if (GameScene.Instance.currentNumberOfBoats < GameScene.Instance.maxNumberOfBoats)
        {
            GameObject boat = Instantiate(GameScene.Instance.boats[random.Next(0, 2)]);
            NetworkServer.SpawnWithClientAuthority(boat, this.gameObject);
            GameScene.Instance.currentNumberOfBoats++;
        }
    }


    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, architectDestination, 0.1f);

        if (Input.GetKeyDown(KeyCode.Escape) && isLocalPlayer)
        {
            Game.Instance.Pause();
        }

        if (!isLocalPlayer || Game.Instance.state != 1 || Game.Instance.paused)
        {
            return;
        }

        CmdSpawnBoat();

        if (!isSingleplayer)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                CmdSwitchStack();
            }
        }

        if (currentTile.ready)
        {
            updatePreviewTile();

            if ((!AI.enabled && Input.GetMouseButtonDown(0)) || (AI.enabled && AI.Place()))
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
                currentTile.ready = true;
                CmdSpawnTile();
            }
        }

    }

    [Command]
    public void CmdSwitchStack()
    {
        currentStack = (currentStack + 1) % 2;

        if (currentTile.ready)
        {
            NetworkServer.Destroy(currentTile.gameObject);
            currentTile.gameObject = Instantiate(
                GameScene.Instance.tiles[nextTileId[currentStack]].tile,
                new Vector3(currentTile.x, GameScene.Instance.tiles[nextTileId[currentStack]].tile.transform.position.y, currentTile.z),
                Quaternion.identity
            );
            currentTile.gameObject.name = GameScene.Instance.tiles[nextTileId[currentStack]].tile.name;
            NetworkServer.SpawnWithClientAuthority(currentTile.gameObject, this.gameObject);

            RpcSetCurrentTileType(currentTile.gameObject, currentTile.x, currentTile.z, false, nextTileId[currentStack]);
        }

        RpcSwitchStack(currentStack, nextTileId[currentStack]);
    }
    [ClientRpc]
    public void RpcSwitchStack(int stackId, int tileId)
    {
        GameScene.Instance.currentStackLabel.GetComponent<TextMeshProUGUI>().SetText(stackId + "");
        //GameScene.Instance.nextTileLabel.GetComponent<TextMeshProUGUI>().SetText(GameScene.Instance.tiles[tileId].tile.name);
    }


    void updatePreviewTile()
    {
        if (currentTile.gameObject == null || AI.enabled)
        {
            return;
        }

        Vector3 pos = currentTile.gameObject.transform.position;

        // Update direction
        Vector3 point = architectCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, architectCamera.transform.position.y));
        if (point.x < lastTile.x && lastTile.direction != 2)
        {
            currentTile.SetDirection(1);
        }
        if (point.x > lastTile.x && lastTile.direction != 1)
        {
            currentTile.SetDirection(2);
        }
        if (Math.Abs(point.z - lastTile.z) > Math.Abs(point.x - lastTile.x))
        {
            currentTile.SetDirection(0);
        }

        if (currentTile.x == Goals.goal.x && currentTile.z == Goals.goal.z)
        {
            switch (currentTile.direction)
            {
                case 0:
                    if (lastTile.direction != 2)
                        currentTile.SetDirection(1);
                    else
                        currentTile.SetDirection(2);
                    break;
                case 2:
                case 1:
                    currentTile.SetDirection(0);
                    break;
            }
        }

        // Update rotation
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentTile.Rotate(1);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentTile.Rotate(-1);
        }
    }

}