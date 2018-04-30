using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchitectAI : MonoBehaviour
{

    public bool enabled;
    public int placeDelay;
    public int difficulty; //random interval for placing (the higher the easier)
    public int lastPlaced = 0;

    private System.Random Random;
    public string[] linearTiles = { "BasicTile", "NarrowTile", "PropellerTile", "JumpTile", "RoughTile" };

    public Dictionary<string, int> tileCompensation = new Dictionary<string, int>();
    //some tiles meshes are not oriented in the same way by default, this dictionary offers a compensation (relative to BasicTile)

    void Start()
    {
        Random = new System.Random();

        tileCompensation.Add("BasicTile", 0);
        tileCompensation.Add("JumpTile", 90);
        tileCompensation.Add("NarrowTile", 90);
        tileCompensation.Add("PropellerTile", 0);
        tileCompensation.Add("RoughTile", 0);
    }

    public int Position(Tile currentTile, Tile lastTile)
    {
        if (Array.Exists(linearTiles, element => element == currentTile.gameObject.name))
        {
            return lastTile.direction;
        }

        //deal with non linear tiles here
        int direction = 0;
        int blockSize = 16;

        switch (direction)
        {
            case 0:
                break;
        }

        return direction;
    }

    public int Rotation(Tile currentTile, Tile lastTile)
    {
        int compensate = tileCompensation[currentTile.gameObject.name];
        if (Array.Exists(linearTiles, element => element == currentTile.gameObject.name)
            && (lastTile.init || Array.Exists(linearTiles, element => element == lastTile.gameObject.name)))
        {
            return 90 + compensate;
        }

        //deal with non linear previous tile here
        return 0;
    }

    public bool Place()
    {
        lastPlaced += 1;
        bool output = lastPlaced + Random.Next(0, difficulty) > placeDelay;

        if (output)
            lastPlaced = 0;

        return output;
    }
}