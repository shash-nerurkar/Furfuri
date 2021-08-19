using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNotifier : MonoBehaviour
{    
    void Start()
    {
        StartCoroutine(Move(transform.position + new Vector3(0, 0.5f), 20/60f));
    }

    IEnumerator Move(Vector3 destinationPosition, float lerpDuration, bool deleteAfter = false)
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

        if(deleteAfter)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(Move(transform.position - new Vector3(0, 0.75f), 20/60f, true));
        }
    }
}
