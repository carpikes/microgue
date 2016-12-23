using UnityEngine;
using System.Collections;

using IntPoint = AIMap.IntPoint;

public class Node
{
    IntPoint mPos;
    int g;

    public Node(IntPoint pos, int pathCost = 1)
    {
        Position = pos;
        g = pathCost;
    }

    public int PathCost
    {
        get
        {
            return g;
        }

        set
        {
            g = value;
        }
    }

    public IntPoint Position
    {
        get
        {
            return mPos;
        }

        set
        {
            mPos = value;
        }
    }
}