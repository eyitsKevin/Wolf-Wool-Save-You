﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    public static Wolf player;

    [SerializeField] float playerSpeed = 3;
    [SerializeField] GameObject sweater;
    public Vector2 targetPosition;
    public bool woolHeld;
    public bool escaped;
    public bool howl;
    public float howlCooldown;

    Animator mAnimator;
    bool isMoving;

    void Start()
    {
        if (player == null)
        {
            player = this;
        }

        woolHeld = false;
        escaped = true;
        howl = false;
        howlCooldown = 0;
        targetPosition = new Vector2Int(0, 0);
        mAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        isMoving = false;

        //Basic WASD movement
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.Translate(-Vector2.right * playerSpeed * Time.deltaTime);
            transform.localScale = new Vector3(-2, 2, 2);
            isMoving = true;
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.Translate(Vector2.right * playerSpeed * Time.deltaTime);
            transform.localScale = new Vector3(2, 2, 2);
            isMoving = true;
        }
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            transform.Translate(-Vector2.up * playerSpeed * Time.deltaTime);
            isMoving = true;
        }
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            transform.Translate(Vector2.up * playerSpeed * Time.deltaTime);
            isMoving = true;
        }        
        
        // Left-click to shear sheep, only succeeds when sufficiently close to it
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D mouseHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (mouseHit.collider != null)
            {
                if (mouseHit.collider.tag == "Unsheared")
                {
                    if ((transform.position - mouseHit.transform.position).magnitude < 3)
                    {
                        mouseHit.collider.tag = "Sheared";
                        woolHeld = true;
                    }
                }
            }
        }

        // Right-click to throw sweater to the indicated position
        if (woolHeld)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Instantiate(sweater, this.transform.position, Quaternion.identity);
                targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                woolHeld = false;
            }
        }

        // Press space to trigger howl if acquired and howl cooldown reaches 0
        if (howl)
        {
            if (howlCooldown <= 0)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    AudioSource howlClip = GetComponent<AudioSource>();
                    howlClip.Play();
                    howlCooldown = 10;
                }
            }
            else
            {
                howlCooldown -= Time.deltaTime;

                if (howlCooldown >= 9)
                {
                    GameObject[] unshearedSheep = GameObject.FindGameObjectsWithTag("Unsheared");
                    GameObject[] shearedSheep = GameObject.FindGameObjectsWithTag("Sheared");
                    GameObject[] clothedSheep = GameObject.FindGameObjectsWithTag("Clothed");

                    foreach (GameObject sheep in unshearedSheep)
                    {
                        if ((sheep.transform.position - this.transform.position).magnitude < 10)
                        {
                            sheep.GetComponent<SheepBehavior>().pathingType = SheepBehavior.SheepPathingType.Fleeing;
                            sheep.GetComponent<SheepBehavior>().howlTimer = 3;
                        }
                    }
                    foreach (GameObject sheep in shearedSheep)
                    {
                        if ((sheep.transform.position - this.transform.position).magnitude < 10)
                        {
                            sheep.GetComponent<SheepBehavior>().pathingType = SheepBehavior.SheepPathingType.Fleeing;
                            sheep.GetComponent<SheepBehavior>().howlTimer = 3;
                        }
                    }
                    foreach (GameObject sheep in clothedSheep)
                    {
                        if ((sheep.transform.position - this.transform.position).magnitude < 10)
                        {
                            sheep.GetComponent<SheepBehavior>().pathingType = SheepBehavior.SheepPathingType.Fleeing;
                            sheep.GetComponent<SheepBehavior>().howlTimer = 3;
                        }
                    }

                }
            }
        }

        mAnimator.SetBool("moving", isMoving);
    }

    public static Vector2Int GetWolfPos() { return (Vector2Int)GridManager.Instance.walkableTilemap.WorldToCell(new Vector3(player.transform.position.x, player.transform.position.y, 0)); }
}
