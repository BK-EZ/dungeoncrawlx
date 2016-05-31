using UnityEngine;

// Handle hitpoints and damages for enemies.
public class EnemyHealthSCRIPT : MonoBehaviour
{
    public int hp = 1; // hit points; number of times enemy can be hit

    // Inflicts damage and check if the object should be destroyed
    public void Damage(int damageCount)
    {
        hp -= damageCount;

        if (hp <= 0) // enemy is kill
        {
            gameObject.GetComponent<EnemyBehaviorSCRIPT>().die();
        }
    }

    void OnTriggerEnter2D(Collider2D otherCollider) // collision checking
    {
        // Is this a Player Shot?
        PlayerShotSCRIPT shot = otherCollider.gameObject.GetComponent<PlayerShotSCRIPT>();
        if (shot != null)
        {
            Damage(shot.damage); // take damage
            Destroy(shot.gameObject); // Destroy the shot           
        }
    }
}
