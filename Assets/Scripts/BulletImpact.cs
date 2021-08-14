using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletImpact : MonoBehaviour
{
    
    public void OnExplosionFinished()
    {
        Destroy(gameObject);
    }
}
