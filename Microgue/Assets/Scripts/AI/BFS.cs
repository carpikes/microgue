using System;
using UnityEngine;
using System.Collections.Generic;

using IntPoint = AIMap.IntPoint;
using System.Text;

public class BFS
{
    public static int[,] CalculateBFSMap(bool[,] map, IntPoint start)
    {
        // get dimensions of map
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        // result dijkstra map
        int[,] resMap = new int[rows, cols];

        // clear map
        for (int i = 0; i < rows; ++i)
            for (int j = 0; j < cols; ++j)
                resMap[i, j] = 0;

        // frontier and closed list
        Queue<Node> Q = new Queue<Node>();
        HashSet<IntPoint> alreadyVisited = new HashSet<IntPoint>();

        // add root node
        Node root = new Node(start);
        Q.Enqueue(root);

        while (Q.Count > 0)
        {
            // dispose of current node
            Node el = Q.Dequeue();
            IntPoint pos = el.Position;

            if (alreadyVisited.Contains(pos))
                continue;

            int g = el.PathCost; // path cost for children

            // update result
            //Debug.Log(pos);
            resMap[pos.x, pos.y] = g;

            // add to closed list
            alreadyVisited.Add(pos);

            // add children if not visited yet
            AddChildren(pos.x - 1, pos.y, g + 1, map, rows, cols, Q, alreadyVisited);
            AddChildren(pos.x + 1, pos.y, g + 1, map, rows, cols, Q, alreadyVisited);
            AddChildren(pos.x, pos.y - 1, g + 1, map, rows, cols, Q, alreadyVisited);
            AddChildren(pos.x, pos.y + 1, g + 1, map, rows, cols, Q, alreadyVisited);
        }

        // return result
        return resMap;
    }

    private static void AddChildren(int x, int y, int g, bool[,] map, int rows, int cols, Queue<Node> q, HashSet<IntPoint> alreadyVisited)
    {
        const bool WALL = true;

        // out of map
        if (!(x >= 0 && x < rows) || !(y >= 0 && y < cols))
            return;

        // if already visited, discard
        IntPoint pos = new IntPoint(x, y);
        if (alreadyVisited.Contains(pos))
            return;

        // discard if not walkable on
        if (map[x, y] == WALL)
        {
            alreadyVisited.Add(pos);
            return;
        }

        // otherwise, update
        q.Enqueue(new Node(pos, g));
    }

    public static void PrintMap(int[,] map)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < map.GetLength(0); ++i)
        {
            for (int j = 0; j < map.GetLength(1); ++j)
            {
                sb.AppendFormat("{0:00}", map[i, j] + " ");
            }
            sb.Append("\n");
        }

        Debug.Log(sb.ToString());
    }
}

