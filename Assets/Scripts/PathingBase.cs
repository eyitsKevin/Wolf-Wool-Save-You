using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*  ----- FIXIT NOTES -----
 * 
 *  - Create <bool checkWalkableArea(Vector2 loc)> method that checks the actual tilemap to see if a tile can be walked on. Use esti's tilemap info for it
 *  - Figure out what I want returned if A-star cannot determine a path to the finish (currently null)
 *  - Currently only considers everything the same room. Needs another entire function for determining best Astar approach from room to room (some of that is hardcoded)
 *  - NEEDS TO BE TESTED
 * 
 */


public class Pathing : MonoBehaviour
{
    class TileNode
    {
        public Vector2 pos;
        public int cost;
        public float heuristic_tileDistance;
        public float heuristic_euclideanDistance;
        public TileNode parent;

        public TileNode(Vector2 loc, TileNode previous, int spacesSoFar, float heuristic_tile = 0.0f, float heuristic_euclidean = 0.0f)
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
            TileNode node = (TileNode)obj;

            if (node.pos == null)
            {
                return false;
            }

            return (pos == node.pos);
        }

        //Static comparison functions. Required for comparing node heuristics
        public static bool operator > (TileNode node1, TileNode node2)
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
        public static bool operator < (TileNode node1, TileNode node2)
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

    List<TileNode> open = new List<TileNode>();
    List<TileNode> closed = new List<TileNode>();

    float tileDistance(Vector2 start, Vector2 finish)
    {
        return Mathf.Abs(start.x - finish.x) + Mathf.Abs(start.y - finish.y);
    }

    float euclideanDistance(Vector2 start, Vector2 finish)
    {
        return Mathf.Sqrt((start.x - finish.x) * (start.x - finish.x) + (start.y - finish.y) * (start.y - finish.y));
    }

    int insertionSort(TileNode newNode, int minIndex, int maxIndex)
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
                //Recursively get 
                return insertionSort(newNode, divider + 1, maxIndex);
            }
            else if (newNode > open[divider])
            {
                if (divider == minIndex)
                {
                    return minIndex;
                }
                else
                {
                    return insertionSort(newNode, minIndex, divider - 1);
                }
            }

            return divider;
        }
    }

    //Function returns a bool: whether or not the end has been found
    bool addNeighbors(TileNode current, Vector2 finish)
    {
        //y+1
        Vector2 checkLoc = current.pos;
        checkLoc.y = checkLoc.y + 1.0f;
        if (true) //FIXIT if (checkWalkableArea(checkLoc))
        {
            TileNode upNode = new TileNode(checkLoc, current, current.cost + 1, tileDistance(checkLoc, finish), euclideanDistance(checkLoc, finish));
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
                    insertionSort(upNode, 0, open.Count - 1);
                }
            }
        }

        //x+1
        checkLoc = current.pos;
        checkLoc.x = checkLoc.x + 1.0f;
        if (true) //FIXIT if (checkWalkableArea(checkLoc))
        {
            TileNode rightNode = new TileNode(checkLoc, current, current.cost + 1, tileDistance(checkLoc, finish), euclideanDistance(checkLoc, finish));
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
                    insertionSort(rightNode, 0, open.Count - 1);
                }
            }
        }

        //y-1
        checkLoc = current.pos;
        checkLoc.y = checkLoc.y - 1.0f;
        if (true) //FIXIT if (checkWalkableArea(checkLoc))
        {
            TileNode downNode = new TileNode(checkLoc, current, current.cost + 1, tileDistance(checkLoc, finish), euclideanDistance(checkLoc, finish));
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
                    insertionSort(downNode, 0, open.Count - 1);
                }
            }
        }

        //x-1
        checkLoc = current.pos;
        checkLoc.x = checkLoc.x - 1.0f;
        if (true) //FIXIT if (checkWalkableArea(checkLoc))
        {
            TileNode leftNode = new TileNode(checkLoc, current, current.cost + 1, tileDistance(checkLoc, finish), euclideanDistance(checkLoc, finish));
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
                    insertionSort(leftNode, 0, open.Count - 1);
                }
            }
        }

        return false;
    }

    public List<Vector2> AStar_SameRoom(Vector2 start, Vector2 finish)
    {
        List<Vector2> path = new List<Vector2>();

        TileNode startNode = new TileNode(start, null, 0, tileDistance(start, finish), euclideanDistance(start, finish));
        open.Add(startNode);

        TileNode currentNode = open[0];

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
}
