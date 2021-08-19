using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using Pathfinding;

public class Map : MonoBehaviour
{
    public Player player;
    public GameObject playerAI;
    public Grid grid1, grid2;
    public Tilemap floorTilemap, roadTilemap, wallTilemap, buildingTilemap;
    public Tile[] floorTiles;
    public RuleTile roadTile;
    public Tile wallTile, buildingTile;
    public AIPath mainPath;
    public Arch arch;
    public PolygonCollider2D cameraBoundary;
    public Vector2 mapDimensions  { get; set; }
    public Arch startArch { get; set; }
    public Arch endArch { get; set; }
    private Vector3 startPosition, endPosition;
    private int boundaryWallThickness = 2;
    private int cameraEdgePadding = 1;
    public bool[,] buildingFlags { get; set; }
    private System.Random RNG;

    void Start()
    {
        SaveData saveData = World.LoadGame();
        RNG = new System.Random();
        if(saveData != null)
        {
            parseSaveData(saveData);
        }
        else
        {
            mapDimensions = new Vector2(30, 30);
            SetExtremities();
            SpawnBuildings();
        }
        startPosition = startArch.transform.position - startArch.GetComponent<SpriteRenderer>().bounds.extents;
        player.SendMessage("SetStartOrientation", startArch.transform);

        playerAI.transform.position = startArch.transform.position;
        playerAI.transform.rotation = startArch.transform.rotation;
        playerAI.GetComponent<AIDestinationSetter>().target = endArch.transform;
        
        endPosition = endArch.transform.position - endArch.GetComponent<SpriteRenderer>().bounds.extents;
        SpawnFloor();
        SpawnBoundaryWalls();
        SetCameraBoundary();
        StartCoroutine(SetAStarGrid());
        ClearPath();
        SpawnRoad();
    }

    void SetExtremities()
    {
        startArch = AddArch(false);
        endArch = AddArch(true);
    }

    Arch AddArch(bool archEndTriggerStatus)
    {
        Arch archInstance = Instantiate(arch);
        archInstance.SendMessage("ArchEndTriggerStatus", archEndTriggerStatus);

        bool flag1 = RNG.Next(2) == 1, flag2 = RNG.Next(2) == 1;
        Vector3 archOffset = archInstance.GetComponent<SpriteRenderer>().bounds.extents;
        Vector3 point = new Vector3(0, 0, 0);
        
        if(flag1) // arch is horizontally aligned
        {
            int archCellOffsetX = grid1.WorldToCell(archOffset).x + 1;
            point.x = RNG.Next((int)mapDimensions.x - archCellOffsetX);
            if(flag2)
            {
                point.y = 0;
            }
            else
            {
                archInstance.transform.eulerAngles += new Vector3(0, 0, 180);
                point.y = mapDimensions.y - 1;
            }
        }
        else // arch is vertically aligned
        {
            int archCellOffsetY = grid1.WorldToCell(archOffset).y + 1;
            archInstance.transform.eulerAngles = new Vector3(0, 0, 90);
            point.y = RNG.Next((int)mapDimensions.y - archCellOffsetY);
            if(flag2)
            {
                archInstance.transform.eulerAngles += new Vector3(0, 0, 180);
                point.x = 0;
            }
            else
            {
                point.x = mapDimensions.x - 1;
            }
        }

        archOffset = archInstance.GetComponent<SpriteRenderer>().bounds.extents;
        archInstance.transform.position = point + archOffset;

        return archInstance;
    }

    void SpawnFloor()
    {
        Vector3Int mapCellDimensions = grid1.WorldToCell(mapDimensions);
        for(int i = 0; i < mapCellDimensions.x; i++)
        {
            for(int j = 0; j < mapCellDimensions.y; j++)
            {
                floorTilemap.SetTile(new Vector3Int(i, j, 0), floorTiles[RNG.Next(floorTiles.Length)]);
            }
        }
    }

    void SpawnBoundaryWalls()
    {
        Vector3Int mapCellDimensions = grid1.WorldToCell(mapDimensions);
        Vector3 startArchBoundExtents = startArch.GetComponent<SpriteRenderer>().bounds.extents;
        Vector3 endArchBoundExtents = endArch.GetComponent<SpriteRenderer>().bounds.extents;

        for(int j = 1; j <= boundaryWallThickness; j++)
        {
            for(int i = -boundaryWallThickness; i < mapCellDimensions.x + boundaryWallThickness; i++)
            {
                wallTilemap.SetTile(new Vector3Int(i, -j, 0), wallTile);

                wallTilemap.SetTile(new Vector3Int(i, mapCellDimensions.y- 1  + j, 0), wallTile);

                if(startArch.transform.rotation.eulerAngles.z / 90 % 2 == 0)
                {
                    if(i >= grid1.WorldToCell(startArch.transform.position - startArchBoundExtents).x && 
                       i <  grid1.WorldToCell(startArch.transform.position + startArchBoundExtents).x)
                    {
                        if(startArch.transform.rotation.eulerAngles.z / 90 == 0)
                        {
                            wallTilemap.SetTile(new Vector3Int(i, -j, 0), null);
                        }
                        else
                        {
                            wallTilemap.SetTile(new Vector3Int(i, mapCellDimensions.y - 1 + j, 0), null);
                        }
                    }
                }

                if(endArch.transform.rotation.eulerAngles.z / 90 % 2 == 0)
                {
                    if(i >= grid1.WorldToCell(endArch.transform.position - endArchBoundExtents).x && 
                       i <  grid1.WorldToCell(endArch.transform.position + endArchBoundExtents).x)
                    {
                        if(endArch.transform.rotation.eulerAngles.z / 90 == 0)
                        {
                            wallTilemap.SetTile(new Vector3Int(i, -j, 0), null);
                        }
                        else
                        {
                            wallTilemap.SetTile(new Vector3Int(i, mapCellDimensions.y - 1 + j, 0), null);
                        }
                    }
                }
            }
            for(int i = -boundaryWallThickness; i < mapCellDimensions.y + boundaryWallThickness; i++)
            {
                wallTilemap.SetTile(new Vector3Int(-j, i, 0), wallTile);

                wallTilemap.SetTile(new Vector3Int(mapCellDimensions.x - 1 + j, i, 0), wallTile);

                if(startArch.transform.rotation.eulerAngles.z / 90 % 2 > 0)
                {
                    if(i >= grid1.WorldToCell(startArch.transform.position - startArchBoundExtents).y && 
                       i <  grid1.WorldToCell(startArch.transform.position + startArchBoundExtents).y)
                    {
                        if(startArch.transform.rotation.eulerAngles.z / 90 == 3)
                        {
                            wallTilemap.SetTile(new Vector3Int(-j, i, 0), null);
                        }
                        else
                        {
                            wallTilemap.SetTile(new Vector3Int(mapCellDimensions.y - 1 + j, i, 0), null);
                        }
                    }
                }

                if(endArch.transform.rotation.eulerAngles.z / 90 % 2 > 0)
                {
                    if(i >= grid1.WorldToCell(endArch.transform.position - endArchBoundExtents).y && 
                       i <  grid1.WorldToCell(endArch.transform.position + endArchBoundExtents).y)
                    {
                        if(endArch.transform.rotation.eulerAngles.z / 90 == 3)
                        {
                            wallTilemap.SetTile(new Vector3Int(-j, i, 0), null);
                        }
                        else
                        {
                            wallTilemap.SetTile(new Vector3Int(mapCellDimensions.y - 1 + j, i, 0), null);
                        }
                    }
                }
            }
        }
    }

    void SetCameraBoundary()
    {
        Vector2[] bounds = new Vector2[4] {new Vector2(0 - cameraEdgePadding, 0 - cameraEdgePadding), 
                                           new Vector2(mapDimensions.x + cameraEdgePadding, 0 - cameraEdgePadding), 
                                           new Vector2(mapDimensions.x + cameraEdgePadding, mapDimensions.y + cameraEdgePadding), 
                                           new Vector2(0 - cameraEdgePadding, mapDimensions.y + cameraEdgePadding)};
        cameraBoundary.SetPath(0, bounds);
    }

    void SpawnBuildings()
    {
        Vector3Int mapCellDimensions = grid2.WorldToCell(mapDimensions);
        buildingFlags = new bool[mapCellDimensions.x, mapCellDimensions.y];

        for(int i = 0; i < mapCellDimensions.x; i++)
        {
            for(int j = 0; j < mapCellDimensions.y; j++)
            {
                if(RNG.Next(2) == 1)
                {
                    buildingTilemap.SetTile(new Vector3Int(i, j, 0), buildingTile);
                    buildingFlags[i, j] = true;
                }
            }
        }
    }

    IEnumerator SetAStarGrid()
    {
        AstarData data = AstarPath.active.data;
        GridGraph gg = data.gridGraph;

        int width = (int)mapDimensions.x;
        int depth = (int)mapDimensions.y;
        int nodeSize = 1;

        gg.SetDimensions(width, depth, nodeSize);
        gg.center = new Vector3(mapDimensions.x/2, mapDimensions.y/2, 0);
        
        yield return new WaitForSeconds(0.1f);

        AstarPath.active.Scan();
    }

    void ClearPath()
    {
        // Debug.Log(mainPath.remainingDistance);
    }

    void SpawnRoad()
    {
        Vector3Int mapCellDimensions = grid1.WorldToCell(mapDimensions);
        for(int i = 0; i < mapCellDimensions.x; i++)
        {
            for(int j = 0; j < mapCellDimensions.y; j++)
            {
                if(buildingTilemap.GetTile(buildingTilemap.WorldToCell(roadTilemap.CellToWorld(new Vector3Int(i, j, 0)))) == null)
                {
                    roadTilemap.SetTile(new Vector3Int(i, j, 0), roadTile);
                }
            }
        }
    }

    void parseSaveData(SaveData saveData)
    {
        mapDimensions = new Vector2(saveData.mapDimensions[0], saveData.mapDimensions[1]);

        startArch = Instantiate(arch);
        startArch.SendMessage("ArchEndTriggerStatus", false);
        startArch.transform.position = new Vector3(saveData.startArchOrientation[0, 0],
                                                   saveData.startArchOrientation[0, 1],
                                                   saveData.startArchOrientation[0, 2]);
        startArch.transform.eulerAngles = new Vector3(saveData.startArchOrientation[1, 0],
                                                      saveData.startArchOrientation[1, 1],
                                                      saveData.startArchOrientation[1, 2]);

        endArch = Instantiate(arch);
        endArch.SendMessage("ArchEndTriggerStatus", true);
        endArch.transform.position = new Vector3(saveData.endArchOrientation[0, 0],
                                                 saveData.endArchOrientation[0, 1],
                                                 saveData.endArchOrientation[0, 2]);
        endArch.transform.eulerAngles = new Vector3(saveData.endArchOrientation[1, 0],
                                                    saveData.endArchOrientation[1, 1],
                                                    saveData.endArchOrientation[1, 2]);

        buildingFlags = saveData.buildingFlags;
        Vector3Int mapCellDimensions = grid2.WorldToCell(mapDimensions);

        for(int i = 0; i < mapCellDimensions.x; i++)
        {
            for(int j = 0; j < mapCellDimensions.y; j++)
            {
                if(buildingFlags[i, j])
                {
                    buildingTilemap.SetTile(new Vector3Int(i, j, 0), buildingTile);
                }
            }
        }
    }
}
