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
        Point p1 = Grid._.GetPoint();
        p1.SetPos(1, 1, 1);
        Grid._.ShowGridLine(p1);

        Point p2 = Grid._.GetPoint();
        p2.SetPos(0, 0, -1);
        Grid._.ShowGridLine(p2);

        Grid._.ConnectPoints(p1, p2);
    }
}
