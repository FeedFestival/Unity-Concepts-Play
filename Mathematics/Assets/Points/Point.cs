using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Point : MonoBehaviour
{
    public TextMeshPro Text;
    public float X;
    public float Y;
    public float Z;

    public void SetPos(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
        transform.position = new Vector3(x, y, z);
        string pos = "(" + x + ", " + y + ", " + z + ")";
        gameObject.name = "p" + pos;
        Text.text = pos;

        var cameraPosition = Camera.main.transform.position;
        Vector3 dir = Text.transform.position - cameraPosition;
        Text.transform.rotation = Quaternion.LookRotation(dir);

        //var distanceToCamera = Vector3.Distance(transform.position, cameraPosition);
        //print(distanceToCamera);
    }
}
