using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 ExcludingYComponent(this Vector3 vector3)
    {
        return new Vector3(vector3.x, 0, vector3.z);
    }
}
