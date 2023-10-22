using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    public GameObject target;

    private const float DISTANCE_TO_TARGET = 10f;

    private const int FOLLOW_SPEED = 4;

    private void Start()
    {
        // On startup, set camera position to already match the target
        transform.position = GetTargetPosition();
    }

    private void Update()
    {
        transform.position = Vector3.Slerp(transform.position, GetTargetPosition(), FOLLOW_SPEED * Time.deltaTime);
    }

    private Vector3 GetTargetPosition() 
    {
        return target.transform.position - transform.forward * DISTANCE_TO_TARGET;
    }
}
