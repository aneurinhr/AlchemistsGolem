using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public float sprintSpeed = 20.0f;
    public GameObject cameraCenter;
    public bool pauseMovement = false;

    public NavMeshAgent agent;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (pauseMovement == false)
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
        }

        //Debug.Log(agent.isOnOffMeshLink);
        if (agent.isOnOffMeshLink == true)
        {
            agent.CompleteOffMeshLink();
        }
    }
}
