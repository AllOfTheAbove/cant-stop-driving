using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoalTile
{
    private GameScene world;

    public GameObject gameObject;

    public bool ready;

    public int x;
    public int y;
    public int z;

    public GoalTile(int x, int y, int z)
    {
        this.world = GameScene.Instance;

        this.ready = false;
        this.x = x;
        this.y = y;
        this.z = z;
    }

}

public class Goals : NetworkBehaviour
{
    static System.Random rng = new System.Random();
    static public GoalTile goal;
    static public bool goalExists = false;
    static int blockSize = 16;
    static string[] linearTiles = { "BasicTile", "NarrowTile", "PropellerTile", "JumpTile", "RoughTile" };


    public Point StartPoint(Tile lastTile, Tile previousTile)
    {
        Point output = new Point(lastTile.x, lastTile.y, lastTile.z);

        try
        {
            //throw new NotImplementedException();

            Quaternion next = new Quaternion(1, 0, 0, 0);
            Quaternion i = new Quaternion(0, 1, 0, 0);
            if (lastTile.gameObject.name == "CornerTile")
                next *= i;

            int rotation = (int)lastTile.gameObject.transform.rotation.eulerAngles.y + ArchitectAI.tileCompensation[lastTile.gameObject.name];

            while (rotation > 0)
            {
                rotation -= 90;
                next *= i;
            }
            //Debug.Log("Pre modif" + output);

            output.x += (int)next.x * blockSize;
            output.z += (int)next.y * blockSize;

            if (Array.Exists(linearTiles, element => element == lastTile.gameObject.name) && previousTile != null && output.x == previousTile.x && output.y == previousTile.y)
            {
                output.x -= 2 * (int)next.x * blockSize;
                output.z -= 2 * (int)next.y * blockSize;
            }

            //Debug.Log(next);
            //Debug.Log("Post modif" + output);
        }
        catch (Exception){}
        
        return output;
    }
    
    public void SetGoal(Tile lastTile)
    {
        if (goalExists)
            return;

        goalExists = true;

        int x = lastTile.x /*+ blockSize * rng.Next(-5, 6)*/;
        int y = lastTile.y;
        int z = lastTile.z + blockSize * rng.Next(2, 10);

        //calculate time for chronometer

        goal = new GoalTile(x, y, z);
        goal.gameObject = Create(x, y, z, true, GameScene.Instance.Goal);
    }

    public GameObject Create(int x, int y, int z, bool solid, GameObject type)
    {
        GameObject o = Instantiate(type,
            new Vector3(x, type.transform.position.y, z),
            Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(o, this.gameObject);
        //RpcCreate(goal.gameObject, solid);
        return o;
    }

    /**[ClientRpc] public void RpcCreate(GameObject go, bool solid)
    {
        go.transform.parent = GameScene.Instance.pathfindingContainer.transform;
        GameScene.Instance.Solid(go, solid);
    }**/

    public bool CheckGoal(Tile currentTile)
    {
        //Point start = StartPoint(currentTile, previousTile);
        GetComponent<Pathfinding>().SetPath(currentTile);
        bool output = ((Math.Abs(currentTile.x - goal.x) == blockSize) && (Math.Abs(currentTile.z - goal.z) == 0)) 
                        || ((Math.Abs(currentTile.x - goal.x) == 0) && (Math.Abs(currentTile.z - goal.z) == blockSize));
        goalExists = !output;
        ArchitectAI.goalReached = output;
        return output;
    }

    public void Reset()
    {
        goalExists = false;
        GetComponent<Pathfinding>().Reset();
    }
}
