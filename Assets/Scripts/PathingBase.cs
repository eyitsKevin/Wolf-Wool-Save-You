using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*  ----- FIXIT NOTES -----
 * 
 *  - I wasn't sure of a better way to get the GridManager object, so I did a FindObjectOfType - I dunno if it works
 *  - Figure out what I want returned if A-star cannot determine a path to the finish (currently null)
 *  - PathRoom currently has a statically increasing ID to separate all rooms, but if any of these functions add to this counter, it
 *      probably ruins everything. Make sure they don't or find another way to set IDs
 *  - Currently only considers everything the same room. Needs another entire function for determining best Astar approach from room
 *      to room (some of that is hardcoded)
 *  - Can't expand on this until I get rooms and links added, as well as ways to pull this data for use
 *  - NEEDS TO BE TESTED
 * 
 */


public class Pathing : MonoBehaviour
{
    List<PathTileNode> open = new List<PathTileNode>();
    List<PathTileNode> closed = new List<PathTileNode>();
    List<PathRoom> roomOpen = new List<PathRoom>();
    List<PathRoom> roomClosed = new List<PathRoom>();

    float tileDistance(Vector2 start, Vector2 finish)
    {
        return Mathf.Abs(start.x - finish.x) + Mathf.Abs(start.y - finish.y);
    }

    float euclideanDistance(Vector2 start, Vector2 finish)
    {
        return Mathf.Sqrt((start.x - finish.x) * (start.x - finish.x) + (start.y - finish.y) * (start.y - finish.y));
    }

    int insertionSort_Node(PathTileNode newNode, int minIndex, int maxIndex)
    {
        //Check if done
        if (minIndex == maxIndex)
        {
            if (newNode < open[minIndex])
            {
                return minIndex;
            }
            else
            {
                return minIndex + 1;
            }
        }
        else
        {
            //Get the halfway point
            int divider = Mathf.FloorToInt((maxIndex - minIndex) / 2.0f) + minIndex;

            if (newNode > open[divider])
            {
                //Recursively check the next middlepoint
                return insertionSort_Node(newNode, divider + 1, maxIndex);
            }
            else if (newNode < open[divider])
            {
                if (divider == minIndex)
                {
                    return minIndex;
                }
                else
                {
                    return insertionSort_Node(newNode, minIndex, divider - 1);
                }
            }

            return divider;
        }
    }

    int insertionSort_Room(PathRoom newRoom, int minIndex, int maxIndex)
    {
        //Check if done
        if (minIndex == maxIndex)
        {
            if (newRoom < roomOpen[minIndex])
            {
                return minIndex;
            }
            else
            {
                return minIndex + 1;
            }
        }
        else
        {
            //Get the halfway point
            int divider = Mathf.FloorToInt((maxIndex - minIndex) / 2.0f) + minIndex;

            if (newRoom > roomOpen[divider])
            {
                //Recursively check the next middlepoint
                return insertionSort_Room(newRoom, divider + 1, maxIndex);
            }
            else if (newRoom < roomOpen[divider])
            {
                if (divider == minIndex)
                {
                    return minIndex;
                }
                else
                {
                    return insertionSort_Room(newRoom, minIndex, divider - 1);
                }
            }

            return divider;
        }
    }

    //Function returns a bool: whether or not the end has been found
    bool addNeighbors(PathTileNode current, Vector2 finish)
    {
        GridManager grid = (GridManager)GameObject.FindObjectOfType(typeof(GridManager)); //FIXIT make sure this works

        //y+1
        Vector2 checkLoc = current.pos;
        checkLoc.y = checkLoc.y + 1.0f;
        if (grid.isWalkableTile(checkLoc))
        {
            PathTileNode upNode = new PathTileNode(checkLoc, current, current.cost + 1, tileDistance(checkLoc, finish), euclideanDistance(checkLoc, finish));
            if (checkLoc == finish)
            {
                open.Clear();
                open.Add(upNode);
                return true;
            }
            else if (!closed.Contains(upNode))
            {
                if (open.Count == 0)
                {
                    open.Add(upNode);
                }
                else
                {
                    open.Insert(insertionSort_Node(upNode, 0, open.Count - 1), upNode);
                }
            }
        }

        //x+1
        checkLoc = current.pos;
        checkLoc.x = checkLoc.x + 1.0f;
        if (grid.isWalkableTile(checkLoc))
        {
            PathTileNode rightNode = new PathTileNode(checkLoc, current, current.cost + 1, tileDistance(checkLoc, finish), euclideanDistance(checkLoc, finish));
            if (checkLoc == finish)
            {
                open.Clear();
                open.Add(rightNode);
                return true;
            }
            else if (!closed.Contains(rightNode))
            {
                if (open.Count == 0)
                {
                    open.Add(rightNode);
                }
                else
                {
                    open.Insert(insertionSort_Node(rightNode, 0, open.Count - 1), rightNode);
                }
            }
        }

        //y-1
        checkLoc = current.pos;
        checkLoc.y = checkLoc.y - 1.0f;
        if (grid.isWalkableTile(checkLoc))
        {
            PathTileNode downNode = new PathTileNode(checkLoc, current, current.cost + 1, tileDistance(checkLoc, finish), euclideanDistance(checkLoc, finish));
            if (checkLoc == finish)
            {
                open.Clear();
                open.Add(downNode);
                return true;
            }
            else if (!closed.Contains(downNode))
            {
                if (open.Count == 0)
                {
                    open.Add(downNode);
                }
                else
                {
                    open.Insert(insertionSort_Node(downNode, 0, open.Count - 1), downNode);
                }
            }
        }

        //x-1
        checkLoc = current.pos;
        checkLoc.x = checkLoc.x - 1.0f;
        if (grid.isWalkableTile(checkLoc))
        {
            PathTileNode leftNode = new PathTileNode(checkLoc, current, current.cost + 1, tileDistance(checkLoc, finish), euclideanDistance(checkLoc, finish));
            if (checkLoc == finish)
            {
                open.Clear();
                open.Add(leftNode);
                return true;
            }
            else if (!closed.Contains(leftNode))
            {
                if (open.Count == 0)
                {
                    open.Add(leftNode);
                }
                else
                {
                    open.Insert(insertionSort_Node(leftNode, 0, open.Count - 1), leftNode);
                }
            }
        }

        return false; //end not found yet
    }

    void addLinks(PathRoom current, PathRoom finish)
    {
        for (int i = 0; i < current.links.Count; i++)
        {
            current.links[i].linkedRoom(current).cost = current.cost + current.links[i].tileDistance;
            if (roomOpen.Count == 0)
            {
                roomOpen.Add(current.links[i].linkedRoom(current));
            }
            else
            {
                roomOpen.Insert(insertionSort_Room(current.links[i].linkedRoom(current), 0, roomOpen.Count - 1), current.links[i].linkedRoom(current));
            }
        }
    }

    public List<Vector2> AStar_SameRoom(Vector2 start, Vector2 finish)
    {
        open.Clear();
        closed.Clear();
        List<Vector2> path = new List<Vector2>();

        PathTileNode startNode = new PathTileNode(start, null, 0, tileDistance(start, finish), euclideanDistance(start, finish));
        open.Add(startNode);

        PathTileNode currentNode = open[0];

        bool endFound = false;
        while (open.Count > 0 && !endFound)
        {
            currentNode = open[0];
            open.RemoveAt(0);
            closed.Add(currentNode);

            endFound = addNeighbors(currentNode, finish);
        }

        if (!endFound)
        {
            //no path found to finish!
            return null; //FIXIT
        }

        //Follow the finish back to the start
        currentNode = open[0];
        while (currentNode.parent != null)
        {
            path.Insert(0, currentNode.pos); //always insert at 0
            currentNode = currentNode.parent;
        }
        path.Insert(0, currentNode.pos); //same as start. might not need to add this

        return path;
    }

    public List<PathRoom> AStar_Rooms(PathRoom start, PathRoom finish)
    {
        roomOpen.Clear();
        roomClosed.Clear();
        List<PathRoom> path = new List<PathRoom>();

        start.cost = 0;
        roomOpen.Add(start);

        PathRoom currentRoom = roomOpen[0]; //FIXIT make sure this doesn't increment ROOM_ID in PathRoom class or this won't work

        bool endFound = false;
        while (roomOpen.Count > 0 && !endFound)
        {
            currentRoom = roomOpen[0]; //FIXIT make sure this doesn't increment ROOM_ID in PathRoom class or this won't work
            roomOpen.RemoveAt(0);
            roomClosed.Add(currentRoom);

            //Unlike with tiles, doesn't exit as soon as it finds the finish, because costs aren't all equal to one. Not actually
            //  A-star unless we check all routes until the finish room ends up in the shortest position (index 0 in roomOpen list)
            if (currentRoom.Equals(finish))
            {
                endFound = true;
                break;
            }
            else
            {
                addLinks(currentRoom, finish);
            }
        }

        if (!endFound)
        {
            //impossible to get to target room!
            return null; //FIXIT
        }

        //Follow the finish back to the start
        currentRoom = roomOpen[0]; //FIXIT make sure this doesn't increment ROOM_ID in PathRoom class or this won't work
        while (currentRoom.previous != null)
        {
            path.Insert(0, currentRoom); //always insert at 0
            currentRoom = currentRoom.previous;
        }
        path.Insert(0, currentRoom); //same as start. might not need to add this

        return path;
    }

    public List<Vector2> AStar(Vector2 start, Vector2 finish)
    {
        //step 1: consult the hardcoded list of room charts to see if the start & finish tiles are in the same room

        //if tile of start is in the same room of tile of finish:
        return AStar_SameRoom(start, finish);
        //else, continue steps

        //step 2: consult the hardcoded list of room charts to see the fastest route from start room to finish room

        //step 3: use the results of step 2 to determine which exit of start room to move towards

        //step 4: consult the hardcoded list of room charts to figure out the tile of the exit

        //if tile of exit != start tile: AStar_SameRoom from current room to exit of current room
        //else, continue steps

        //step 5: consult the hardcoded list of room charts to figure out the tile of the next exit

        //step 6: AStar_SameRoom to step 4's result
    }
}
