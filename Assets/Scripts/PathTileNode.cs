using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTileNode
{
    public Vector2Int pos;
    public int cost;
    public float heuristic_tileDistance;
    public float heuristic_euclideanDistance;
    public PathTileNode parent;

    public PathTileNode(Vector2Int loc, PathTileNode previous, int spacesSoFar, float heuristic_tile = 0.0f, float heuristic_euclidean = 0.0f)
    {
        pos = loc;
        parent = previous;
        cost = spacesSoFar;
        heuristic_tileDistance = heuristic_tile;
        heuristic_euclideanDistance = heuristic_euclidean;
    }


    //Override equals. Required for searching a list for the node
    public override bool Equals(object obj)
    {
        PathTileNode node = (PathTileNode)obj;

        if (node.pos == null)
        {
            return false;
        }

        return (pos == node.pos);
    }

    //Static comparison functions. Required for comparing node heuristics
    public static bool operator >(PathTileNode node1, PathTileNode node2)
    {
        if (node1.cost + node1.heuristic_tileDistance > node2.cost + node2.heuristic_tileDistance)
        {
            return true;
        }

        if (node1.cost + node1.heuristic_tileDistance == node2.cost + node2.heuristic_tileDistance &&
            node1.heuristic_euclideanDistance > node2.heuristic_euclideanDistance)
        {
            return true;
        }

        return false;
    }
    public static bool operator <(PathTileNode node1, PathTileNode node2)
    {
        if (node1.cost + node1.heuristic_tileDistance < node2.cost + node2.heuristic_tileDistance)
        {
            return true;
        }

        if (node1.cost + node1.heuristic_tileDistance == node2.cost + node2.heuristic_tileDistance &&
            node1.heuristic_euclideanDistance < node2.heuristic_euclideanDistance)
        {
            return true;
        }

        return false;
    }
}
