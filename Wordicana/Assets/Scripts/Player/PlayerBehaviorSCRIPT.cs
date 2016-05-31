using UnityEngine;

// Player controller and behavior
// * Note that currently the player character is movable ingame with WASD, just because why the fuck not. Once the actual word mechanic is implemented,
// * move will have to be disabled, or at least mapped off of WASD.
public class PlayerBehaviorSCRIPT : MonoBehaviour
{
  public Vector2 speed = new Vector2(50, 50); // speed of Character
  private Vector2 movement; // store movement

  void Update() // called every frame
  {
    // Retrieve input axis information
    float input_X = Input.GetAxis("Horizontal");
    float input_Y = Input.GetAxis("Vertical");

    // Movement per direction
    movement = new Vector2(speed.x * input_X, speed.y * input_Y); 

    // Shooting
    bool shoot = (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"));
    if (shoot)
    {
        PlayerAttackSCRIPT weapon = GetComponent<PlayerAttackSCRIPT>();
        weapon.Attack();
    }
  }

  void OnTriggerEnter2D(Collider2D collision) // Collision event handling
  {
      // Collision with enemy
      EnemyBehaviorSCRIPT enemy = collision.gameObject.GetComponent<EnemyBehaviorSCRIPT>();
      if (enemy != null)
      {
          enemy.fallBack();
          PlayerHealthSCRIPT playerHealth = this.GetComponent<PlayerHealthSCRIPT>();
          playerHealth.Damage(1);
      }
  }

  void FixedUpdate()
  {
    GetComponent<Rigidbody2D>().velocity = movement; // Move game object
  }

}