using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRoomLink : MonoBehaviour
{
    public PathRoom roomA;
    public PathRoom roomB;
    public int tileDistance;

    public PathRoomLink(PathRoom room1, PathRoom room2, int tileDistanceBetweenRooms = 0)
    {
        roomA = room1;
        roomB = room2;
        tileDistance = tileDistanceBetweenRooms;
    }

    //Override equals. Required for searching a list for the link
    public override bool Equals(object obj)
    {
        PathRoomLink link = (PathRoomLink)obj;

        return (link.roomA == roomA && link.roomB == roomB);
    }
    public bool Equals(PathRoom room1, PathRoom room2)
    {
        PathRoomLink tempLink = new PathRoomLink(room1, room2);
        return this.Equals(tempLink);
    }
}
