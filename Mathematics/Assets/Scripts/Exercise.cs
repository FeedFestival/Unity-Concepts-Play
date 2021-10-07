using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exercise : MonoBehaviour
{

    void Start()
    {
        DoExercise();
    }

    void DoExercise()
    {
        Point p = Grid._.GetPoint();
        p.SetPos(1, 1, 1);
    }
}
