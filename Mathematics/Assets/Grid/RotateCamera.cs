using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public Camera CameraObj;
    public float Speed = 2f;

    void Update()
    {
        RotateMainCamera();
    }

    void RotateMainCamera()
    {
        if (Input.GetMouseButton(0))
        {
            CameraObj.transform.RotateAround(Grid._.transform.position,
                                            CameraObj.transform.up,
                                            -Input.GetAxis("Mouse X") * Speed);

            CameraObj.transform.RotateAround(Grid._.transform.position,
                                            CameraObj.transform.right,
                                            -Input.GetAxis("Mouse Y") * Speed);
        }
    }
}
