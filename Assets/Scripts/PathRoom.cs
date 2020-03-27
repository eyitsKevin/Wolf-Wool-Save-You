using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRoom : MonoBehaviour
{
    public static int ROOM_ID = 0;
    public static List<string> ROOM_NAMES = new List<string>();

    public List<Vector2> exits;
    public int id;
    public string name;

    public PathRoom(string roomName)
    {
        name = roomName;
        exits = new List<Vector2>();

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
}
