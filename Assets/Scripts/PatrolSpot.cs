using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolSpot : MonoBehaviour
{
    public bool shouldWait;

    public Vector3 GetPosition { get { return transform.position; } }
}
