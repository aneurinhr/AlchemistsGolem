using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBillboard : MonoBehaviour
{
    public Camera camera = null;

    private void Update()
    {
        if (camera != null)
        {
            transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
        }
        else
        {
            GameObject temp = GameObject.FindGameObjectWithTag("PlayerCamera");
            camera = temp.GetComponent<Camera>();
        }
    }
}
