using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ArchitectAI : MonoBehaviour
{

    public bool enabled;
    public int placeDelay = 0;
    public int difficulty = 10; //random interval for placing (the higher the easier)
    public int lastPlaced = 0;

    private Tile lastTile = null;

    static public GoalTile lastGoal = null;

    private System.Random Random;
    public static string[] linearTiles = { "BasicTile", "BoostTile", "CornerTile", "CrossTile", "JumpTile", "NarrowTile", "PropellerTile",
                                    "RoofTile", "RoughTile", "SlowTile", "ProjectilesTile", "SplashTile" };

    public static Dictionary<string, int> tileCompensation = new Dictionary<string, int>();
    //some tiles meshes are not oriented in the same way by default, this dictionary offers a compensation (relative to BasicTile)

    void Start()
    {
        Random = new System.Random();

        tileCompensation.Add("BasicTile", 0);
        tileCompensation.Add("BoostTile", 0);
        tileCompensation.Add("CornerTile", 0);
        tileCompensation.Add("CrossTile", 0);
        tileCompensation.Add("JumpTile", 90);
        tileCompensation.Add("NarrowTile", 90);
        tileCompensation.Add("PropellerTile", 0);
        tileCompensation.Add("RoofTile", 0);
        tileCompensation.Add("RoughTile", 0);
        tileCompensation.Add("SlowTile", 0);
        tileCompensation.Add("ProjectilesTile", 0);
        tileCompensation.Add("SplashTile", 0);
    }

    public int Position(Tile currentTile, Tile lastTile)
    {
        GoalTile position = GetComponent<Pathfinding>().currentPath.Peek();

        GoalTile nextPath;
        try
        {
            nextPath = GetComponent<Pathfinding>().currentPath.Skip(1).First();
        }
        catch (Exception e)
        {
            Debug.Log("path too short");
            nextPath = new GoalTile(Goals.goal.x, Goals.goal.y, Goals.goal.z);
        }


        //unset crosstiles to avoid weird bugs
        if (currentTile.gameObject.name == "CrossTile")
        {
            NetworkServer.Destroy(currentTile.gameObject);
            currentTile.gameObject = Instantiate(GameScene.Instance.tiles[0].tile,
                new Vector3(currentTile.x, GameScene.Instance.tiles[0].tile.transform.position.y, currentTile.z), Quaternion.identity);
            currentTile.gameObject.name = GameScene.Instance.tiles[0].tile.name;
        }

        //set corner when needed
        //yes, the ai is basically cheating (but for the greater good)
        if ((nextPath.x - lastTile.x == 16 || nextPath.x - lastTile.x == -16) && (nextPath.z - lastTile.z == 16 || nextPath.z - lastTile.z == -16))
        {
            NetworkServer.Destroy(currentTile.gameObject);
            currentTile.gameObject = Instantiate(GameScene.Instance.tiles[2].tile,
                new Vector3(currentTile.x, GameScene.Instance.tiles[2].tile.transform.position.y, currentTile.z), Quaternion.identity);
            currentTile.gameObject.name = GameScene.Instance.tiles[2].tile.name;
        }
        else if (currentTile.gameObject.name == "CornerTile")
        {
            NetworkServer.Destroy(currentTile.gameObject);
            currentTile.gameObject = Instantiate(GameScene.Instance.tiles[0].tile,
                new Vector3(currentTile.x, GameScene.Instance.tiles[0].tile.transform.position.y, currentTile.z), Quaternion.identity);
            currentTile.gameObject.name = GameScene.Instance.tiles[0].tile.name;
        }



        int direction = 0;

        Vector3 delta = new Vector3(position.x - lastTile.x, 0, position.z - lastTile.z);

        if (delta.x > 0)
            direction = 2;
        if (delta.x < 0)
            direction = 1;


        return direction;
    }

    public int Rotation(Tile currentTile, Tile lastTile)
    {

        int lastCompensate = -90;
        try
        {
            lastCompensate = tileCompensation[lastTile.gameObject.name];
            if (lastTile.gameObject.name == "CornerTile")
            {
                switch (((int)lastTile.gameObject.transform.rotation.eulerAngles.y) + 360 % 360)
                {
                    case 0:
                    case 90:
                        lastCompensate = 270;
                        break;
                    case 270:
                    case -90:
                        lastCompensate = 270;
                        break;
                    default:
                        lastCompensate = 0;
                        break;
                }
            }
        }
        catch (Exception e) { };

        int compensate = tileCompensation[currentTile.gameObject.name] - lastCompensate;

        int delta = 0;
        if (currentTile.gameObject.name == "CornerTile")
        {
            GoalTile goal = Goals.goal;
            delta = goal.x - currentTile.x;
            if (delta == 0)
                delta = lastTile.x - currentTile.x > 0 ? 90 : 180;//z increases
            else
                delta = (delta > 0 ? 90 : 180); //x modifies
        }



        try
        {
            if (lastTile != null && lastTile.gameObject.name == "JumpTile" && currentTile.gameObject.name == "JumpTile")
                compensate += 180;

            if (lastTile != null && lastTile.gameObject.name == "CrossTile" && (lastTile.x != currentTile.x || lastTile.z != currentTile.z))
            {
                if (lastTile.x - currentTile.x == 0)
                    return 90 + compensate;
                else if (lastTile.x - currentTile.x > 0)
                    return 180 + compensate;
                return 0 + compensate;
            }

            //Debug.Log(currentTile.gameObject.name + ": " + new Vector3((int)lastTile.gameObject.transform.rotation.eulerAngles.y, compensate, delta));
            //Debug.Log(lastTile.gameObject.name);
            return (int)lastTile.gameObject.transform.rotation.eulerAngles.y + compensate + delta;
        }
        catch (Exception e) { Debug.Log(e); }
        return compensate + delta;
    }

    public bool Place()
    {
        if (lastGoal != null)
            lastGoal = null;

        lastPlaced += 1;
        bool output = Random.Next(0, 5) > 3;

        if (output)
            lastPlaced = 1;

        //output = true;
        return output;
    }
}