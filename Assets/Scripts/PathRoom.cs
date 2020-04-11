using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRoom
{
    public static int ROOM_ID = 0;
    public static List<string> ROOM_NAMES = new List<string>();

    public List<Vector2Int> exits;
    public List<PathRoomLink> links;
    public int id;
    public string name;

    public int cost = 0; //Only used in pathing
    public PathRoom previous = null; //Only used in pathing

    public PathRoom(string roomName)
    {
        name = roomName;
        exits = new List<Vector2Int>();
        links = new List<PathRoomLink>();

        //For easy lookup
        id = ROOM_ID;
        ROOM_ID++;
        ROOM_NAMES.Add(roomName);
    }

    public static int roomIDByName(string roomName)
    {
        for (int i = 0; i < ROOM_NAMES.Count; i++)
        {
            if (ROOM_NAMES[i] == roomName)
            {
                return i;
            }
        }

        return -1; //DOES NOT EXIST
    }

    public static void ConnectRooms(PathRoom roomA, PathRoom roomB, int distance)
    {
        PathRoomLink newLink = new PathRoomLink(roomA, roomB, distance);

        roomA.links.Add(newLink);
        roomB.links.Add(newLink);
    }

    //Override equals. Required for searching a list for the link
    public override bool Equals(object obj)
    {
        PathRoom link = (PathRoom)obj;

        return (link.id == id);
    }


    //Static comparison functions. Required for comparing room heuristics
    public static bool operator >(PathRoom room1, PathRoom room2)
    {
        if (room1.cost > room2.cost)
        {
            return true;
        }

        return false;
    }
    public static bool operator <(PathRoom room1, PathRoom room2)
    {
        if (room1.cost < room2.cost)
        {
            return true;
        }

        return false;
    }
}
