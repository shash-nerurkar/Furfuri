using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    public enum State 
    {
        Idle,
        Aiming,
        Shooting,
        Spawning,
    }
    public State state = State.Idle;
    public enum AttackTypes
    {
        lockIn, 
        bullet, 
        projectile,
    }
    public AttackTypes attackType;// = AttackTypes.bullet;
    public World world;
    public Transform muzzle;
    public Player player;
    public Tilemap buildingTilemap;
    public GameObject enemyContainer;
    public Animator animator;
    public GameObject bulletPrefab;
    public GameObject projectilePrefab;
    public LineRenderer bulletLineRenderer;
    public GameObject bulletImpactPrefab;
    public Transform projectileContainerTransform;
    private float shootTimer;
    private float despawnTimer;
    public int health = 50;
    private int maxHealth = 50;
    public int shootRange = 4;
    public int despawnRange = 8;
    public float shootCooldown = 3.0f;
    public float despawnCooldown = 5.0f;
    public int rayDamage = 3;

    void Start()
    {
        // Enemy.DrawCircle(gameObject.transform, gameObject.GetComponent<LineRenderer>(), despawnRange);
        StartCoroutine(MoveToSpawnLocation());
    }

    void Update()
    {   
        switch(state)
        {
            default:

            case State.Idle:
                CheckRange();
                despawnTimer += Time.deltaTime;
                if(despawnTimer >= despawnCooldown) 
                {
                    state = State.Spawning;
                    animator.SetInteger("Alive", -1);
                    animator.SetInteger("Shoot", 0);
                    despawnTimer = 0;
                }
                break;
            
            case State.Aiming:
                Aim();
                CheckRange();
                shootTimer += Time.deltaTime;
                if(shootTimer >= shootCooldown) 
                {
                    state = State.Shooting;
                    animator.SetInteger("Shoot", 1);
                    shootTimer = 0;
                }
                break;
            
            case State.Shooting:
                Aim();
                break;
            
            case State.Spawning:
                break;

        }
    }

    public void Aim() 
    {
        transform.up = player.transform.position - transform.position;
    }

    public void CheckRange()
    {
        if(Vector3.Distance(transform.position, player.transform.position) <= shootRange)
        {
            state = State.Aiming;
        }
        else if(Vector3.Distance(transform.position, player.transform.position) > despawnRange)
        {
            animator.SetInteger("Alive", -1);
            animator.SetInteger("Shoot", 0);
            state = State.Spawning;
        }
        else
        {
            state = State.Idle;
        }
    }

    public void TakeDamage(int damage) 
    {
        health -= damage;
        if(health <= 0)
        {
            health = 0;
            animator.SetInteger("Alive", -1);
            animator.SetInteger("Shoot", 0);
            state = State.Spawning;
        }
    }

    public void OnDespawnFinish()
    {
        health = maxHealth;
        animator.SetInteger("Alive", 1);
        StartCoroutine(MoveToSpawnLocation());
    }

    public void OnRespawnFinish() 
    {
        animator.SetInteger("Alive", 0);
        state = State.Idle;
    }
    
    public void OnShootFinish()
    {
        switch(attackType)
        {
            default:
            case AttackTypes.bullet:
                ShootBullet();
                break;

            case AttackTypes.lockIn:
                StartCoroutine(ShootRay());
                break;

            case AttackTypes.projectile:
                ShootProjectile();
                break;
        }
        animator.SetInteger("Shoot", 2);
    }
    
    public void OnReloadFinish()
    {
        state = State.Idle;
        animator.SetInteger("Shoot", 0);
    }

    public void ShootProjectile()
    {
        Instantiate(projectilePrefab, muzzle.position, muzzle.rotation, projectileContainerTransform);
    }

    public void ShootBullet()
    {
        Instantiate(bulletPrefab, muzzle.position, muzzle.rotation, projectileContainerTransform);
    }

    IEnumerator ShootRay() 
    {
        RaycastHit2D rayHitInfo = Physics2D.Raycast(muzzle.transform.position, 
                                                    muzzle.transform.up, 
                                                    float.PositiveInfinity, 
                                                    LayerMask.GetMask(("Player")));
        
        if(rayHitInfo)
        {
            Player playerHit = rayHitInfo.transform.GetComponent<Player>();
            if(playerHit != null)
            {
                playerHit.TakeDamage(rayDamage);
            }
            
            GameObject currentImpact = Instantiate(bulletImpactPrefab, rayHitInfo.point, Quaternion.identity);
            currentImpact.GetComponent<SpriteRenderer>().color = Color.blue;

            bulletLineRenderer.SetPosition(1, transform.InverseTransformPoint(rayHitInfo.point));
        }
        else
        {
            bulletLineRenderer.SetPosition(1, transform.InverseTransformPoint(muzzle.transform.position + muzzle.transform.right*100));
        }

        bulletLineRenderer.enabled = true;

        yield return new WaitForSeconds(1f);

        bulletLineRenderer.enabled = false;
    }

    IEnumerator MoveToSpawnLocation()
    {
        List<Vector3Int> buildingCellLocations = player.GetNearbyBuildings(shootRange);
        if(buildingCellLocations.Count == 0)
        {
            yield return new WaitForSeconds(0f);
        }
        else
        {
            transform.position = buildingTilemap.CellToWorld(buildingCellLocations[(int)Random.Range(0, buildingCellLocations.Count-1)]);
            yield return new WaitForSeconds(0f); 
            // List<Vector3> enemyLocations = new List<Vector3>();
            // int sleepCounter = 5;
            // for(int i=0; i<enemyContainer.transform.childCount; ++i)
            // {
            //     enemyLocations.Add(enemyContainer.transform.GetChild(i).transform.position);
            // }

            // while(true)
            // {
            //     Vector3 selectedPosition = buildingTilemap.CellToWorld(buildingCellLocations[(int)Random.Range(0, buildingCellLocations.Count)]);
            //     if(!enemyLocations.Contains(selectedPosition))
            //     {
            //         transform.position = selectedPosition;
            //         break;
            //     }    
            //     ++sleepCounter;
            //     yield return new WaitForSeconds(3f);       
            // }
        }
    }

    public static void DrawCircle(Transform transform, LineRenderer line, float radius, float lineWidth = 0.2f)
    {
        var numberOfPoints = 360;
        Vector3 pos;
        float theta = 0f;
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = numberOfPoints;

        for (int i = 0; i < numberOfPoints; i++)
        {
            theta += (2.0f * Mathf.PI * 0.01f);
            pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0);
            line.SetPosition(i, pos);
        }
    }   

    // public void OnDrawGizmos()
    // {
    //     Gizmos.color = new Color(1, 0, 0, 0.2f);
    //     Gizmos.DrawSphere(transform.position, shootRange);
    // }
}
