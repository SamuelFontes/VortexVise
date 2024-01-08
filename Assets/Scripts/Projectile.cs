using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float BaseDamage;
    public GameObject Explosion;
    public Teams Team;
    public Vector2 Direction;
    public float Force;
    
    private Rigidbody2D Rigidbody;
    public void Init(float baseDamage, GameObject explosion, Teams team, Vector2 direction, float force)
    {
        BaseDamage = baseDamage;
        if(explosion != null) 
        Explosion = explosion;
        Team = team;
        Direction = direction;
        Force = force;
        Rigidbody = GetComponent<Rigidbody2D>();

        ApplyTeam();
        ApplyForce();
    }

    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //TODO: Apply damage
        Instantiate(Explosion, transform.position, transform.rotation);
        GameObject.Destroy(gameObject);
    }

    void ApplyForce()
    {
        Rigidbody.velocity = Direction * Force;
    }
    void ApplyTeam()
    {
        gameObject.layer = (int)Team;
    }
}
