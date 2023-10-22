using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    const int MOVEMENT_SPEED = 2;

    private Rigidbody rb;
    private enum PlayerMode 
    {
        Free,
        LockedOnTarget
    }

    PlayerMode playerMode = PlayerMode.Free;

    private GameObject lockedOnTarget = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovement();

        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            List<Collider> colliders = Physics.OverlapSphere(transform.position, 10)
                .ToList()
                .Where(x => x.gameObject.tag != "Player")
                .ToList();

            // TODO: Don't choose the closest object, but instead the first object that is in the line of sight of the player
            lockedOnTarget = GetClosestGameObjectToTarget(colliders, transform.position);

            if (lockedOnTarget != null) 
            {
                playerMode = PlayerMode.LockedOnTarget;
            }       
        }
        if (Input.GetKeyUp(KeyCode.Tab)) 
        {
            lockedOnTarget = null;

            playerMode = PlayerMode.Free;
        }

        if (Input.GetKey(KeyCode.Tab))
        {            
            if (lockedOnTarget != null)
            {
                Vector3 directionToTarget = lockedOnTarget.transform.position - transform.position;
                directionToTarget.y = 0;
                Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);

                transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, 10 * Time.deltaTime);
            }
        }
    }

    private GameObject GetClosestGameObjectToTarget(List<Collider> colliders, Vector3 target)
    {
        colliders.Sort((a, b) =>
        {
            float distanceA = Vector3.Distance(a.gameObject.transform.position, target);
            float distanceB = Vector3.Distance(b.gameObject.transform.position, target);
            return distanceA.CompareTo(distanceB);
        });
        return colliders.FirstOrDefault()?.gameObject;
    }

    private List<Vector3> inputHistory = new List<Vector3>();

    private void UpdateMovement()
    {
        Vector3 input = GetInput();

        // Collect the last x inputs
        // Check if all inputs are the same - if they are, only then process the input?
        inputHistory.Add(input);
        if (inputHistory.Count > 5)
        {
            inputHistory.RemoveAt(0);
        }

        if (input != Vector3.zero) 
        {
            // take the average of the last couple of inputs
            var averageTargetPosition = CalculateAverageVector(inputHistory);

            // Define and set rotation
            Quaternion targetRotation = Quaternion.LookRotation(averageTargetPosition);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);

            // Define and set position
            var target = transform.position + averageTargetPosition * MOVEMENT_SPEED * Time.deltaTime;
            rb.MovePosition(target);
        }
    }

    private Vector3 GetInput()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // Normalize input to prevent diagonal values (only for non-controller inputs)
        if (Math.Abs(input.x) == 1 || Math.Abs(input.z) == 1)
        {
            input.Normalize();
        }

        input = ApplyInputThreshold(input);
        input = ToIso(input);
        return input;
    }

    private Vector3 ApplyInputThreshold(Vector3 input)
    {
        const float INPUT_THRESHOLD = 0.2f;

        // Only process input that cross a threshold
        // This minimized flicker and erratic movements, as registered inputs are larger
        // Whenever the input exceeds the threshold, the input needs to be mapped between 0 and 1
        // This is done by normalizing in the new range
        if (input.magnitude < INPUT_THRESHOLD) return Vector3.zero;

        // Remap the magnitude from the range [threshold, 1] to [0, 1]
        float mappedMagnitude = Mathf.InverseLerp(INPUT_THRESHOLD, 1f, input.magnitude);
        // Return normalized vector with the new magnitude
        return mappedMagnitude * input.normalized;
    }

    private Vector3 ToIso(Vector3 input) 
    {
        return Quaternion.Euler(0, 45f, 0) * input;
    }

    private Vector3 CalculateAverageVector(List<Vector3> input)
    {
        return new Vector3(
            input.Average(vector => vector.x),
            input.Average(vector => vector.y),
            input.Average(vector => vector.z)
        );
    }
}
