using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchitectAI : MonoBehaviour {

    public bool on;
    public int placeDelay;
    public int difficulty; //random interval for placing (the higher the easier)
    public int lastPlaced = 0;
    System.Random Rnd = new System.Random();


    public ArchitectAI(bool on, int delay, int difficulty)
    {
        this.on = on;
        placeDelay = delay;
    }

    public string Position(ref float x, ref float z, int currentX, int currentZ, string lastDirection)
    {
        string direction = "forward";
        int blockSize = 16;

        switch (direction)
        {
            case "forward":
                x = currentX;
                z = currentZ + blockSize;
                break;
        }

        return direction;
    }
    
    public int Rotation(ref int rotation)
    {
        int mod = 0;

        rotation = (rotation + mod + 4) % 4;
        return 90 * mod;
    }

    public bool Place()
    {
        lastPlaced += 1;
        bool output = lastPlaced + Rnd.Next(0, difficulty) > placeDelay;

        if (output)
            lastPlaced = 0;

        return output;
    }
}
