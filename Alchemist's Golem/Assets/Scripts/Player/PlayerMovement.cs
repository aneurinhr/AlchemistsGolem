using System.Collections;
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

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

        transform.Translate(x, 0, z);
        transform.Rotate(0, horizontal, 0);

        Floating();
    }

    void Floating()
    {
        RaycastHit hit;
        Vector3 down = transform.TransformDirection(Vector3.down);

        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red);
            Debug.Log(hit.distance + " => " + hit.collider.name);
        }

        float heightDif = floatHeight - hit.distance;
        float moveUp = heightDif * Time.deltaTime;

        if ((gameObject.GetComponent<Rigidbody>().velocity.y > 0) && (hit.distance > floatHeight))
        {
            gameObject.GetComponent<Rigidbody>().drag = overShootDrag;
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().drag = 0;
        }

        if (heightDif >= 0)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(0, floatStrenght, 0);
        }
    }
}
