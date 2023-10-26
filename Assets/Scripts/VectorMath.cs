using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VectorMath
{
    public static Vector3 CalculateAverageVector(List<Vector3> input)
    {
        return new Vector3(
            input.Average(vector => vector.x),
            input.Average(vector => vector.y),
            input.Average(vector => vector.z)
        );
    }

    public static Vector3 ToIso(Vector3 input)
    {
        return Quaternion.Euler(0, 45f, 0) * input;
    }
}
