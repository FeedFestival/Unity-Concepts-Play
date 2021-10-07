using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;

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

        Camera.main.transform
            .ObserveEveryValueChanged(t => t.position)
            .Subscribe(AlignTextToView);

        var camPos = Camera.main.transform.position;
        AlignTextToView(camPos);
    }

    void AlignTextToView(Vector3 camPos)
    {
        Vector3 dir = Text.transform.position - camPos;
        Text.transform.rotation = Quaternion.LookRotation(dir);
    }
}
