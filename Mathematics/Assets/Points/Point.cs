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

        Vector3 dir = Text.transform.position - Camera.main.transform.position;

        //Text.transform.LookAt(Text.transform, dir);
        Text.transform.rotation = Quaternion.LookRotation(dir);
    }
}
