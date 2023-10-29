using System.Collections.Generic;
using UnityEngine;

public class WalkController : MonoBehaviour
{
    private Rigidbody rb;

    private List<Vector3> waypoints;
    private int waypointIndex = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        waypoints = new List<Vector3>() 
        {
            transform.position,
            new Vector3(transform.position.x, transform.position.y, transform.position.z + 3),
            new Vector3(transform.position.x - 3, transform.position.y, transform.position.z + 3),
            new Vector3(transform.position.x - 3, transform.position.y, transform.position.z)
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(waypoints[waypointIndex], transform.position) < 0.1f) 
        {
            waypointIndex++;
            if (waypointIndex >= waypoints.Count) waypointIndex = 0;
        }

        Vector3 directionToTarget = waypoints[waypointIndex] - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);

        // set position
        rb.MovePosition(transform.position + transform.forward * 0.5f * Time.deltaTime);
    }
}
