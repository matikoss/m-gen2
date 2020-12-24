﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public float zoomSpeed;

    public float minZoomDist;
    public float maxZoomDist;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Move();
        Zoom();
    }

    void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        Vector3 dir = transform.up * yInput + transform.right * xInput;

        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    void Zoom()
    {
        /*float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float dist = Vector3.Distance(transform.position, cam.transform.position);

        if (dist < minZoomDist && scrollInput > 0.0f)
            return;
        else if (dist > maxZoomDist && scrollInput < 0.0f)
            return;

        cam.transform.position += cam.transform.forward * scrollInput * zoomSpeed;*/
    }
}