using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingContainer : MonoBehaviour
{
    public GameObject buildingPrefab;
    public LinkedList<GameObject> buildingList = new LinkedList<GameObject>();

    void Start()
    {
        for( int i=0; i<30; i++ )
            buildingList.AddLast( Instantiate( buildingPrefab, 
                                               new Vector3(Random.Range(0, 10), Random.Range(0, 10), 0), 
                                               Quaternion.identity,
                                               transform ) );

        // foreach(GameObject building in buildingList) {
        //     print(building.transform.position);
        // }
    }
}
