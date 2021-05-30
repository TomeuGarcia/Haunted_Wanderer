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

    private float groundLength;
    private float ceilingLength;
    public bool moveHorizontally;
    public bool moveVertically;
    public bool canMoveUp = false;
    public bool canMoveDown = false;

    public Vector3 respawnPosition;

    private float speed = 6.0f;

    // Components
    private PlayerController pc;
    private Rigidbody2D rb2;
    public LayerMask cameraLimitLayer;

    void Start()
    {
        threshold = calculateThreshold();
        pc = followObject.GetComponent<PlayerController>();
        rb2 = followObject.GetComponent<Rigidbody2D>();

        ceilingLength = groundLength = Camera.main.orthographicSize;
        moveVertically = false;
        moveHorizontally = false;

        respawnPosition = transform.position;
    }

    private void Update()
    {
        // Camera can move up if not in contact with cameraCeiling
        canMoveUp = !Physics2D.Raycast(transform.position, Vector2.up, ceilingLength, cameraLimitLayer);
        // Camera can move down if not in contact with cameraGround
        canMoveDown = !Physics2D.Raycast(transform.position, Vector2.down, groundLength, cameraLimitLayer);
    }



    // Move Camera when followObject (Player) moves
    void FixedUpdate()
    {
        // set camera position equal to player's position if it went out off view range
        if (pc.offCamera)
            StartCoroutine(FocusFollow());


        // Variable that keeps track of followObject's position
        follow = followObject.transform.position;

        // Variable that makes the camera pass through the hole level ("cinematic")
        //follow = new Vector3(transform.position.x +5, transform.position.y, transform.position.z);

        // Calculate distance between camera-leftsideLimit and camera-rightsideLimit
        lLimitDist = Mathf.Abs(lLimit.transform.position.x - follow.x);
        rLimitDist = Mathf.Abs(rLimit.transform.position.x - follow.x);
        // Don't Move the camera if player is close to the limits
        if (lLimitDist <= threshold.x * 0.95f || rLimitDist <= threshold.x * 0.95f)
        {
            return;
        }


        // Variable that keeps track of the distance from the followObject to the center of the Camera on the X axis
        xDiff = transform.position.x - follow.x;
        // Variable that keeps track of the distance from the followObject to the center of the Camera on the Y axis
        yDiff = transform.position.y - follow.y;


        // Camera moves horizontally
        // Update Camera's position evaluating followObject's position with camera threshold
        Vector3 newPosition = transform.position;
        if ((Mathf.Abs(xDiff) >= threshold.x / 8.0f) && !moveHorizontally)
        {
            moveHorizontally = true;
        }
        if (moveHorizontally)
        {
            newPosition.x = follow.x;
            if (Mathf.Abs(follow.x) <= Mathf.Abs(transform.position.x) + 0.05f && Mathf.Abs(follow.x) >= Mathf.Abs(transform.position.x) - 0.05f)
            {
                moveHorizontally = false;
            }
        }


        // Camera moves vertically
        const float offset = 2f;// 3f;// 5f;
       if ((canMoveUp && follow.y > transform.position.y + offset) ||
           (canMoveDown && follow.y < transform.position.y - offset))
       {
            newPosition.y = follow.y; 
       }
       else
       {
            newPosition.y = transform.position.y;
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


    IEnumerator FocusFollow()
    {
        //pc.canMove = false;
        //transform.position = new Vector3(follow.x, follow.y + groundLength - 1.1f, transform.position.z);
        transform.position = respawnPosition;
        yield return new WaitForSeconds(0.2f);
        //pc.canMove = true;
        pc.offCamera = false;
    }


    // Function that visualized Camera threshold in Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector2 border = calculateThreshold();
        Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));

        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundLength));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + ceilingLength));
    }

}
