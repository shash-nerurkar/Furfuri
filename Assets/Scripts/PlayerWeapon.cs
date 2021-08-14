using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private enum State 
    {
        Searching,
        Aiming,
    }
    private State state = State.Searching;
    public Transform muzzleTransform;
    public Transform weaponPivotTransform;
    public int damage = 40;
    public int shootRange = 4;
    public GameObject bulletImpactPrefab;
    public LineRenderer bulletLineRenderer;
    public Transform enemyContainerTransform;
    public Transform projectileContainerTransform;
    private Transform targetEnemyTransform;
    private float minEnemyDist;
    private float aimStartTime;
    private float aimSpeed = 10;
    private float aimTotalRotation;
    private float aimRotationCovered;
    private float aimRotationFraction;
    private Vector3 weaponStartRotation;
    
    void Start()
    {
        Enemy.DrawCircle(gameObject.transform, gameObject.GetComponent<LineRenderer>(), shootRange);
    }

    void Update()
    {   
        switch(state)
        {
            default:
            
            case State.Searching:

                minEnemyDist = shootRange;
                foreach (Transform enemyTransform in enemyContainerTransform)
                {
                    if(Vector3.Distance(transform.position, enemyTransform.position) <= minEnemyDist)
                    {
                        state = State.Aiming;
                        minEnemyDist = Vector3.Distance(transform.position, enemyTransform.position);
                        targetEnemyTransform = enemyTransform;
                        weaponStartRotation = weaponPivotTransform.right;
                        aimStartTime = Time.time;
                        aimTotalRotation = Vector3.Distance(weaponPivotTransform.right, targetEnemyTransform.position - transform.position);
                    }
                }
                // if(state != State.Aiming)
                // {
                //     weaponPivotTransform.right += new Vector3(0, 1, 0);
                // }
                break;
            
            case State.Aiming:

                aimRotationCovered = (Time.time - aimStartTime) * aimSpeed;
                aimTotalRotation = Vector3.Distance(weaponPivotTransform.right, targetEnemyTransform.position - transform.position);
                aimRotationFraction = aimRotationCovered / aimTotalRotation;

                weaponPivotTransform.right = Vector3.Lerp(weaponStartRotation, targetEnemyTransform.position - transform.position, aimRotationFraction);
                
                if(targetEnemyTransform.gameObject == null)
                    state = State.Searching;
                else if(targetEnemyTransform.gameObject.GetComponent<Animator>().GetInteger("Alive") != 0)
                    state = State.Searching;
                else if(Vector3.Distance(transform.position, targetEnemyTransform.position) > shootRange)
                    state = State.Searching;
                break;
        }
    }

    public void ShootFunction() 
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot() 
    {
        RaycastHit2D rayHitInfo = Physics2D.Raycast(muzzleTransform.position, muzzleTransform.right, float.PositiveInfinity, LayerMask.GetMask(("Enemy")));
        if(rayHitInfo)
        {
            Enemy enemy = rayHitInfo.transform.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Instantiate(bulletImpactPrefab, rayHitInfo.point, Quaternion.identity, projectileContainerTransform);
            bulletLineRenderer.SetPosition(1, transform.InverseTransformPoint(rayHitInfo.point));
        }
        else
        {
            bulletLineRenderer.SetPosition(1, transform.InverseTransformPoint(muzzleTransform.position + muzzleTransform.right*100));
        }

        bulletLineRenderer.enabled = true;

        yield return new WaitForSeconds(0.1f);

        bulletLineRenderer.enabled = false;
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

}
