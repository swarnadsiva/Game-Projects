using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Range(1, 15)]
    public int width = 10;
    [Range(1, 15)]
    public int height = 15;
    [Range(1, 5)]
    public int borderThickness = 3;
    [Range(1, 5)]
    public int tileDimension = 5;
    [Range(1, 5)]
    public int wallHeight = 3;
    public bool debug = true;

    public GameObject meshPrefab; // Gameobject that will hold the generated meshes
    public Material floorMaterial;
    public Material[] wallMaterials;
    
    public bool Loaded { get => _loaded; }
    private bool _loaded = false;
    private Maze maze;
    // Start is called before the first frame update
    void Awake()
    {
        // make a random maze
        maze = new Maze(width, height, borderThickness, tileDimension, wallHeight, floorMaterial, wallMaterials);

        GameObject newObject;
        Mesh newMesh;
        foreach (MeshSection m in maze.MeshSections)
        {
            newMesh = new Mesh
            {
                name = m.Name,
                vertices = m.Vertices.ToArray(),
                triangles = m.Triangles.ToArray(),
                uv = m.Uvs.ToArray(),
                normals = m.Normals.ToArray()
            };

            newMesh.Optimize();

            newObject = Instantiate(meshPrefab, transform.position, Quaternion.identity);
            newObject.GetComponent<MeshFilter>().mesh = newMesh;
            newObject.GetComponent<MeshCollider>().sharedMesh = newMesh;
            newObject.GetComponent<MeshRenderer>().material = m.MyMaterial;
            newObject.layer = LayerMask.NameToLayer("Floor");
        }

        _loaded = true;

        if (debug)
        {
            Debug.Log(maze.ToString());
            Debug.LogFormat("elapsed time: {0} seconds", Time.fixedTime);
        }
    }

    public Vector3 GetSpawnPoint()
    {
        // find a floor tile on the maze
        return maze.GetRandomTilePositionInWorldSpace();
    }
}
