using UnityEngine;

// Handles movement of most game objects.
public class MovementSCRIPT : MonoBehaviour
{
    public bool automove = true; // enables basic movement in a direction
    
    public bool jittering = true; // adds "jittering" effect to object's movement
    private float jitterHeight = 1f; // height of jitter bounces
    private float jitterSpeed = 2f; // speed at which to bounce
    private bool jitterUp = true; // determines if object is currently "jittering" up or down
    private float jitterDist = 0; // current distance off-center of our jitter
    private float thisFrameJitter;

    public Vector2 speed = new Vector2(10, 10); // speed
    public Vector2 direction = new Vector2(-1, 0); // direction (For X: NEG is LEFT. POS is RIGHT. For Y: NEG is DOWN. POS is UP.)

    private Vector2 movement;
    void Start() // initialization
    {

    }

    void Update()
    {          
        if(jittering)
        {
            thisFrameJitter = Time.deltaTime * jitterSpeed;

            if(jitterUp)
            {
                transform.position += new Vector3(0f, thisFrameJitter, 0f); // move up
                jitterDist += thisFrameJitter;
            }
            else
            {
                transform.position -= new Vector3(0f, thisFrameJitter, 0f); // move down
                jitterDist -= thisFrameJitter;
            }
        }

        // Reverse direction when height or 'floor' is reached
        if (jitterDist >= jitterHeight)
        {
            jitterUp = false;
        }
        else
        if (jitterDist <= 0)
        {
            jitterUp = true;
        }

        if (automove)
        {
            movement = new Vector2(speed.x * direction.x, speed.y * direction.y); // set movement
        } 
    }

    void FixedUpdate()
    {
        if (automove)
        {
            GetComponent<Rigidbody2D>().velocity = movement; // Apply movement to the rigidbody
        }      
    }
}