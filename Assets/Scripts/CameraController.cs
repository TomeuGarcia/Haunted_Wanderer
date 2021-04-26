using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Attached Game Objects
    public GameObject followObject;
    public GameObject lLimit;
    public GameObject rLimit;

    // Variables
    private Vector2 follow;
    private Vector2 threshold;
    public Vector2 followOffset;

    private float lLimitDist;
    private float rLimitDist;

    private float xDiff;
    private float yDiff;

    private bool onGround;
    private float groundLength;
    private bool moveVertically;

    private float speed = 10.0f;

    // Components
    private Rigidbody2D rb2;
    public LayerMask groundLayer;

    void Start()
    {
        threshold = calculateThreshold();
        rb2 = followObject.GetComponent<Rigidbody2D>();

        groundLength = Camera.main.orthographicSize;
        moveVertically = false;
    }


    // Move Camera when followObject (Player) moves
    void FixedUpdate()
    {
        // Variable that keeps track of followObject's position
        follow = followObject.transform.position;

        // Calculate distance between camera-leftsideLimit and camera-rightsideLimit
        lLimitDist = Mathf.Abs(lLimit.transform.position.x - follow.x);
        rLimitDist = Mathf.Abs(rLimit.transform.position.x - follow.x);
        // Don't move the camera if player is close to the limits
        if (lLimitDist <= threshold.x * 0.95f || rLimitDist <= threshold.x * 0.95f)
        {
            return;
        }

        /*
        if (transform.position.y > follow.y)
            follow.y *= 0.05f;
        */

        // Variable that keeps track of the distance from the followObject to the center of the Camera on the X axis
        xDiff = Mathf.Abs(transform.position.x - follow.x);
        // Variable that keeps track of the distance from the followObject to the center of the Camera on the Y axis
        yDiff = Mathf.Abs(transform.position.y - follow.y);
        onGround = Physics2D.Raycast(transform.position, Vector2.down, groundLength, groundLayer);



        // Update Camera's position evaluating folowObject's position with camera threshold
        Vector3 newPosition = transform.position;
        if (xDiff >= threshold.x / 8.0f)
        {
            newPosition.x = follow.x;
        }
        if (yDiff >= threshold.y / 2.0f && !moveVertically)
        {
            moveVertically = true;
        }
        if (moveVertically && !onGround) {
            newPosition.y = follow.y;
            if (transform.position.y == follow.y) {
                moveVertically = false;
            }
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

        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundLength));
    }

}
