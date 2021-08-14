using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public Tilemap floorTilemap, roadTilemap, wallTilemap, buildingTilemap;
    public GameObject Arch;
    private Vector2 mapDimensions = new Vector2(10, 10);

    private int tileSize = 2;

    private Vector2 startPoint, endPoint;

    void Start()
    {
        
    }
}
