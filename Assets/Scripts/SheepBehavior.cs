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
        Fleeing,
        Returning
    }

    public enum SheepDirection
    {
        North,
        East,
        South,
        West
    }

    public GameObject DEBUG_OBJECT;
    // public data members
    [Header("Steering")]
    public float movementSpeed = 2.0f;
    public float angularVelocity = 360;
    public float arrivalDistance = 1.8f;

    [Header("Pathing")]
    public bool pathFound;
    public SheepPathingType pathingType;
    public SheepPathingType oldPathingType;
    public List<Vector2> travelPath;
    public LayerMask obstacleMask;

    [Header("Behaviours")]
    public bool movingToNextTile;
    public bool fleeing;
    public float howlTimer;
    public Vector2 sweaterPos;

    // private data members
    Transform slot;
    Vector2 pos;
    public Vector2 nextPos;
    Vector2 oldPos;
    Vector3 oldScale;
    Quaternion oldRot;
    Transform detectionComponent;

    Wolf wolf;

    // Patrol AI used for single sheep patrolling
    private int patrolSpotsIndex;
    public PatrolSpot[] aiPatrolSpots;

    //Animation
    public bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        pathFound = false;
        movingToNextTile = false;
        travelPath = new List<Vector2>();
        pathingType = SheepPathingType.Stationary; // stationary, unless they have patrol spots
        nextPos = transform.position;
        sheep = GetComponent<Sheep>();
        pos = Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.y));
        
        wolf = GameObject.Find("Wolf").GetComponent<Wolf>();
        howlTimer = 0;
        detectionComponent = transform.GetChild(3);
        // AI has a patrol spot
        if (aiPatrolSpots.Length > 0)
        {
            pathingType = SheepPathingType.Patrolling;
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
        oldPathingType = pathingType;
    }

    // Update is called once per frame
    void Update()
    {
        // OldDecisionMaking();
        switch (pathingType)
        {
            case SheepPathingType.Stationary:
                oldPos = transform.position;
                oldRot = detectionComponent.rotation;
                oldScale = transform.localScale;
                break;
            case SheepPathingType.Patrolling:
                Patrol();
                break;
            case SheepPathingType.ToSweater:
                ChaseSweater();
                break;
            case SheepPathingType.ToPlayer:
                ChasePlayer();
                break;
            case SheepPathingType.Returning:
                ReturnToSpot();
                break;
            case SheepPathingType.Fleeing:
                Flee();
                break;
        }

        Move();
    }

    void Move()
    {
        // if already at spot, dont move
        if ((nextPos - (Vector2)transform.position).magnitude < arrivalDistance)
        {
            Debug.Log("Standing Still");
            isMoving = false;
            return;
        }

        // if no path, generate path
        if (travelPath.Count == 0)
        {
            Debug.Log("Need to generate a path!");
            travelPath = GeneratePath(nextPos);
        }

        if(travelPath.Count > 0)
        {
            if ((travelPath[0] - (Vector2)transform.position).magnitude < arrivalDistance)
            {
                Debug.Log("Arrived to node");
                travelPath.RemoveAt(0);
            }
            Arrive();
        }
        else
        {
            isMoving = false;
        }
    }

    void Flee()
    {    
        // only occurs if near a wolf howl
        if (howlTimer > 0)
        {
            howlTimer -= Time.deltaTime;
            nextPos = ((Vector2)transform.position - wolf.GetWolfPos()) + (Vector2)transform.position;
        }
        else
        {
            pathingType = SheepPathingType.Returning; // change to ToPlayer if flee will only be used if all sheep chase the player
            travelPath.Clear();
        }
    }


    void ReturnToSpot()
    {
        nextPos = oldPos;
        if (((Vector2)transform.position - oldPos).magnitude < arrivalDistance)
        {
            pathingType = oldPathingType;
            travelPath.Clear();
            if (oldPathingType == SheepPathingType.Stationary)
            {
                transform.localScale = oldScale;
                detectionComponent.rotation = oldRot;
            }
        }
    }

    void ChasePlayer()
    {
        nextPos = wolf.GetWolfPos();
        // Return to old position if wolf escapes sheep's FoV
        if (wolf.escaped) 
        {
            pathingType = SheepPathingType.Returning;
            travelPath.Clear();
        }
    }

    void ChaseSweater()
    {
        if (this.tag == "Clothed")
        {
            pathingType = SheepPathingType.Returning;
            return;
        }
        nextPos = sweaterPos;
    }

    private void Patrol()
    {
        oldPathingType = SheepPathingType.Patrolling;
        if(slot == null)
        {
            throw new System.Exception(gameObject.name + " sheep needs to be assigned to slot");
        }
        nextPos = slot.position;
    }

    void Arrive()
    {
        if(travelPath.Count == 0) { return; }
        Vector2 direction = travelPath[0] - (Vector2)transform.position;
        
        // if obstacle, then recalculate
        RaycastHit2D hit = Physics2D.Raycast(transform.position, travelPath[0], direction.magnitude, obstacleMask);
        if (hit)
        {
            // arrive // DEBUG_OBJECT.transform.position = hit.point;
            travelPath.Clear();
            return;
        }

        if(direction.magnitude < arrivalDistance)
        {
            isMoving = false;
            return;
        }
        isMoving = true;
        float angle = Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI - 90;
        transform.position = (Vector2)transform.position + movementSpeed * direction.normalized * Time.deltaTime;
        detectionComponent.rotation = Quaternion.RotateTowards(detectionComponent.rotation, Quaternion.Euler(0,0,angle), angularVelocity * Time.deltaTime);
    }

    List<Vector2> GeneratePath(Vector2 destination)
    {
        List<Vector2> path = new List<Vector2>();
        Vector2 direction = destination - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude, obstacleMask);
        if(hit)
        {
            Debug.Log("Sheep raycasted this: " + hit.transform.tag);

            pos = transform.position;
            List<Vector2Int> pathInt = Pathing.AStar(PositionToWorldVector2Int(pos), PositionToWorldVector2Int(destination));
            
            foreach(Vector2Int node in pathInt)
            {
                path.Add(node);
            }            
        }
        else
        {
            path.Add(destination);
            // DEBUG_OBJECT.transform.position = destination;
        }

        Debug.Log("Generated path size is " + path.Count);
        return path;
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

    // Group formation sets target slot for each sheep
    public void SetSlot(Transform target)
    {
        slot = target;
    }

    void OldDecisionMaking()
    {

        switch (pathingType)
        {
            case SheepPathingType.Stationary:
                //sheep doesn't move, keep information on its position, FoV rotation and facing direction

                oldPos = transform.position;
                oldRot = detectionComponent.rotation;
                oldScale = transform.localScale;

                break;

            case SheepPathingType.Patrolling:
                //move along designated path, keep information on its current position, FoV rotation and facing direction
                //make sure the patrol points are close to each other for more natural behaviour

                oldPos = transform.position;
                oldRot = detectionComponent.rotation;
                oldScale = transform.localScale;

                if (!movingToNextTile && aiPatrolSpots.Length > 0)
                {
                    if (!sheep.IsSheared)
                    {
                        if (slot != null)
                        {
                            travelPath = GeneratePath(slot.position);
                            if (travelPath != null)
                            {
                                pathFound = true;
                                float dist = Vector3.Distance(transform.position, slot.position);
                                if (dist < arrivalDistance)
                                {
                                    pathFound = false;
                                    travelPath = null;
                                }
                            }
                        }
                        else
                        {
                            Patrol();
                        }
                    }
                }
                break;

            case SheepPathingType.ToSweater:
                //make naked sheep go to sweater, then return to its original path
                if (!movingToNextTile)
                {
                    if (this.tag == "Clothed")
                    {
                        pathingType = SheepPathingType.Returning;
                    }
                    else if (!pathFound || travelPath.Count == 0)
                    {
                        travelPath = GeneratePath(sweaterPos);
                        if (travelPath != null)
                        {
                            pathFound = true;
                        }
                    }
                }
                break;

            case SheepPathingType.ToPlayer:
                if (!movingToNextTile)
                {
                    if (!pathFound || travelPath.Count == 0 || SameRoomAsTarget() || PlayerChangedRooms())
                    {
                        travelPath = GeneratePath(wolf.GetWolfPos());
                        if (travelPath != null)
                        {
                            pathFound = true;

                            /*for (int i = 0; i < travelPath.Count; i++)
                            {
                                Debug.Log("checking path to player (" + travelPath.Count + " tiles): " + travelPath[i].x + "," + travelPath[i].y);
                            }*/
                        }
                    }
                    if (wolf.escaped) // Return to old position if wolf escapes sheep's FoV
                    {
                        pathingType = SheepPathingType.Returning;
                    }
                    else
                    {
                        // increment detection meter
                    }
                }

                //if a path has already been made, and the player is in a different room than the sheep and the player is in the same room as when the path was made, go along that path
                //otherwise, make a new path first
                break;

            case SheepPathingType.Fleeing:
                //only occurs if near a wolf howl
                if (howlTimer > 0)
                {
                    howlTimer -= Time.deltaTime;
                    transform.Translate((this.transform.position - wolf.transform.position).normalized * movementSpeed * Time.deltaTime);
                    if (transform.localScale == new Vector3(1, 1, 1))
                    {
                        transform.localScale = new Vector3(-1, 1, 1);
                    }
                }
                else
                {
                    pathingType = SheepPathingType.Returning; // change to ToPlayer if flee will only be used if all sheep chase the player
                }
                break;

            case SheepPathingType.Returning:
                if (!movingToNextTile)
                {
                    if (pos == oldPos)
                    {
                        pathingType = oldPathingType;
                        if (oldPathingType == SheepPathingType.Stationary)
                        {
                            transform.localScale = oldScale;
                            detectionComponent.rotation = oldRot;
                        }
                    }
                    else if (!pathFound || travelPath.Count == 0)
                    {
                        travelPath = GeneratePath(oldPos);
                        if (travelPath != null)
                        {

                            pathFound = true;
                            // movingToNextTile = true;
                        }
                    }
                }
                break;
        }

        isMoving = false;

        Move();
    }

    //Quick functions to reduce rewriting
    public Vector2Int PositionToWorldVector2Int(Vector2 position) { return (Vector2Int)GridManager.Instance.walkableTilemap.WorldToCell(new Vector3(position.x, position.y, 0)); }
    Vector2Int GetSheepPos() { return (Vector2Int)GridManager.Instance.walkableTilemap.WorldToCell(new Vector3(transform.position.x, transform.position.y, 0)); }
}
