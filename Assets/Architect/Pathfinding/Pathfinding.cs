using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pathfinding : NetworkBehaviour
{

    private Stack<Tile> placedTiles = new Stack<Tile>();
    public Stack<GoalTile> currentPath = new Stack<GoalTile>();

    private GoalTile pathStart = null;

    private static int blockSize = 16;


    public void EmptyPath(Stack<GoalTile> path)
    {
        //Debug.Log("EMPTY");
        foreach (GoalTile tile in path)
        {
            NetworkServer.Destroy(tile.gameObject);
        }
        currentPath = new Stack<GoalTile>();
    }

    private static int[,,] MapToArray(Stack<Tile> placedTiles, Point start)
    {
        int size = 2 * Math.Max(Math.Abs(Goals.goal.x - start.x) / blockSize, Math.Abs(Goals.goal.z - start.z) / blockSize) + 3;

        int[,,] array = new int[size, size, size];
        int center = array.GetLength(0) / 2;

        array[center, center, center] = -1;

        foreach (Tile tile in placedTiles)
        {
            try
            {
                array[(tile.x - start.x) / blockSize, (tile.y - start.y) / blockSize, (tile.z - start.z) / blockSize] = -10;
            }
            catch (Exception) { }
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
        //Debug.Log("SET");



        if (currentPath.Count > 0)
        {
            GoalTile pathStart = currentPath.Pop();
            NetworkServer.Destroy(pathStart.gameObject);
        }

        if (pathStart != null)
        {
            NetworkServer.Destroy(pathStart.gameObject);
            pathStart = null;
        }

        Tile previousTile = placedTiles.Count > 0 ? placedTiles.Peek() : null;

        Point start = GetComponent<Goals>().StartPoint(lastTile, previousTile);
        GoalTile startTile = new GoalTile(start.x, start.y, start.z);

        pathStart = startTile;

        startTile.gameObject = GetComponent<Goals>().Create(start.x, start.y, start.z, false, GameScene.Instance.Path);



        int[,,] array = MapToArray(placedTiles, start);
        int center = array.GetLength(0) / 2;

        EmptyPath(currentPath);
        Queue<Point> pointQueue = Dijkstra.Solve(array, center, center, center, center);
        placedTiles.Push(lastTile);

        while (pointQueue.Count > 0)
        {
            Point point = pointQueue.Dequeue();
            GoalTile path = new GoalTile(point.x * blockSize + start.x, point.y * blockSize + start.y, point.z * blockSize + start.z);
            path.gameObject = GetComponent<Goals>().Create(point.x * blockSize + start.x, point.y * blockSize + start.y, point.z * blockSize + start.z, false, GameScene.Instance.Path);
            currentPath.Push(path);
        }
    }

    public void Reset()
    {
        EmptyPath(currentPath);
        placedTiles = new Stack<Tile>();
    }
}
