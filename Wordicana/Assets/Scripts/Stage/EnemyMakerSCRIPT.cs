using UnityEngine;
using System.Collections;

// Handles infinitely spawning enemies from the right side of the screen.
public class EnemyMakerSCRIPT : MonoBehaviour 
{    
    public Transform enemyPrefab; // Prefab that the script spawns- this variable is set in the Unity inspector
   
    // after every spawn, baneTimer is set to a random value between spawnTimeMin and spawnTimeMax
    public float spawnTimeMin = 2; 
    public float spawnTimeMax = 5;
    
    private float spawnTimer; // acts a timer/cooldown for spawning
    public float spawnXPos = 10; // x position of where enemies spawn- 10 is roughly just offscreen to the right

    void Start() // Use this for initialization
    {
        spawnTimer = Random.Range(spawnTimeMin, spawnTimeMax); // timer for first spawn
	}

    void Update() // Update is called once per frame
    {
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime; // reduce timer by seconds passed
        }
        
        if(CanSpawn())
        {
            var enemy_TF = Instantiate(enemyPrefab) as Transform; // create the enemy
            enemy_TF.position = new Vector3(spawnXPos, -3.1f, 0); // position it into the game
            spawnTimer = Random.Range(spawnTimeMin, spawnTimeMax); // set spawn time for next enemy
        }
	}

    public bool CanSpawn() // is spawnTimer active?
    {
        return (spawnTimer <= 0);
    }
}
