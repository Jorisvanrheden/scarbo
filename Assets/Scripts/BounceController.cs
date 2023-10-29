using UnityEngine;

public class BounceController : MonoBehaviour
{
    public float bounceHeight = 0.1f; 
    public float bounceSpeed = 10f; 

    private Vector3 initialPosition;
    private float counter = 0;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        counter += bounceSpeed * Time.deltaTime;

        transform.position = new Vector3(
            initialPosition.x,
            initialPosition.y + bounceHeight * Mathf.Sin(counter),
            initialPosition.z
        );
    }
}
