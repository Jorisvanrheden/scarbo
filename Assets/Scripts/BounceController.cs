using UnityEngine;

public class BounceController : MonoBehaviour
{
    public float bounceHeight = 0.1f; 
    public float bounceSpeed = 10f; 

    private float counter = 0;

    private Transform parentTransform;
    private float initialOffsetY;

    private void Awake()
    {
        parentTransform = transform.root;

        initialOffsetY = transform.position.y - parentTransform.position.y;
    }

    void Update()
    {
        counter += bounceSpeed * Time.deltaTime;

        transform.position = new Vector3(
            parentTransform.position.x,
            initialOffsetY + parentTransform.position.y + bounceHeight * Mathf.Sin(counter),
            parentTransform.position.z
        );
    }
}
