﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonChest : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenChest()
    {
        animator.SetTrigger("Opening");
    }
}
