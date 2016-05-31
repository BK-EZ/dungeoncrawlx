using UnityEngine;

// Launch projectiles
public class PlayerAttackSCRIPT : MonoBehaviour
{
    public Transform shotPrefab; // Prefab that the script spawns- this variable is set in the Unity inspector

    public float shootingRate = 0.25f; // Cooldown in seconds between two shots
    private float shootCooldown;

    void Start() // initialization
    {
        shootCooldown = 0f;
    }

    void Update() // update every frame
    {
        if (shootCooldown > 0)
        {
            shootCooldown -= Time.deltaTime; // decrease cooldown
        }
    }

    // Create a new projectile if possible
    public void Attack()
    {
        if (CanAttack())
        {
            shootCooldown = shootingRate; // set cooldown

            // new shot
            var shot_TF = Instantiate(shotPrefab) as Transform; // create a new shot
            shot_TF.position = transform.position; // assign position
        }
    }

    public bool CanAttack() // Is the weapon ready to create a new projectile?
    {
            return shootCooldown <= 0f ;
    }
}