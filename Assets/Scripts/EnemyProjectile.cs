using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    public float speed = 5f;
    public int damage = 3;
    public float[] slowArgs = {0.4f};
    
    void Start ()
    {
        rigidBody.velocity = transform.up * speed;
        rigidBody.AddForce(Vector3.up * 5);        
    }

    void OnTriggerEnter2D (Collider2D bulletHitInfo)
    {
        Player player = bulletHitInfo.gameObject.GetComponent<Player>();

        if(player != null)
        {
            player.TakeDamage(damage);
            player.ApplyStatus("net", slowArgs);
        }
        
        Destroy(gameObject);
    }
}
