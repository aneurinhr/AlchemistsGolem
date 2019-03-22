using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloat : MonoBehaviour
{
    public float floatHeight = 2.0f;
    public float floatStrenght = 30.0f;
    public float overShootDrag = 5.0f;
    public Rigidbody rb;

    private void Update()
    {
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

        if (hit.collider.isTrigger == false)
        {
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
                rb.AddForce(0, heightDif * floatStrenght, 0);
            }
        }
    }
}
