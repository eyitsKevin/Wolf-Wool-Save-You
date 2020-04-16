using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepAnimation : MonoBehaviour
{
    Animator mAnimator;
    public bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isMoving = transform.parent.GetComponent<SheepBehavior>().isMoving;
        mAnimator.SetBool("moving", isMoving);
        //Debug.Log(mAnimator.GetBool(isMoving));
    }
}
