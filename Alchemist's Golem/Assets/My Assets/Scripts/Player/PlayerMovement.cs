﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public float sprintSpeed = 20.0f;

    public float floatHeight = 2.0f;
    public float floatStrenght = 30.0f;
    public float overShootDrag = 5.0f;

    public GameObject cameraCenter;

    private Rigidbody rb;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float x = 0;
        float z = 0;

        if (Input.GetButton("Sprint"))
        {
            x = Input.GetAxis("Horizontal") * Time.deltaTime * sprintSpeed;
            z = Input.GetAxis("Vertical") * Time.deltaTime * sprintSpeed;
        }
        else
        {
            x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
            z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        }

        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        transform.Translate(x, 0, z);
        transform.Rotate(0, horizontal, 0);

        float nextRotation = cameraCenter.gameObject.transform.eulerAngles.x + vertical;
        if (((nextRotation <= 80) && (nextRotation >= -80)) || ((nextRotation <= 440) && (nextRotation >= 280)))
        {
            cameraCenter.gameObject.transform.Rotate(vertical, 0, 0);
        }

        Floating();
    }

    void Floating()
    {
        RaycastHit hit;
        Vector3 down = transform.TransformDirection(Vector3.down);

        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.blue);
            //Debug.Log(hit.distance + " => " + hit.collider.name);
        }

        float heightDif = floatHeight - hit.distance;

        if ((rb.velocity.y > 0) && (hit.distance > floatHeight))//checks if overshooting
        {
            rb.drag = overShootDrag;
        }
        else
        {
            rb.drag = 0;
        }

        if (heightDif >= 0)
        {
            rb.AddForce(0, heightDif*floatStrenght, 0);
        }
    }
}