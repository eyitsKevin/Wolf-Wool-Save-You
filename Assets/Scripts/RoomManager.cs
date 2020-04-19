using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager roomManager;
    public const int MAINAREA_INNER_X = 67;
    public const int MAINAREA_OUTER_X = 80;
    public const int MAINAREA_UPPERTUNNEL_UPPERY = 10;
    public const int MAINAREA_UPPERTUNNEL_LOWERY = 6;
    public const int MAINAREA_LOWERTUNNEL_UPPERY = -6;
    public const int MAINAREA_LOWERTUNNEL_LOWERY = -9;
    public const int EXIT_Y = -16;
    public const int EXIT_LEFTX = 108;
    public const int EXIT_RIGHTX = 111;
    public const int FINAL_ROOM_STAY_Y = -48;
    public const int FINAL_ROOM_START_Y = -46;

    public List<PathRoom> rooms;
    public PathRoom voiceRoom;

    // Start is called before the first frame update
    void Start()
    {
        if (roomManager == null)
        {
            roomManager = this;
        }

        rooms = new List<PathRoom>();

        rooms.Add(new PathRoom("entrance", new Vector2Int(-24, -6), new Vector2Int(17, 6), new Vector2Int(18, 0), new Vector2Int(16, 0)));
        rooms.Add(new PathRoom("up side room 1", new Vector2Int(18, 21), new Vector2Int(27, 15), new Vector2Int(23, 14), new Vector2Int(23, 16)));
        rooms.Add(new PathRoom("up side room 2", new Vector2Int(30, 21), new Vector2Int(38, 15), new Vector2Int(34, 14), new Vector2Int(34, 16)));
        rooms.Add(new PathRoom("up side room 3", new Vector2Int(41, 21), new Vector2Int(49, 15), new Vector2Int(45, 14), new Vector2Int(45, 16)));
        rooms.Add(new PathRoom("up side room 4", new Vector2Int(52, 21), new Vector2Int(60, 15), new Vector2Int(56, 14), new Vector2Int(56, 16)));
        rooms.Add(new PathRoom("down side room 1", new Vector2Int(18, -20), new Vector2Int(27, -15), new Vector2Int(23, -14), new Vector2Int(23, -16)));
        rooms.Add(new PathRoom("down side room 2", new Vector2Int(30, -20), new Vector2Int(38, -15), new Vector2Int(34, -14), new Vector2Int(34, -16)));
        rooms.Add(new PathRoom("down side room 3", new Vector2Int(41, -20), new Vector2Int(50, -15), new Vector2Int(45, -14), new Vector2Int(45, -16)));
        rooms.Add(new PathRoom("down side room 4", new Vector2Int(53, -20), new Vector2Int(67, -15), new Vector2Int(57, -14), new Vector2Int(57, -16)));

        voiceRoom = new PathRoom("voice room", new Vector2Int(100, 3), new Vector2Int(112, -3), new Vector2Int(113, -4), new Vector2Int(113, -2));
    }

    public static bool InExitRoom(Vector2Int loc)
    {
        return roomManager.voiceRoom.isInRoom(loc);
    }

    /// <summary>
    /// Returns null if the room isn't found. This means they're in the main area
    /// </summary>
    public static PathRoom getRoomOf(Vector2Int loc)
    {
        for (int i = 0; i < roomManager.rooms.Count; i++)
        {
            if (roomManager.rooms[i].isInRoom(loc))
            {
                return roomManager.rooms[i];
            }
        }
        return null;
    }

    public static bool inSameRoom(Vector2Int loc1, Vector2Int loc2)
    {
        PathRoom room1 = getRoomOf(loc1);
        if (room1 != null && room1 == getRoomOf(loc2))
        {
            return true;
        }

        return false;
    }
}
