using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Range(1, 15)]
    public int width = 10;
    [Range(1, 15)]
    public int height = 5;
    [Range(1, 5)]
    public int borderThickness = 1;
    [Range(1, 5)]
    public int tileDimension = 5;
    [Range(1, 5)]
    public int wallHeight = 5;
    public bool debug = true;

    public GameObject meshPrefab; // Gameobject that will hold the generated meshes
    public Material floorMaterial;
    public Material[] wallMaterials;

    
    public bool Loaded { get; }
    private bool _loaded = false;

    // Start is called before the first frame update
    void Awake()
    {

        // make a random maze
        Maze maze = new Maze(width, height, borderThickness, tileDimension, wallHeight, floorMaterial, wallMaterials);

        GameObject newObject;
        Mesh newMesh;
        foreach (MeshSection m in maze.MeshSections)
        {
            newMesh = new Mesh();
            newMesh.name = m.Name;
            newMesh.vertices = m.Vertices.ToArray();
            newMesh.triangles = m.Triangles.ToArray();
            newMesh.uv = m.Uvs.ToArray();
            newMesh.normals = m.Normals.ToArray();

            newMesh.Optimize();

            newObject = Instantiate(meshPrefab, transform.position, Quaternion.identity);
            newObject.GetComponent<MeshFilter>().mesh = newMesh;
            newObject.GetComponent<MeshCollider>().sharedMesh = newMesh;
            newObject.GetComponent<MeshRenderer>().material = m.MyMaterial;
        }

        _loaded = true;

        if (debug)
        {
            Debug.Log(maze.ToString());
            Debug.LogFormat("elapsed time: {0} seconds", Time.fixedTime);
        }

        //// TESTING!
        //Vector3[] vertices = {
        //    new Vector3(0, 0, 0), // 0
        //    new Vector3(0, 0, 5), // 1
        //    new Vector3(0, 0, 10), // 2
        //    new Vector3(5, 0, 0), // 3
        //    new Vector3(10, 0, 0), // 4
        //    new Vector3(10, 0, 5), // 5
        //    new Vector3(5, 0, 5), // 6
        //    new Vector3(10, 0, 10), // 7
        //    new Vector3(5, 0, 10), // 8
        //    new Vector3(5, 0, 10), // 8
        //    new Vector3(5, 0, 10) // 8
        //};

        //int[] triangles =
        //{
        //    3, 0, 6,
        //    1, 2, 8,
        //    6, 0, 3,
        //    8, 2, 1
        //};

        //Vector2[] uvs =
        //{

        //};

        //Mesh myMesh = new Mesh();

        //myMesh.vertices = vertices;
        //myMesh.triangles = triangles;

        //myMesh.Optimize();
        //Debug.LogFormat("original num vertices: {0}, after optimizing: {1}", vertices.Length, myMesh.vertices.Length);
        //Debug.LogFormat("original num triangles: {0}, after optimizing: {1}", triangles.Length, myMesh.triangles.Length);

        //GetComponent<MeshFilter>().mesh = myMesh;
        //GetComponent<MeshCollider>().sharedMesh = myMesh;


    }

}
