using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SheepBehavior : MonoBehaviour
{
    // In order to access the Sheep's class
    private Sheep sheep;
    public enum SheepPathingType
    {
        Stationary,
        Patrolling,
        ToSweater,
        ToPlayer,
        Fleeing
    }

    public enum SheepDirection
    {
        North,
        East,
        South,
        West
    }

    public float movementSpeed = 2.0f;

    Vector2Int pos;
    Vector2Int nextPos;
    Vector2Int oldPos;
    public Vector2Int sweaterPos;
    bool pathFound;
    bool movingToNextTile;
    public SheepPathingType pathingType;
    public SheepPathingType oldPathingType;
    public List<Vector2Int> travelPath;

    // Patrol AI
    private int patrolSpotsIndex;
    public PatrolSpot[] aiPatrolSpots;

    // Start is called before the first frame update
    void Start()
    {
        pathFound = false;
        movingToNextTile = false;
        travelPath = new List<Vector2Int>();

        pathingType = SheepPathingType.Patrolling;
        oldPathingType = SheepPathingType.Patrolling;
        sheep = GetComponent<Sheep>();
        pos = Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.y));

        // AI has a patrol spot
        if (aiPatrolSpots.Length > 0)
        {
            Vector3 min = aiPatrolSpots[patrolSpotsIndex].GetPosition;

            for (int i = 0; i < aiPatrolSpots.Length; i++)
            {
                if (Vector3.Distance(transform.position, min) > Vector3.Distance(transform.position, aiPatrolSpots[i].GetPosition))
                {
                    min = aiPatrolSpots[i].GetPosition;
                    patrolSpotsIndex = i;
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {        
        switch (pathingType)
        {
            case SheepPathingType.Stationary:
                //don't move, but make sure not sheared and if so, keep an eye out for a sweater. also be alert for the wolf
                break;

            case SheepPathingType.Patrolling:
                //move along designated path, but make sure not sheared and if so, keep an eye out for a sweater. also be alert for the wolf
               if (!movingToNextTile && aiPatrolSpots.Length > 0)
                {
                    if (!sheep.IsSheared)
                    {
                        pos = GetSheepPos();

                        travelPath = Pathing.AStar(pos, this.PositionToWorldVector2Int(aiPatrolSpots[patrolSpotsIndex].GetPosition));

                        if (travelPath != null)
                        {
                            pathFound = true;
                            float dist = Vector3.Distance(transform.position, aiPatrolSpots[patrolSpotsIndex].GetPosition);
                            if (dist < 1.8f)
                            {
                                pathFound = false;
                                travelPath = null;
                                patrolSpotsIndex = (patrolSpotsIndex + 1) % aiPatrolSpots.Length;
                            } 

                        }

                    }
                }
                break;

            case SheepPathingType.ToSweater:
                //check if a path has already been made and follow that path if so, otherwise make a path to the sweater first
                oldPos = GetSheepPos();

                switch(oldPathingType)
                {
                    case SheepPathingType.Stationary:
                        ToSweater(oldPos, SheepPathingType.Stationary);
                        break;
                    case SheepPathingType.Patrolling:
                        ToSweater(oldPos, SheepPathingType.Patrolling);
                        break;
                    case SheepPathingType.ToPlayer:
                        ToSweater(oldPos, SheepPathingType.ToPlayer);
                        break;
                    case SheepPathingType.Fleeing:
                        ToSweater(oldPos, SheepPathingType.Fleeing);
                        break;
                }                

                break; 

            case SheepPathingType.ToPlayer:
                if (!movingToNextTile)
                {
                    if (!pathFound || travelPath.Count == 0 || SameRoomAsTarget() || PlayerChangedRooms())
                    {
                        pos = GetSheepPos();
                        travelPath = Pathing.AStar(GetSheepPos(), Wolf.GetWolfPos());
                        if (travelPath != null)
                        {
                            pathFound = true;

                            for (int i = 0; i < travelPath.Count; i++)
                            {
                                Debug.Log("checking path to player (" + travelPath.Count + " tiles): " + travelPath[i].x + "," + travelPath[i].y);
                            }
                        }
                    }
                }

                //if a path has already been made, and the player is in a different room than the sheep and the player is in the same room as when the path was made, go along that path
                //otherwise, make a new path first
                break;

            case SheepPathingType.Fleeing:
                //flee away from source?
                break;
        }

        Move();
    }

    void Move()
    {
        if (movingToNextTile)
        {
            Vector2Int checkPos = GetSheepPos();
            if (checkPos != pos)
            {
                movingToNextTile = false;
                pos = checkPos;
            }
        }

        if (!movingToNextTile && pathFound)
        {
            if (pathingType == SheepPathingType.Stationary) return;

            if (travelPath.Count > 0)
            {
                nextPos = travelPath[0];
                travelPath.RemoveAt(0);
                movingToNextTile = true;
            }
            else
            {
                movingToNextTile = false;
                pathFound = false;
            }
        }

        if (movingToNextTile)
        {
            if (NextPosUp())
            {
                transform.Translate(new Vector3(0, movementSpeed, 0) * Time.deltaTime);
            }
            else if (NextPosRight())
            {
                transform.Translate(new Vector3(movementSpeed, 0, 0) * Time.deltaTime);
            }
            else if (NextPosDown())
            {
                transform.Translate(new Vector3(0, -movementSpeed, 0) * Time.deltaTime);
            }
            else if (NextPosLeft())
            {
                transform.Translate(new Vector3(-movementSpeed, 0, 0) * Time.deltaTime);
            }
            else
            {
                movingToNextTile = false;
                pathFound = false;
                //Debug.LogError("Next tile isn't adjacent! Current: (" + pos.x + "," + pos.y + ") & Next: (" + nextPos.x + "," + nextPos.y + ")");
            }
            //figure out where the next tile is from current and continue in that direction
        }
    }

    bool SameRoomAsTarget()
    {
        //FIXIT actually do checking, once rooms are coded in
        switch (pathingType)
        {
            case SheepPathingType.Patrolling:
                break;
            case SheepPathingType.ToSweater:
                break;
            case SheepPathingType.ToPlayer:
                break;
        }

        return false;
    }

    bool PlayerChangedRooms()
    {
        //FIXIT actually do checking, once rooms are coded in
        return false;
    }

    void ToSweater(Vector2Int oldPosition, SheepPathingType sheepPathingType)
    {
        if(!movingToNextTile)
        {
            pos = GetSheepPos();

            travelPath = Pathing.AStar(pos, sweaterPos);

            if (travelPath != null)
            {
                pathFound = true;
            }
            else
            {
                travelPath = Pathing.AStar(pos, oldPosition);
                pathFound = true;

                if (pos == oldPosition)
                {
                    pathingType = sheepPathingType;
                    pathFound = false;
                }
            }

        }
    }

    //Quick functions to reduce rewriting
    Vector2Int PositionToWorldVector2Int(Vector2 position) { return (Vector2Int)GridManager.Instance.walkableTilemap.WorldToCell(new Vector3(position.x, position.y, 0)); }
    Vector2Int GetSheepPos() { return (Vector2Int)GridManager.Instance.walkableTilemap.WorldToCell(new Vector3(transform.position.x, transform.position.y, 0)); }
    bool NextPosUp() { return (pos.x == nextPos.x && pos.y + 1 == nextPos.y); }
    bool NextPosRight() { return (pos.x + 1 == nextPos.x && pos.y == nextPos.y); }
    bool NextPosDown() { return (pos.x == nextPos.x && pos.y - 1 == nextPos.y); }
    bool NextPosLeft() { return (pos.x - 1 == nextPos.x && pos.y == nextPos.y); }
}
