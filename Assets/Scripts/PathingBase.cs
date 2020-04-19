using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*  ----- FIXIT NOTES -----
 * 
 *  - there's still some random to fix
 * 
 */


public class Pathing
{
    static List<PathTileNode> open = new List<PathTileNode>();
    static List<PathTileNode> closed = new List<PathTileNode>();
    static List<PathRoom> roomOpen = new List<PathRoom>();
    static List<PathRoom> roomClosed = new List<PathRoom>();

    static float tileDistance(Vector2Int start, Vector2Int finish)
    {
        return Mathf.Abs(start.x - finish.x) + Mathf.Abs(start.y - finish.y);
    }

    static float euclideanDistance(Vector2Int start, Vector2Int finish)
    {
        return Mathf.Sqrt((start.x - finish.x) * (start.x - finish.x) + (start.y - finish.y) * (start.y - finish.y));
    }

    static int insertionSort_Node(PathTileNode newNode, int minIndex, int maxIndex)
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

    //Function returns a bool: whether or not the end has been found
    static bool addNeighbors(PathTileNode current, Vector2Int finish)
    {
        //GridManager grid = (GridManager)GameObject.FindObjectOfType(typeof(GridManager)); //FIXIT make sure this works

        //y+1
        Vector2Int checkLoc = current.pos;
        checkLoc.y = checkLoc.y + 1;
        if (GridManager.Instance.isWalkableTile(checkLoc))
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
        checkLoc.x = checkLoc.x + 1;
        if (GridManager.Instance.isWalkableTile(checkLoc))
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
        checkLoc.y = checkLoc.y - 1;
        if (GridManager.Instance.isWalkableTile(checkLoc))
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
        checkLoc.x = checkLoc.x - 1;
        if (GridManager.Instance.isWalkableTile(checkLoc))
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

    static public List<Vector2Int> AStar_SameRoom(Vector2Int start, Vector2Int finish)
    {
        if (!GridManager.Instance.isWalkableTile(finish))
        {
            Debug.LogError("End is unreachable");
            return null;
        }

        open.Clear();
        closed.Clear();
        List<Vector2Int> path = new List<Vector2Int>();

        PathTileNode startNode = new PathTileNode(start, null, 0, tileDistance(start, finish), euclideanDistance(start, finish));
        open.Add(startNode);

        PathTileNode currentNode = open[0];

        int giveUpCounter = 0;

        bool endFound = false;
        while (open.Count > 0 && !endFound && giveUpCounter < 1000)
        {
            //Debug.Log("Checking node: " + currentNode.pos.x + "," + currentNode.pos.y);
            currentNode = open[0];
            open.RemoveAt(0);
            closed.Add(currentNode);
            endFound = addNeighbors(currentNode, finish);
            giveUpCounter++;
        }

        if (!endFound)
        {
            Debug.LogError("No end found");
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
        //path.Insert(0, currentNode.pos); //same as start. might not need to add this

        return path;
    }

    /// <summary>
    /// Should return a list of Vector2Int from current position -> end position. Needs testing to be sure
    /// </summary>
    static public List<Vector2Int> AStar(Vector2Int start, Vector2Int finish)
    {
        if (RoomManager.inSameRoom(start, finish))
        {
            return AStar_SameRoom(start, finish);
        }
        else if (RoomManager.getRoomOf(start) != null)
        {
            return AStar_SameRoom(start, RoomManager.getRoomOf(start).getExit());
        }
        else if (start.x <= RoomManager.MAINAREA_INNER_X && RoomManager.getRoomOf(finish) != null)
        {
            return AStar_SameRoom(start, RoomManager.getRoomOf(finish).getExit());
        }
        if (start.x <= RoomManager.MAINAREA_OUTER_X && finish.x <= RoomManager.MAINAREA_OUTER_X ||
            start.x >= RoomManager.MAINAREA_INNER_X && finish.x >= RoomManager.MAINAREA_INNER_X)
        {
            return AStar_SameRoom(start, finish);
        }
        else if (start.x <= RoomManager.MAINAREA_INNER_X)
        {
            List<Vector2Int> node1 = AStar_SameRoom(start, new Vector2Int(RoomManager.MAINAREA_INNER_X + 1, RoomManager.MAINAREA_UPPERTUNNEL_LOWERY));
            List<Vector2Int> node2 = AStar_SameRoom(start, new Vector2Int(RoomManager.MAINAREA_INNER_X + 1, RoomManager.MAINAREA_LOWERTUNNEL_UPPERY));

            if (node1.Count < node2.Count)
            {
                List<Vector2Int> node3 = AStar_SameRoom(start, new Vector2Int(RoomManager.MAINAREA_INNER_X + 1, RoomManager.MAINAREA_UPPERTUNNEL_UPPERY));
                if (node1.Count <= node3.Count)
                {
                    return node1;
                }
                else
                {
                    return node3;
                }
            }
            else if (node1.Count > node2.Count)
            {
                List<Vector2Int> node3 = AStar_SameRoom(start, new Vector2Int(RoomManager.MAINAREA_INNER_X + 1, RoomManager.MAINAREA_LOWERTUNNEL_LOWERY));
                if (node2.Count <= node3.Count)
                {
                    return node2;
                }
                else
                {
                    return node3;
                }
            }
            else
            {
                //FIXIT randomly choose one
                return node1;
            }
        }
        else
        {
            List<Vector2Int> node1 = AStar_SameRoom(start, new Vector2Int(RoomManager.MAINAREA_OUTER_X - 1, RoomManager.MAINAREA_UPPERTUNNEL_LOWERY));
            List<Vector2Int> node2 = AStar_SameRoom(start, new Vector2Int(RoomManager.MAINAREA_OUTER_X - 1, RoomManager.MAINAREA_LOWERTUNNEL_UPPERY));

            if (node1.Count < node2.Count)
            {
                List<Vector2Int> node3 = AStar_SameRoom(start, new Vector2Int(RoomManager.MAINAREA_OUTER_X - 1, RoomManager.MAINAREA_UPPERTUNNEL_UPPERY));
                if (node1.Count <= node3.Count)
                {
                    return node1;
                }
                else
                {
                    return node3;
                }
            }
            else if (node1.Count > node2.Count)
            {
                List<Vector2Int> node3 = AStar_SameRoom(start, new Vector2Int(RoomManager.MAINAREA_OUTER_X - 1, RoomManager.MAINAREA_LOWERTUNNEL_LOWERY));
                if (node2.Count <= node3.Count)
                {
                    return node2;
                }
                else
                {
                    return node3;
                }
            }
            else
            {
                //FIXIT randomly choose one
                return node1;
            }
        }
    }
}
