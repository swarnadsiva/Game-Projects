using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public int levelSize = 10;
    public int tileDimension = 5;
    public int wallHeight = 5;
    public bool debug = true;
    public GameObject testVertex;

    MeshFilter mf;
    MeshCollider mc;
    Mesh myMesh;
    int gridSize;

    // Start is called before the first frame update
    void Awake()
    {

        Maze maze = new Maze(levelSize, tileDimension);

        MeshGenerator mgFloor = new MeshGenerator(wallHeight, testVertex, debug);
        mgFloor.Generate(maze);

        if (debug)
        {
            Debug.LogFormat("elapsed time: {0} seconds", Time.fixedTime);
        }
        //// create components on gameobject
        //mf = gameObject.GetComponent<MeshFilter>();
        //mc = gameObject.GetComponent<MeshCollider>();

        ////gridSize = tileDimension * levelSize;

        //mf.mesh = mgFloor.myMesh;
        //mc.sharedMesh = mgFloor.myMesh;

        //MakeFloor();
    }

    ///// <summary>
    ///// Generates the floor portion of the level.
    ///// </summary>
    //void MakeFloor()
    //{

    //    // create empty floor gameobject in scene
    //    myMesh = new Mesh();
    //    mf.mesh = myMesh;
    //    mc.sharedMesh = myMesh;

    //    // make vertices
    //    int numVertices = MakeVertices();


    //    // make triangles
    //    MakeTriangles(numVertices);

    //    // make UVs
    //    MakeUVs(numVertices);

    //    // make normals
    //    MakeNormals(numVertices);

    //}


}
