using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private GameObject HUD;

    void Start()
    {
        HUD = GameObject.Find("HUD");
    }

    public void flingCoinFunction(Vector3 destinationPosition)
    {
        StartCoroutine(flingCoin(destinationPosition, Random.Range(2, 5)));
    }

    public IEnumerator flingCoin(Vector3 destinationPosition, float lerpDuration)
    {
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;
        
        while (timeElapsed < lerpDuration)
        {
            transform.position = Vector3.Lerp(startPosition, destinationPosition, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            
            yield return null;
        }

        transform.position = destinationPosition;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            HUD.SendMessage("UpdateScore");
            Destroy(gameObject);
        }
    }
}
