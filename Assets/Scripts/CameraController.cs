using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject followObject;
    public GameObject leftsideLimit;
    public GameObject rightsideLimit;

    public Vector2 followOffset;
    public float speed = 3.0f;
    private Vector2 threshold;
    private Rigidbody2D rb2;
    private float yVariance;

    // Start is called before the first frame update
    void Start()
    {
        threshold = calculateThreshold();
        rb2 = followObject.GetComponent<Rigidbody2D>();
    }

    // Move Camera when followObject (Player) moves
    void FixedUpdate()
    {
        // Calculate distance between camera-leftsideLimit and camera-rightsideLimit
        float distanceLeftsideLimit = leftsideLimit.transform.position.x - followObject.transform.position.x;
        float distanceRightsideLimit = rightsideLimit.transform.position.x - followObject.transform.position.x;
        // Don't move the camera if player is close to the limits
        if (Mathf.Abs(distanceLeftsideLimit) <= threshold.x * 0.95f || distanceRightsideLimit <= threshold.x * 0.95f)
        {
            return;
        }

        // Variable that keeps track of followObject's position
        Vector2 follow = followObject.transform.position;
        if (transform.position.y > follow.y)
            follow.y *= 0.05f;
            

        // Variable that keeps track of the distance from the followObject to the center of the Camera on the X axis
        // (Vector2.right = (1,0,0))
        float xDifference = Vector2.Distance(Vector2.right * transform.position.x, Vector2.right * follow.x);

        // Variable that keeps track of the distance from the followObject to the center of the Camera on the Y axis
        // (Vector2.up = (0,1,0))
        float yDifference = Vector2.Distance(Vector2.up * transform.position.y, Vector2.up * follow.y);


        // Update Camera's position evaluating folowObject's position with camera threshold
        Vector3 newPosition = transform.position;
        if (Mathf.Abs(xDifference) >= threshold.x / 8.0f)
        {
            newPosition.x = follow.x;
        }
        if (Mathf.Abs(yDifference) >= threshold.y / 8.0f)
        {
            if (transform.position.y > 3f)
                newPosition.y = follow.y;
        }
        // Move Camera at required speed
        float moveSpeed = rb2.velocity.magnitude > speed ? rb2.velocity.magnitude : speed;
        transform.position = Vector3.MoveTowards(transform.position, newPosition, moveSpeed * Time.deltaTime);
    }

    // Function that calculates and returns Camera's threshold 
    private Vector3 calculateThreshold()
    {
        Rect aspect = Camera.main.pixelRect;
        Vector2 t = new Vector2(Camera.main.orthographicSize * aspect.width / aspect.height, Camera.main.orthographicSize);
        t.x -= followOffset.x;
        t.y -= followOffset.y;
        return t;
    }

    // Function that visualized Camera threshold in Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector2 border = calculateThreshold();
        Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));
    }

}
