using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pathfinding : NetworkBehaviour {

    private static Stack<Tile> placedTiles = new Stack<Tile>();
    private static Stack<GoalTile> currentPath = new Stack<GoalTile>();

    private static int blockSize = 16;


    private static void EmptyPath(ref Stack<GoalTile> path)
    {
        foreach (GoalTile tile in path)
            Destroy(tile.gameObject);
        path = new Stack<GoalTile>();
    }

    private static int[,,] MapToArray(Stack<Tile> placedTiles, Point start)
    {
        int size = 2 * Math.Max(Math.Abs(Goals.goal.x - start.x) / blockSize, Math.Abs(Goals.goal.z - start.z) / blockSize) + 3;

        int[,,] array = new int[size, size, size];
        int center = array.GetLength(0) / 2;

        array[center, center, center] = -1; //initialize start position

        foreach (Tile tile in placedTiles)
        {
            try
            {
                array[(tile.x - start.x) / blockSize, (tile.y - start.y) / blockSize, (tile.z - start.z) / blockSize] = -10;
            }
            catch (Exception) {}
        }

        array = SetGoal(array, start);
        return array;
    }

    private static int[,,] SetGoal(int[,,] array, Point start)
    {
        int center = array.GetLength(0) / 2;
        int x = (Goals.goal.x - start.x) / blockSize + center;
        int y = (Goals.goal.y - start.y) / blockSize + center;
        int z = (Goals.goal.z - start.z) / blockSize + center;
        array[x, y, z] = -2;
        return array;
    }

    public void SetPath(Tile lastTile)
    {
        if (currentPath.Count > 0)
        {
            GoalTile pathStart = currentPath.Pop();
            Destroy(pathStart.gameObject);

            /*if (pathStart.x == lastTile.x && pathStart.y == lastTile.y && pathStart.z == lastTile.z)
            {
                placedTiles.Push(lastTile);
                Debug.Log("no dijkstra");
                return;
            }*/
        }

        Tile previousTile = placedTiles.Count > 0 ? placedTiles.Peek() : null;
        Point start = GetComponent<Goals>().StartPoint(lastTile, previousTile);

        GetComponent<Goals>().CmdCreate(start.x, start.y, start.z, false, GameScene.Instance.Path);


        int[,,] array = MapToArray(placedTiles, start);
        int center = array.GetLength(0) / 2;

        EmptyPath(ref currentPath);
        Queue<Point> pointQueue = Dijkstra.Solve(array, center, center, center, center);
        Debug.Log("Dijkstra success");
        placedTiles.Push(lastTile);

        while (pointQueue.Count > 0)
        {
            Point point = pointQueue.Dequeue();
            GoalTile path = new GoalTile(point.x * blockSize + start.x, point.y * blockSize + start.y, point.z * blockSize + start.z);
            currentPath.Push(path);
            GetComponent<Goals>().CmdCreate(point.x * blockSize + start.x, point.y * blockSize + start.y, point.z * blockSize + start.z, false, GameScene.Instance.Path);
        }

        if (start.x != lastTile.x || start.y != lastTile.y || start.z != lastTile.z)
        {
            GoalTile path = new GoalTile(start.x * blockSize, start.y * blockSize, start.z * blockSize);
            currentPath.Push(path);
            GetComponent<Goals>().CmdCreate(start.x * blockSize, start.y * blockSize, start.z * blockSize, false, GameScene.Instance.Path);
        }
    }

    public static void Reset()
    {
        currentPath = new Stack<GoalTile>();
        placedTiles = new Stack<Tile>();
    }
}
