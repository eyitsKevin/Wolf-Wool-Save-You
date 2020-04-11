using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepBehavior : MonoBehaviour
{
    public enum SheepPathingType
    {
        Stationary,
        Patrolling,
        ToSweater,
        ToPlayer,
        Fleeing
    }
  
    bool pathFound;
    SheepPathingType pathingType;

    // Start is called before the first frame update
    void Start()
    {
        pathFound = false;
        pathingType = SheepPathingType.Patrolling;
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
                break;

            case SheepPathingType.ToSweater:
                //check if a path has already been made and follow that path if so, otherwise make a path to the sweater first
                break;

            case SheepPathingType.ToPlayer:
                //if a path has already been made, and the player is in a different room than the sheep and the player is in the same room as when the path was made, go along that path
                //otherwise, make a new path first
                break;

            case SheepPathingType.Fleeing:
                //flee away from source?
                break;
        }
    }
}
