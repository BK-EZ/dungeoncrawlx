using UnityEngine;
using System.Collections;

/* Handles the background scrolling to the right and looping. The looping effect is created by having 3 side-to-side backgrounds scroll to the 
   right and when the rightmost background reaches a certain offcamera position, it destroys itself, and a new background is put to the left 
   of the two remaining backgrounds. */

public class BackgroundLoopSCRIPT : MonoBehaviour 
{
    public Transform bgPrefab; // Prefab that the script spawns- this variable is set in the Unity inspector

    private Transform bgRight_TF; // rightmost background
    private Transform bgMid_TF; // middle background
    private Transform bgLeft_TF; // leftmost background   
    
    public Vector2 speed = new Vector2(5, 0); // speed backgrounds scroll at
    public Vector2 direction = new Vector2(1, 0); // scrolls to the right

    // The test BGs currently used are *I think* an exact width of 19.2f- but for reasons I don't understand, noticable gaps between LeftBG and MidBG
    // are visible with that value, so I found 19 to be 'good enough' for now.
    public float bgWidth = 19; // used to calculate where to spawn new LeftBG relative to current MidBG 
    
    void Start() // Use this for initialization
    {
        // Create 3 backgrounds
        bgRight_TF = Instantiate(bgPrefab) as Transform;
        bgMid_TF = Instantiate(bgPrefab) as Transform;
        bgLeft_TF = Instantiate(bgPrefab) as Transform;

        // Assign positions
        bgRight_TF.position = new Vector3(19.2f, 0, 10);
        bgMid_TF.position = new Vector3(0, 0, 10);
        bgLeft_TF.position = new Vector3(-19.2f, 0, 10);	
	}

    void Update() // Update is called once per frame
    {
        // move each background
        Vector3 movement = new Vector3(speed.x * direction.x, speed.y * direction.y, 0);
        movement *= Time.deltaTime;
        bgRight_TF.Translate(movement);
        bgMid_TF.Translate(movement);
        bgLeft_TF.Translate(movement);

        // loop effect
        if(bgRight_TF.position.x >= 19.2f*1.75f) // if rightmost background moves too far right...
        {
            Destroy(bgRight_TF.gameObject); // delete RightBG
            bgRight_TF = bgMid_TF; // MidBG is now RightBG
            bgMid_TF = bgLeft_TF; // LeftBG is now the MidBG

            float leftpos = (bgMid_TF.position.x) - (bgWidth); // get positon for new LeftBG to link up with MidBG
            bgLeft_TF = Instantiate(bgPrefab) as Transform; // create LeftBG
            bgLeft_TF.position = new Vector3(leftpos, 0, 10); // set pos for LeftBG
        }
	}
}
