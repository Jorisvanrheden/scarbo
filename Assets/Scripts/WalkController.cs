using System.Collections.Generic;
using UnityEngine;

public class WalkController : MonoBehaviour
{
    private Rigidbody rb;

    private List<Vector3> waypoints;
    private int waypointIndex = 0;
    private const int WAYPOINT_DISTANCE = 2;

    private enum State 
    {
        Walking,
        Stopping
    }
    private State state = State.Stopping;
    private const float STOP_DURATION = 1f; // seconds;
    private float currentStopDuration = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        waypoints = new List<Vector3>() 
        {
            new Vector3(transform.position.x, transform.position.y, transform.position.z - WAYPOINT_DISTANCE),
            new Vector3(transform.position.x - WAYPOINT_DISTANCE, transform.position.y, transform.position.z - WAYPOINT_DISTANCE),
            new Vector3(transform.position.x - WAYPOINT_DISTANCE, transform.position.y, transform.position.z),
            transform.position,
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state) 
        {
            case State.Walking:
                if (Vector3.Distance(waypoints[waypointIndex], transform.position) < 0.1f)
                {
                    waypointIndex++;
                    if (waypointIndex >= waypoints.Count) waypointIndex = 0;

                    state = State.Stopping;
                    break;
                }

                Vector3 directionToTarget = waypoints[waypointIndex] - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);

                // set position
                rb.MovePosition(transform.position + transform.forward * 0.5f * Time.deltaTime);
                break;
            case State.Stopping:
                currentStopDuration += Time.deltaTime;
                if (currentStopDuration >= STOP_DURATION) 
                {
                    currentStopDuration = 0;
                    state = State.Walking;
                }
                break;
        }      
    }
}
