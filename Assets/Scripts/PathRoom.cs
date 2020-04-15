using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRoom
{
    private Vector2Int corner1;
    private Vector2Int corner2;
    private Vector2Int exit;
    private string name;

    public PathRoom(string roomName, Vector2Int cornerA, Vector2Int cornerB, Vector2Int exitLoc)
    {
        name = roomName;
        corner1 = cornerA;
        corner2 = cornerB;
        exit = exitLoc;
    }

    public Vector2Int getCorner1() { return corner1; }
    public Vector2Int getCorner2() { return corner2; }
    public Vector2Int getExit() { return exit; }
    public string getName() { return name; }

    public bool isInRoom(Vector2Int loc)
    {
        if (loc.x >= corner1.x && loc.x <= corner2.x ||
            loc.x >= corner2.x && loc.x <= corner1.x)
        {
            if (loc.y >= corner1.y && loc.y <= corner2.y ||
                loc.y >= corner2.y && loc.y <= corner1.y)
            {
                return true;
            }
        }

        return false;
    }
}
