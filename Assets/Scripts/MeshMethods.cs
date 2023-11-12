using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshMethods
{
    public static float GetMeshTopYPosition(Transform transform)
    {
        Ray ray = new Ray(transform.position + Vector3.up * 20f, Vector3.down);
        RaycastHit hit;

        // Check if the ray hits any collider
        if (Physics.Raycast(ray, out hit))
        {
            // The hit.point will give you the position where the ray hits the collider
            return hit.point.y;
        }
        else return transform.position.y;
    }
}
