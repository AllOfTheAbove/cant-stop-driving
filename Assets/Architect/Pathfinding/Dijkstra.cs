using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;

public class Point
{
    //Unity does not handle Tuples for some reason so I had to create a custom Point class (which is just an elaborated Enum) in order to replace them
    public int x;
    public int y;
    public int z;

    public Point(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override string ToString()
    {
        return "x: " + x.ToString() + ", y: " + y.ToString() + ", z: " + z.ToString();
    }
}

public class Dijkstra
{
    //3 DIMENSIONS FUNCTIONS
    public static Queue<Point> Solve(int[,,] board, int x, int y, int z, int offset)
    {
        int height = board.GetLength(0);
        int width = board.GetLength(1);
        int length = board.GetLength(2);
        Queue<Point> output = new Queue<Point>();


        // Start = -1, Exit = -2, walls = -10

        int weight;
        Point coordinates;
        Queue<Point> next = new Queue<Point>();
        next.Enqueue(new Point(x, y, z));
        while (board[x, y, z] != -2)
        {
            coordinates = next.Dequeue();
            x = coordinates.x;
            y = coordinates.y;
            z = coordinates.z;

            if (board[x, y, z] != -10 && board[x, y, z] != -2)
            {
                weight = (board[x, y, z] == -1 ? 0 : board[x, y, z]);

                for (int i = -1; i < 2; i += 2)
                {
                    if (x + i >= 0 && x + i < height && board[x + i, y, z] <= 0)
                    {
                        next.Enqueue(new Point(x + i, y, z));
                        if (board[x + i, y, z] == 0)
                            board[x + i, y, z] = weight + 1;
                    }
                }

                for (int j = -1; j < 2; j += 2)
                {
                    if (y + j >= 0 && y + j < height && board[x, y + j, z] <= 0)
                    {
                        next.Enqueue(new Point(x, y + j, z));
                        if (board[x, y + j, z] == 0)
                            board[x, y + j, z] = weight + 1;
                    }
                }

                for (int k = -1; k < 2; k += 2)
                {
                    if (z + k >= 0 && z + k < length && board[x, y, z + k] <= 0)
                    {
                        next.Enqueue(new Point(x, y, z + k));
                        if (board[x, y, z + k] == 0)
                            board[x, y, z + k] = weight + 1;
                    }
                }

            }

            if (next.Count == 0)
                throw new ArgumentException("board has no solution");
        }

        //PrintArray(board);
        //Console.WriteLine();

        coordinates = new Point(x, y, z);

        while (board[x, y, z] != -1)
        {
            for (int i = -1; i < 2; i += 2)
            {
                if (x + i >= 0 && x + i < height)
                {
                    if (board[x + i, y, z] > 0 && board[x, y, z] == -2)
                        coordinates = new Point(x + i, y, z);
                    else if (board[x + i, y, z] > 0 && board[x + i, y, z] < board[coordinates.x, coordinates.y, coordinates.z])
                        coordinates = new Point(x + i, y, z);
                }
            }

            for (int j = -1; j < 2; j += 2)
            {
                if (y + j >= 0 && y + j < height)
                {
                    if (board[x, y + j, z] > 0 && board[x, y, z] == -2)
                        coordinates = new Point(x, y + j, z);
                    else if (board[x, y + j, z] > 0 && board[x, y + j, z] < board[coordinates.x, coordinates.y, coordinates.z])
                        coordinates = new Point(x, y + j, z);
                }
            }

            for (int k = -1; k < 2; k += 2)
            {
                if (z + k >= 0 && z + k < height)
                {
                    if (board[x, y, z + k] > 0 && board[x, y, z] == -2)
                        coordinates = new Point(x, y, z + k);
                    else if (board[x, y, z + k] > 0 && board[x, y, z + k] < board[coordinates.x, coordinates.y, coordinates.z])
                        coordinates = new Point(x, y, z + k);
                }
            }



            board[x, y, z] = -1;

            output.Enqueue(new Point(x - offset, y - offset, z - offset));

            x = coordinates.x;
            y = coordinates.y;
            z = coordinates.z;
        }

        //PrintArray(board);

        return output;
    }

    public static void PrintArray(int[,,] arr)
    {
        int height = arr.GetLength(0);
        int width = arr.GetLength(1);
        int length = arr.GetLength(2);

        for (int i = 0; i < height; i++)
        {
            for (int k = 0; k < width; k++)
            {
                for (int j = 0; j < length; j++)
                    Console.Write("| " + (arr[i, j, k] == -1 ? "S" : arr[i, j, k].ToString()) + " ");
                Console.Write("|    ");
            }
            Console.WriteLine();
        }
    }

    public static int[,,] Init(int size, int x, int y, int z)
    {
        int[,,] board = new int[size, size, size];

        Random rnd = new Random();
        Random rnd2 = new Random(rnd.Next(0, 1000));

        board[x, y, z] = -1;

        x = rnd2.Next(0, size);
        y = rnd2.Next(0, size);
        z = rnd2.Next(0, size);

        board[x, y, z] = -2;
        return board;
    }

    public static void Launch3D(int size)
    {
        Random rnd = new Random();
        int x = rnd.Next(0, size);
        int y = rnd.Next(0, size);
        int z = rnd.Next(0, size);

        foreach (var tuple in Solve(Dijkstra.Init(size, x, y, z), x, y, z, 0))
        {
            Console.Write("({0}, {1}, {2}) ", tuple.x, tuple.y, tuple.z);
        }
    }
}