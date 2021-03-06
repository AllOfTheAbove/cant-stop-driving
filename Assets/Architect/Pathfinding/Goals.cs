﻿using System;
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

    public bool missedGoal = false;

    public Point StartPoint(Tile lastTile, Tile previousTile)
    {
        Point output = new Point(lastTile.x, lastTile.y, lastTile.z);


        if (!lastTile.init && !missedGoal && lastTile.gameObject.name == "CrossTile")
        {
            GoalTile pathStart = GetComponent<Pathfinding>().currentPath.Peek();
            return new Point(pathStart.x, pathStart.y, pathStart.z);
        }


        try
        {
            Quaternion next = new Quaternion(1, 0, 0, 0);
            Quaternion i = new Quaternion(0, 1, 0, 0);
            if (lastTile.gameObject.name == "CornerTile")
                next *= i * i;

            int rotation = (int)lastTile.gameObject.transform.rotation.eulerAngles.y + ArchitectAI.tileCompensation[lastTile.gameObject.name];

            while (rotation > 0)
            {
                rotation -= 90;
                next *= i;
            }

            output.x += (int)next.x * blockSize;
            output.z += (int)next.y * blockSize;

            if (Array.Exists(ArchitectAI.linearTiles, element => element == lastTile.gameObject.name) && previousTile != null && output.x == previousTile.x && output.y == previousTile.y)
            {
                output.x -= 2 * (int)next.x * blockSize;
                output.z -= 2 * (int)next.y * blockSize;
            }
        }
        catch (Exception) { }

        return output;
    }

    public void SetGoal(Tile lastTile)
    {
        if (goalExists)
            return;

        goalExists = true;

        int x = lastTile.x + blockSize * rng.Next(-5, 6);
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
        GetComponent<Pathfinding>().SetPath(currentTile);
        bool output = ((Math.Abs(currentTile.x - goal.x) == blockSize) && (Math.Abs(currentTile.z - goal.z) == 0))
                        || ((Math.Abs(currentTile.x - goal.x) == 0) && (Math.Abs(currentTile.z - goal.z) == blockSize));

        if (output && currentTile.gameObject.name != "CrossTile")
        {
            int delta = goal.x - currentTile.x;

            if (currentTile.gameObject.name == "CornerTile")
            {
                //Debug.Log(delta);
                //Debug.Log((int)currentTile.gameObject.transform.eulerAngles.y);
                switch ((int)currentTile.gameObject.transform.eulerAngles.y)
                {
                    case 180:
                        output = delta > 0;
                        break;
                    case 270:
                    case -90:
                        output = delta < 0;
                        break;
                    default:
                        output = delta == 0;
                        break;
                }
            }
            else
            {
                switch ((int)currentTile.gameObject.transform.eulerAngles.y - ArchitectAI.tileCompensation[currentTile.gameObject.name])
                {
                    case 180:
                        output = delta > 0;
                        break;
                    case 0:
                        output = delta < 0;
                        break;
                    default:
                        output = delta == 0;
                        break;
                }
            }
        }


        if (output) //increases score when goal reached
            Debug.Log("goal reached");
        //FIXME increase score

        if (!output && currentTile.z > goal.z) //decreases score when goal missed, but keeps on generating goal
        {
            Debug.Log("MISSED A GOAL !");
            missedGoal = true;
            Reset();
            SetGoal(currentTile);
            GetComponent<Pathfinding>().SetPath(currentTile);
            //FIXME decrease score
        }

        missedGoal = false;
        goalExists = !output;
        if (output)
            ArchitectAI.lastGoal = goal;
        return output;
    }

    public void Reset()
    {
        //Debug.Log("Reset");
        goalExists = false;
        goal = null;
        GetComponent<Pathfinding>().Reset();
    }

}
