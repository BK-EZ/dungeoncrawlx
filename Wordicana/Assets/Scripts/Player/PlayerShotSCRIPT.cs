using UnityEngine;

// Projectile behavior
public class PlayerShotSCRIPT : MonoBehaviour
{
    public int damage = 1; // Damage inflicted

    void Start()
    {
        Destroy(gameObject, 4); // destroy this after 4 seconds
    }
}