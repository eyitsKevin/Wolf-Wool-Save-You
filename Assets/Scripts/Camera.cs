﻿using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour
{
    //Player tracking
    public GameObject player;
    [Range(0.01f, 1.0f)] public float smoothSpeed = 0.5f;

    //Point of Interest tracking
    private Vector3 POI;
    private float lingerDuration;

    //General settings
    public Vector3 offset;
    public float distanceMargin;
    [Range(0.01f, 10.0f)] public float panningSpeed = 1f;

    private bool isTrackingPlayer = true;
    private bool returningToPlayer = false;

    void FixedUpdate()
    {
        if (isTrackingPlayer && !returningToPlayer) trackPlayer();

        else if (!isTrackingPlayer && returningToPlayer) returnToPlayer();

        else if (!isTrackingPlayer && !returningToPlayer) trackPOI();
    }

    public void goTo(Vector3 _POI, float _lingerDuration)
    {
        POI = _POI;
        lingerDuration = _lingerDuration;
        isTrackingPlayer = false;
    }

    private void trackPlayer()
    {
        Vector3 desiredPosition = player.transform.position + offset;
        Vector3 smoothedPosition = Vector3.Slerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    private void trackPOI()
    {
        float dist = Vector3.Distance(transform.position, POI + offset);

        if (dist < distanceMargin)
        {
            StartCoroutine(lingerWait());
        }
        else
        {
            Vector3 desiredPosition = POI + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * panningSpeed);
            transform.position = smoothedPosition;
        }
    }

    private void returnToPlayer()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position + offset);

        if (dist < distanceMargin)
        {
            isTrackingPlayer = true;
            returningToPlayer = false;
        }
        else
        {
            Vector3 desiredPosition = player.transform.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * panningSpeed);
            transform.position = smoothedPosition;

        }
    }

    IEnumerator lingerWait()
    {
        yield return new WaitForSeconds(lingerDuration);
        returningToPlayer = true;
    }
}
