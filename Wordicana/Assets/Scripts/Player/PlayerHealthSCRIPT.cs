using System.Collections;
using UnityEngine;

// Handle hitpoints and damages for player. Also displays health indicators onscreen.
public class PlayerHealthSCRIPT : MonoBehaviour
{
    public Transform hpFullPrefab; // Prefab that the script spawns- this variable is set in the Unity inspector
    public Sprite hpMissSprite; // sprite that a health indicator changes to use when hurt
    public int hp = 3; // number of times a player can be hit
    
    public Vector3 hpPosVector = new Vector3(-8, 4, 20); // base position of first HP indicator
    public float hpPosMod_X = 1; // additional HP indicator are spaced this value apart

    ArrayList hpList = new ArrayList(); // holds hp indicators 

    void Start()
    {
        // display a health indicator sprite for each hp player has
        for(int i = 0; i < hp; i++) 
        {
            Transform hpFull_TF = Instantiate(hpFullPrefab) as Transform;

            // Assign position
            var pos = hpPosVector;
            pos.x += i*hpPosMod_X;
            hpFull_TF.position = pos;

            hpList.Add(hpFull_TF); // add to list
        }
    }

    // Inflicts damage and check if the object should be destroyed
    public void Damage(int damageCount)
    {
        hp -= damageCount;

        // update health indicator- currently only works with damageCount of 1
        if(damageCount == 1) 
        {
            Transform targetHP = hpList[hp] as Transform; // get specific hp indicator
            targetHP.gameObject.GetComponent<SpriteRenderer>().sprite = hpMissSprite; // change its sprite
        }

        if (hp <= 0) // player is kill
        {
            Destroy(gameObject);
        }
    }
}
