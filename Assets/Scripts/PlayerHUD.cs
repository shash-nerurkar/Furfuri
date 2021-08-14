using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public Player player;
    private Vector3 distanceFromPlayer = new Vector3(0, 0.9f, 0);
    void Update()
    {
        transform.position = player.transform.position + distanceFromPlayer;
    }
}
