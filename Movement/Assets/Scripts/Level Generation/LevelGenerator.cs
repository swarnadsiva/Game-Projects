using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public int levelSize = 10;
    public int tileDimension = 5;
    public int wallHeight = 5;
    public bool debug = true;
    public GameObject testVertexFloor;
    public GameObject testVertexWall;
    public GameObject floorPrefab;
    public GameObject wallPrefab;


    private int gridSize;
    private GameObject floor;
    private GameObject wall;

    // Start is called before the first frame update
    void Awake()
    {

        // set up floor and wall objects
        floor = Instantiate(floorPrefab, transform.position, Quaternion.identity);
        wall = Instantiate(wallPrefab, transform.position, Quaternion.identity);

        // make a random maze
        Maze maze = new Maze(levelSize, tileDimension, wallHeight);

        // make floor mesh
        MakeMesh(maze.GetFloorTiles(), floor, testVertexFloor);

        // make wall mesh
        MakeMesh(maze.GetWallTiles(), wall, testVertexWall);
        
       
        if (debug)
        {
            Debug.Log(maze.ToString());
            Debug.LogFormat("elapsed time: {0} seconds", Time.fixedTime);
        }

    }

    private void MakeMesh(HashSet<MazeTile> mazeTiles, GameObject gameObject, GameObject prefab = null)
    {
        MeshGenerator mg = new MeshGenerator(prefab, debug);
        mg.Generate(mazeTiles);

        gameObject.GetComponent<MeshFilter>().mesh = mg.myMesh;
        gameObject.GetComponent<MeshCollider>().sharedMesh = mg.myMesh;
    }

}
