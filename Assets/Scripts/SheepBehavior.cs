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

    public const float GIVE_UP_TIMER_MAX = 5;
    public const float GIVE_UP_TIMER_MAX_G = 15;
    public const float HOWL_DURATION = 5;

    public GameObject DEBUG_OBJECT;
    // public data members
    [Header("Steering")]
    public float movementSpeed = 2.0f;
    public float angularVelocity = 360;
    public float arrivalDistance = 0.8f;

    [Header("Pathing")]
    public bool pathFound;
    public SheepPathingType pathingType;
    public SheepPathingType oldPathingType;
    public List<Vector2> travelPath;
    public LayerMask obstacleMask;
    public bool returnedToOldPos;
    public GameObject PatrolPlot;
    public bool seesPlayer;
    public bool escapeSequence = false;
    public bool lastChaseBegun = false;
    public float giveUpTimer;

    [Header("Behaviours")]
    public bool movingToNextTile;
    public bool fleeing;
    public float howlTimer;
    public Vector2 sweaterPos;
    public bool goldenSheep = false;

    // private data members
    Transform slot;
    Vector2 pos;
    public Vector2 nextPos;
    Vector2 oldPos;
    Vector3 oldScale;
    Quaternion oldRot;
    Transform detectionComponent;
    public float current_velocity;


    Wolf wolf;

    //Debug stuff
    bool isVisibleInLog = false;

    // Patrol AI used for single sheep patrolling
    
    private bool forward = true;
    private List<Transform> patrol_nodes = new List<Transform>();
    private int target_node_index = 0;

    //Animation
    public bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        current_velocity = movementSpeed;
        pathFound = false;
        movingToNextTile = false;
        travelPath = new List<Vector2>();
        oldPathingType = pathingType;
        nextPos = transform.position;
        oldPos = nextPos;
        returnedToOldPos = true;
        sheep = GetComponent<Sheep>();
        pos = Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.y));
        
        wolf = GameObject.Find("Wolf").GetComponent<Wolf>();
        howlTimer = 0;
        detectionComponent = transform.GetChild(3);
        // AI has a patrol spot

        if (PatrolPlot != null)
        {
            pathingType = SheepPathingType.Patrolling;
            // Debug.Log("I am " + gameObject.name + " and I am initializing my patrol route");
            foreach (Transform child in PatrolPlot.transform)
            {
                if (child.tag == "PatrolNode")
                {
                    patrol_nodes.Add(child);
                }
            }
            target_node_index = 0;
        }
        oldPathingType = pathingType;

        if (goldenSheep)
        {
            movementSpeed *= 1.5f;
            current_velocity = movementSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pathingType != SheepPathingType.Fleeing && escapeSequence && PositionToWorldVector2Int((Vector2)transform.position).x >= RoomManager.MAINAREA_INNER_X)
        {
            SetOldPos();
            pathingType = SheepPathingType.ToPlayer;
        }

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
                isVisibleInLog = true;
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
        isMoving = true;

        // if already at spot, dont move
        if ((nextPos - (Vector2)transform.position).magnitude < arrivalDistance)
        {
            if (isVisibleInLog)
            {
                Debug.Log("Standing Still");
            }
            isMoving = false;
            return;
        }

        // if no path, generate path
        if (travelPath.Count == 0)
        {
            if (isVisibleInLog)
            {
                Debug.Log("Need to generate a path!");
            }
            travelPath = GeneratePath(nextPos);
        }

        if(travelPath.Count > 0)
        {
            if ((travelPath[0] - (Vector2)transform.position).magnitude < arrivalDistance)
            {
                if (isVisibleInLog)
                {
                    Debug.Log("Arrived to node");
                }
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
            if (!goldenSheep)
            {
                nextPos = ((Vector2)transform.position - wolf.GetWolfPos()) + (Vector2)transform.position;
            }
            else
            {
                nextPos = wolf.GetWolfPos();
            }
        }
        else
        {
            current_velocity = movementSpeed;
            pathingType = SheepPathingType.ToPlayer;
            travelPath.Clear();
        }
    }


    void ReturnToSpot()
    {
        nextPos = oldPos;
        if (((Vector2)transform.position - oldPos).magnitude < arrivalDistance)
        {
            pathingType = oldPathingType;
            returnedToOldPos = true;
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
        if (!seesPlayer && !escapeSequence) 
        {
            if (giveUpTimer <= 0)
            {
                // Debug.Log("Giving up");
                pathingType = SheepPathingType.Returning;
                travelPath.Clear();
            }
            else
            {
                // Debug.Log(giveUpTimer);
                giveUpTimer -= 1.0f * Time.deltaTime;
                travelPath.Clear();
            }
        }
        else if (!seesPlayer && escapeSequence)
        {
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
            if(patrol_nodes == null || patrol_nodes.Count == 0)
            {
                throw new System.Exception(gameObject.name + "Sheep is set to patrol but has nothing to patrol to");
            }
            // when you reach a node, go to next node. If you're at the edge of a patrol path, change direction
            if ((patrol_nodes[target_node_index].position - transform.position).magnitude <= arrivalDistance)
            {
                if (target_node_index == patrol_nodes.Count - 1 && forward == true)
                {
                    forward = false;
                }
                else if (target_node_index == 0 && forward == false)
                {
                    forward = true;
                }

                if (forward)
                {
                    target_node_index++;
                }
                else
                {
                    target_node_index--;
                }
            }
            nextPos = patrol_nodes[target_node_index].position;
        }
        else
        {
            nextPos = slot.position;
        }
    }

    void Arrive()
    {
        if(travelPath.Count == 0) { return; }
        Vector2 direction = travelPath[0] - (Vector2)transform.position;
        
        // if obstacle, then recalculate
        /*RaycastHit2D hit = Physics2D.Raycast(transform.position, travelPath[0], direction.magnitude, obstacleMask);
        if (hit)
        {
            // arrive // DEBUG_OBJECT.transform.position = hit.point;
            travelPath.Clear();
            return;
        }*/

        if(direction.magnitude < arrivalDistance)
        {
            isMoving = false;
            return;
        }
        isMoving = true;
        float angle = Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI - 90;
        transform.position = (Vector2)transform.position + current_velocity * direction.normalized * Time.deltaTime;
        detectionComponent.rotation = Quaternion.RotateTowards(detectionComponent.rotation, Quaternion.Euler(0,0,angle), angularVelocity * Time.deltaTime);
    }

    List<Vector2> GeneratePath(Vector2 destination)
    {
        List<Vector2> path = new List<Vector2>();
        Vector2 direction = destination - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude, obstacleMask);
        if(hit && !escapeSequence)
        {
            if (isVisibleInLog)
            {
                Debug.Log("Sheep raycasted this: " + hit.transform.tag);
            }

            pos = transform.position;
            Vector2Int posV2I = PositionToWorldVector2Int(pos);
            List<Vector2Int> pathInt = Pathing.AStar(posV2I, PositionToWorldVector2Int(destination));
            if (pathInt == null)
            {
                path.Add(destination);
            }
            else
            {
                if (isVisibleInLog)
                {
                    Debug.Log("Current: " + posV2I.x + "," + posV2I.y);
                }
                int counter = 0;
                foreach (Vector2Int node in pathInt)
                {
                    path.Add(GridManager.Instance.walkableTilemap.CellToWorld(new Vector3Int(node.x, node.y, 0)));
                    if (isVisibleInLog)
                    {
                        Debug.Log("Path[ " + counter + "]: " + node.x + "," + node.y);
                    }
                    counter++;
                }
            }

            //FIXIT go through the list of nodes in path and see if we can skip any (if you can raycast to 3 without a hit, you don't need to include 0, 1, or 2)
        }
        else if (hit && escapeSequence)
        {
            pos = transform.position;
            Vector2Int posV2I = PositionToWorldVector2Int(pos);
            Vector2Int desV2I = PositionToWorldVector2Int(destination);

            if (desV2I.y > RoomManager.EXIT_Y && posV2I.y > RoomManager.FINAL_ROOM_STAY_Y)
            {
                path.Add(destination);
                /*if (RoomManager.InExitRoom(desV2I))
                {
                    path.Add((Vector2)(GridManager.Instance.walkableTilemap.CellToWorld(new Vector3Int(RoomManager.roomManager.voiceRoom.getExit().x,
                        RoomManager.roomManager.voiceRoom.getExit().y, 0)) - transform.position));
                }
                else
                {
                    path.Add(destination);
                }*/
            }
            else if (desV2I.y > RoomManager.FINAL_ROOM_START_Y && posV2I.y <= RoomManager.FINAL_ROOM_STAY_Y && !lastChaseBegun)
            {
                path.Clear();
            }
            else
            {
                if (posV2I.y > RoomManager.EXIT_Y)
                {
                    if (posV2I.x < RoomManager.EXIT_LEFTX)
                    {
                        path.Add((Vector2)(GridManager.Instance.walkableTilemap.CellToWorld(new Vector3Int(RoomManager.EXIT_LEFTX, RoomManager.EXIT_Y - 1, 0)) - transform.position));
                    }
                    else if (posV2I.x > RoomManager.EXIT_RIGHTX)
                    {
                        path.Add((Vector2)(GridManager.Instance.walkableTilemap.CellToWorld(new Vector3Int(RoomManager.EXIT_RIGHTX, RoomManager.EXIT_Y - 1, 0)) - transform.position));
                    }
                    else
                    {
                        path.Add((Vector2)(GridManager.Instance.walkableTilemap.CellToWorld(new Vector3Int(RoomManager.MAINAREA_OUTER_X, RoomManager.MAINAREA_LOWERTUNNEL_UPPERY, 0)) - transform.position));
                    }
                }
                else
                {
                    if (desV2I.y <= RoomManager.FINAL_ROOM_START_Y)
                    {
                        lastChaseBegun = true;
                    }
                    path.Add(destination);
                }
            }
        }
        else
        {
            //Debug.Log("No raycast");
            path.Add(destination);
            // DEBUG_OBJECT.transform.position = destination;
        }

        if (isVisibleInLog)
        {
            Debug.Log("Generated path size is " + path.Count);
        }
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

    public void SetOldPos()
    {
        if (returnedToOldPos)
        {
            oldPos = transform.position;
            returnedToOldPos = false;
        }
    }

    public void IsNowFleeing()
    {
        if (!goldenSheep)
        {
            SetOldPos();
            pathingType = SheepPathingType.Fleeing;
            travelPath.Clear();
            current_velocity = movementSpeed * 2;
            howlTimer = HOWL_DURATION;
        }
        else
        {
            pathingType = SheepPathingType.Fleeing;
            current_velocity = movementSpeed * 0.7f;
            howlTimer = HOWL_DURATION;
        }
    }
    
    //Quick functions to reduce rewriting
    public Vector2Int PositionToWorldVector2Int(Vector2 position) { return (Vector2Int)GridManager.Instance.walkableTilemap.WorldToCell(new Vector3(position.x, position.y, 0)); }
    Vector2Int GetSheepPos() { return (Vector2Int)GridManager.Instance.walkableTilemap.WorldToCell(new Vector3(transform.position.x, transform.position.y, 0)); }
}
