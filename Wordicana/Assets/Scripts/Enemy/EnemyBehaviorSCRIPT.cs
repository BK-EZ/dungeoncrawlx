using UnityEngine;
using System.Collections;

// handles behavior for enemy including dying and "falling back"- basic movement handled in movementSCRIPT.cs
public class EnemyBehaviorSCRIPT : MonoBehaviour 
{
    // when an enemy damages player, it enters "fall back"- moving away to the right for several frames
    public bool willFallBack = true; // if true, enables fall back behavior
    public int fallBackAmt = 20; // number of frames an enemy falls back for
    public float fallBackSpeed = 2f; // speed at which an enemy fall back at; note this is added to- not replacing- the enemy's normal move speed
    private bool inFallBack = false; // handles above case- true while falling back
    private int fallBackTimer; // handles fall back time

    // when an enemy dies, it flies off at a random x and y speeds while spinning- these variables handle this "event"
    private float dieSpeed_X; // actual speeds after calaculation
    private float dieSpeed_Y; 
    public float dieSpeedMin_X = 50; // Min and Max possible speeds- varying speeds means varying launch angles!
    public float dieSpeedMax_X = 50; 
    public float dieSpeedMin_Y = 10;
    public float dieSpeedMax_Y = 50;
    private Vector2 dieDirectionVector = new Vector2(1, 1); // move up + right
    private bool isDead = false; // enemy is kill
    public float rotateAmount = 20; // degrees of rotation per frame

    private bool killSwitch = false; // set to true if enemy wanders too far left 

    void Start() // Use this for initialization
    {
	
	}

    void Update() // Update is called once per frame
    {
        if(!killSwitch && transform.position.x <= -11.0) // if killswitch isn't already on and enemy is too far left...
        {
            killSwitch = true;                           
            Destroy(gameObject, 2); // destroy this enemy in 2 seconds
        }

        if(inFallBack) // if currently falling back...
        {
            transform.position += new Vector3(fallBackSpeed, 0f, 0f); // directly change position to new value to the right
            fallBackTimer--; // reduce frames remaining for timer
            
            if(fallBackTimer == 0) // done falling back
            {
                inFallBack = false;
            }
        }
        else if(isDead) 
        {
            transform.Rotate(Vector3.forward * rotateAmount); // spin around while dead and flying
        }	
	}

    public void fallBack()
    {
        if(!willFallBack) // ignore function if not set to fall back
        { 
            return; 
        }

        inFallBack = true;
        fallBackTimer = fallBackAmt; // set frame timer;
    }

    public void die() // change velocity to fly off to the right when dead
    {
        isDead = true;
        MovementSCRIPT moveScript = this.GetComponent<MovementSCRIPT>();
        
        // set x direction speed to number between min and max
        if(dieSpeedMin_X == dieSpeedMax_X)
        {
            dieSpeed_X = dieSpeedMin_X;
        }
        else
        {
            dieSpeed_X = Random.Range(dieSpeedMin_X, dieSpeedMax_X);
        }

        // set y direction speed to number between min and max
        if (dieSpeedMin_Y == dieSpeedMax_Y)
        {
            dieSpeed_Y = dieSpeedMin_Y;
        }
        else
        {
            dieSpeed_Y = Random.Range(dieSpeedMin_Y, dieSpeedMax_Y);
        }

        moveScript.speed = new Vector2(dieSpeed_X, dieSpeed_Y); // atually update move speed
        moveScript.direction = dieDirectionVector; // change direction (up + right)
        Destroy(gameObject, 2f); // destroy after 2 seconds
    }
}
