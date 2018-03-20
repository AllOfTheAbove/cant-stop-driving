using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchitectAI : MonoBehaviour {

    public bool enabled;
    public int placeDelay;
    public int difficulty; //random interval for placing (the higher the easier)
    public int lastPlaced = 0;

    private System.Random Random;

    void Start()
    {
        Random = new System.Random();
    }

    public int Position(ref float x, ref float z, int currentX, int currentZ, int lastDirection)
    {
        int direction = 0;
        int blockSize = 16;

        switch (direction)
        {
            case 0:
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
        bool output = lastPlaced + Random.Next(0, difficulty) > placeDelay;

        if (output)
            lastPlaced = 0;

        return output;
    }
}
