using UnityEngine;

public class BounceController : MonoBehaviour
{
    public float bounceHeight = 0.1f; 
    public float bounceSpeed = 10f; 

    private float initialPositionY;
    private float counter = 0;

    void Start()
    {
        initialPositionY = transform.position.y;
    }

    void Update()
    {
        counter += bounceSpeed * Time.deltaTime;

        transform.position = new Vector3(
            transform.position.x,
            initialPositionY + bounceHeight * Mathf.Sin(counter),
            transform.position.z
        );
    }
}
