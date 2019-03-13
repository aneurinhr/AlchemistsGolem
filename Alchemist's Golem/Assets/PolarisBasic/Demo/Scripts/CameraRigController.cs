using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.PolarisBasic.Demo
{
	public class CameraRigController : MonoBehaviour
	{
        [SerializeField]
        private Vector3 rotateSpeed;
        public Vector3 RotateSpeed
        {
            get
            {
                return rotateSpeed;
            }
            set
            {
                rotateSpeed = value;
            }
        }

        private void Update()
        {
            transform.Rotate(rotateSpeed * Time.deltaTime, Space.Self);
        }
    }
}
