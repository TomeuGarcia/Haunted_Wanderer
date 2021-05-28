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
    public bool canMoveUp;
    public bool canMoveDown;


    private float speed = 6.0f;

    // Components
    private PlayerController pc;
    private Rigidbody2D rb2;
    public LayerMask groundLayer;

    public Collider2D ceilingTrigger;
    public Collider2D groundTrigger;

    void Start()
    {
        threshold = calculateThreshold();
        pc = followObject.GetComponent<PlayerController>();
        rb2 = followObject.GetComponent<Rigidbody2D>();

        ceilingLength = groundLength = Camera.main.orthographicSize;// * 0.8f;
        moveVertically = false;
        moveHorizontally = false;
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
       const float offset = 5f;
       if ((canMoveUp && follow.y > transform.position.y) ||
           (canMoveDown && follow.y < transform.position.y))
       {
            newPosition.y = follow.y;
       }
       //else
       //{
       //     newPosition.y = transform.position.y;
       //}



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
        transform.position = new Vector3(follow.x, follow.y + groundLength - 1.1f, transform.position.z);
        yield return new WaitForSeconds(0.2f);
        //pc.canMove = true;
        pc.offCamera = false;
    }


    // Check if camera can move vertically (not colliding with camera limits)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CameraFloor"))
        {
            if (other.IsTouching(ceilingTrigger))
            {
                canMoveUp = false;
                Debug.Log("cam touching ceiling");
            }
            else if (other.IsTouching(groundTrigger))
            {
                canMoveDown = false;
                Debug.Log("cam touching ground");
            }
            Debug.Log("trigger enter");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("CameraFloor")) {
            if (!other.IsTouching(ceilingTrigger))
            {
                canMoveUp = true;
                Debug.Log("cam NOT touching ceiling");
            }
            else if (!other.IsTouching(groundTrigger))
            {
                canMoveDown = true;
                Debug.Log("cam NOT touching ground");
            }
            Debug.Log("trigger exit");
        }
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
