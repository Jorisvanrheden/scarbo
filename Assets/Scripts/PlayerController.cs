using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /**
     * Constants
     */
    const int MOVEMENT_SPEED = 2;
    const int FOCUS_RANGE = 60;

    /**
     * References
     */
    private Rigidbody rb;

    /**
     * Local variables
     */
    private GameObject lockedOnTarget = null;
    private GameObject lockedOnIndicator = null;

    private Quaternion targetQuaternion;


    /** 
     * Move this code to a different module, or at least not have the player be the placeholder for all these 
     * properties. They don't belong here, but for now it's okay
     */
    public GameObject uiTargetIndicator;

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
        var targetLock = GameButton.GetStatus(GameButton.Command.TargetLock);

        if (targetLock == GameButton.Status.Pressed)
        {
            List<Collider> colliders = Physics.OverlapSphere(transform.position, FOCUS_RANGE)
                .Where(x => x.gameObject.tag == "Enemy")
                .ToList();

            var objectsFacingPlayer = FilterOnFacing(colliders, transform.position, this.targetQuaternion * Vector3.forward);
            // if there are objects in front of the player, only process those
            if (objectsFacingPlayer.Count > 0)
            {
                colliders = objectsFacingPlayer;
            }
            colliders = SortOnDistance(colliders, transform.position);

            var target = colliders.FirstOrDefault()?.gameObject;
            if (target != null) 
            {
                SelectLockedOnTarget(target);
            }       
        }

        if (targetLock == GameButton.Status.Released)
        {
            DeselectLockedOnTarget();
        }

        if (targetLock == GameButton.Status.Down)
        {
            if (lockedOnTarget != null)
            {
                if (Vector3.Distance(transform.position, lockedOnTarget.transform.position) > FOCUS_RANGE)
                {
                    DeselectLockedOnTarget();
                    return;
                }

                Vector3 directionToTarget = lockedOnTarget.transform.position - transform.position;
                directionToTarget.y = 0;

                SetRotation(Quaternion.LookRotation(directionToTarget));
            }
        }

        if (lockedOnTarget != null) 
        {
            // find target closest in the direction of the right thumb stick
            Vector3 input = new Vector3(Input.GetAxisRaw("Right Thumb Horizontal"), 0, Input.GetAxisRaw("Right Thumb Vertical"));
            if (input != Vector3.zero)
            {
                List<Collider> colliders = Physics.OverlapSphere(transform.position, FOCUS_RANGE)
                .Where(x => x.gameObject.tag == "Enemy")
                .ToList();

                var target = FilterOnFacing(colliders, transform.position, input)
                            .FirstOrDefault()?.gameObject;

                if (target != null)
                {
                    SelectLockedOnTarget(target);
                }
            }
        }
    }

    private void SelectLockedOnTarget(GameObject gameObject) 
    {
        if (lockedOnTarget != null) 
        {
            DeselectLockedOnTarget();
        }

        lockedOnTarget = gameObject;

        lockedOnIndicator = Instantiate(uiTargetIndicator, lockedOnTarget.transform);
    }
    private void DeselectLockedOnTarget()
    {
        lockedOnTarget = null;
        if (lockedOnIndicator != null) 
        {
            Destroy(lockedOnIndicator);
            lockedOnIndicator = null;
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
            Vector3.Dot((x.gameObject.transform.position - target).normalized, forward) > 0.7f
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
            var averageTargetPosition = VectorMath.CalculateAverageVector(inputHistory);

            // set rotation         
            if (lockedOnTarget != null)
            {
                Vector3 directionToTarget = lockedOnTarget.transform.position - transform.position;
                directionToTarget.y = 0;

                SetRotation(Quaternion.LookRotation(directionToTarget));
            }
            else 
            {
                SetRotation(Quaternion.LookRotation(averageTargetPosition));
            }

            // set position
            var target = transform.position + averageTargetPosition * MOVEMENT_SPEED * Time.deltaTime;
            rb.MovePosition(target);
        }
    }

    private void SetRotation(Quaternion targetQuaternion) 
    {
        this.targetQuaternion = targetQuaternion; 

        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, 10 * Time.deltaTime);
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
        input = VectorMath.ToIso(input);
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
}
