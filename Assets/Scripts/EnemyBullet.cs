using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public GameObject bulletImpactPrefab;
    public float speed = 5f;
    public int damage = 3;
    public Rigidbody2D rb;

    void Start ()
    {
        rb.velocity = transform.up * speed;        
    }

    void OnTriggerEnter2D (Collider2D bulletHitInfo)
    {
        Player player = bulletHitInfo.gameObject.GetComponent<Player>();

        if(player != null)
        {
            player.TakeDamage(damage);
        }

        GameObject currentImpact = Instantiate(bulletImpactPrefab, transform.position, Quaternion.identity);
        currentImpact.GetComponent<SpriteRenderer>().color = Color.blue;
        
        Destroy(gameObject);
    }
}
