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

        return (link.roomA.Equals(roomA) && link.roomB.Equals(roomB) || link.roomA.Equals(roomB) && link.roomB.Equals(roomA));
    }
    public bool Equals(PathRoom room1, PathRoom room2)
    {
        PathRoomLink tempLink = new PathRoomLink(room1, room2);
        return this.Equals(tempLink);
    }

    //Checks if connected to the specified room (used when checking if we found the finish room in AStar)
    public bool connectedToRoom(PathRoom room)
    {
        if (room == null)
        {
            return false;
        }

        if (room.Equals(roomA) || room.Equals(roomB))
        {
            return true;
        }

        return false;
    }

    //Returns the other room of the link
    public PathRoom linkedRoom(PathRoom room)
    {
        if (roomA.Equals(room))
        {
            return roomB;
        }
        else if (roomB.Equals(room))
        {
            return roomA;
        }

        return null;
    }
}
