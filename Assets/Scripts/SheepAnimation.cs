using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepAnimation : MonoBehaviour
{
    Animator mAnimator;
    public bool isMoving;
    Vector3 fovAngle;

    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Set facing direction based on FoV angle; default orientation is facing left
        fovAngle = transform.parent.GetChild(3).eulerAngles;

        if (fovAngle.z >= 0 && fovAngle.z <= 180)
        {
            if (transform.parent.tag == "Golden")
            {
                transform.localScale = new Vector2(2, 1.5f);
            }
            else
            {
                transform.localScale = new Vector2(1, 1);
            }
        }
        else
        {
            if (transform.parent.tag == "Golden")
            {
                transform.localScale = new Vector2(-2, 1.5f);
            }
            else
            {
                transform.localScale = new Vector2(-1, 1);
            }
        }

        isMoving = transform.parent.GetComponent<SheepBehavior>().isMoving;
        mAnimator.SetBool("moving", isMoving);
    }
}
