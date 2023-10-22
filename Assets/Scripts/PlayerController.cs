using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    const int MOVEMENT_SPEED = 2;
    const int FOCUS_RANGE = 5;

    private Rigidbody rb;

    private KeyCode KeyCodeFocus = KeyCode.Joystick1Button0;

    private GameObject lockedOnTarget = null;

    // Stored properties, as working with actual properties might feel sluggish
    private Vector3 targetDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCodeFocus))
        {
            List<Collider> colliders = Physics.OverlapSphere(transform.position, FOCUS_RANGE)
                .ToList()
                .Where(x => x.gameObject.tag != "Player" && x.gameObject.tag != "Terrain")
                .ToList();

            var objectsFacingPlayer = FilterOnFacing(colliders, transform.position, transform.forward);
            // if there are objects in front of the player, only process those
            if (objectsFacingPlayer.Count > 0) 
            {
                colliders = objectsFacingPlayer;
            }
            colliders = SortOnDistance(colliders, transform.position);

            lockedOnTarget = colliders.FirstOrDefault()?.gameObject;             
        }
        if (Input.GetKeyUp(KeyCodeFocus))
        {
            lockedOnTarget = null;
        }

        if (Input.GetKey(KeyCodeFocus))
        {
            if (lockedOnTarget != null)
            {
                if (Vector3.Distance(transform.position, lockedOnTarget.transform.position) > FOCUS_RANGE) 
                {
                    lockedOnTarget = null;
                    return;
                }

                Vector3 directionToTarget = lockedOnTarget.transform.position - transform.position;
                directionToTarget.y = 0;
                Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);

                transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, 10 * Time.deltaTime);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovement();
    }

    private List<Collider> SortOnDistance(List<Collider> colliders, Vector3 target)
    {
        colliders.Sort((a, b) =>
        {
            float distanceA = Vector3.Distance(a.gameObject.transform.position, target);
            float distanceB = Vector3.Distance(b.gameObject.transform.position, target);
            return distanceA.CompareTo(distanceB);
        });
        return colliders;
    }

    private List<Collider> FilterOnFacing(List<Collider> colliders, Vector3 target, Vector3 forward)
    {
        return colliders.Where(x =>
            Vector3.Dot((x.gameObject.transform.position - target).normalized, forward) > 0.2f
        ).ToList();
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

            // set rotation         
            if (lockedOnTarget != null)
            {
                Vector3 directionToTarget = lockedOnTarget.transform.position - transform.position;
                directionToTarget.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
            }
            else 
            {
                Quaternion targetRotation = Quaternion.LookRotation(averageTargetPosition);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
            }

            // set position
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
