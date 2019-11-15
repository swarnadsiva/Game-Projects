using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    private List<Vector2> _uvs;
    private List<Vector3> _vertices;
    private List<Vector3> _normals;
    private List<int> _triangles;
    private bool _debug;

    public readonly Mesh myMesh;

    /// <summary>
    /// Default constructor. Initializes mesh and lists.
    /// </summary>
    public MeshGenerator(bool debug = false)
    {
        myMesh = new Mesh();
        _uvs = new List<Vector2>();
        _vertices = new List<Vector3>();
        _normals = new List<Vector3>();
        _triangles = new List<int>();

        _debug = debug;
    }

    public void Generate(Maze newMaze)
    {
        // Do stuff
        // testing only
        if (_debug)
        {
            Debug.Log(newMaze.ToString());
        }

        // use maze

    }

    ///// <summary>
    ///// Generates the vertices for the floor mesh.
    ///// </summary>
    ///// <returns>The vertex count.</returns>
    //int MakeVertices()
    //{
    //    Vector3 pos;

    //    // create vertices for entire grid
    //    for (int z = 0; z < gridSize; z += tileSize)
    //    {
    //        for (int x = 0; x < gridSize; x += tileSize)
    //        {

    //            pos = new Vector3(x, 0, z);
    //            vertices.Add(pos);
    //            if (debug)
    //            {
    //                Instantiate(testVertex, pos, Quaternion.identity);
    //            }
    //        }
    //    }

    //    myMesh.vertices = vertices.ToArray();
    //    return vertices.Count;
    //}

    ///// <summary>
    ///// Generates the triangles of the floor mesh.
    ///// </summary>
    ///// <param name="numVertices">The number of vertices in the mesh.</param>
    //void MakeTriangles(int numVertices)
    //{
    //    int topRight;
    //    int topLeft;
    //    int bottomRight;
    //    int bottomLeft;

    //    //   ^
    //    //   |
    //    //   |
    //    // z |_______>
    //    //      x
    //    // 
    //    // points are ordered from 0, 1, 2, ... in x direction,
    //    // then go up at gridDimension, gridDimension + 1, ...

    //    for (int i = gridDimension; i < numVertices; i += gridDimension) // go through the rows in the grid 
    //    {
    //        for (int j = i; j < i + gridDimension - 1; j++) // go through the columns in the grid
    //        {
    //            topLeft = j;
    //            topRight = j + 1;
    //            bottomLeft = j - gridDimension;
    //            bottomRight = topRight - gridDimension;

    //            // add first triangle
    //            triangles.Add(bottomLeft);
    //            triangles.Add(topLeft);
    //            triangles.Add(bottomRight);

    //            // add second triangle
    //            triangles.Add(topLeft);
    //            triangles.Add(topRight);
    //            triangles.Add(bottomRight);
    //            if (debug)
    //            {
    //                Debug.LogFormat("topLeft: {0} topRight: {1} bottomLeft: {2} bottomRight: {3}", topLeft, topRight, bottomLeft, bottomRight);
    //            }
    //        }
    //    }

    //    myMesh.triangles = triangles.ToArray();

    //}

    ///// <summary>
    ///// Generates the correct UV coordinates to map the texture of the floor accordingly.
    ///// </summary>
    ///// <param name="numVertices">The number of vertices in the mesh.</param>
    //void MakeUVs(int numVertices)
    //{

    //    // in order to make the texture fit on the mesh properly,
    //    // we have to set each point on the grid on a range where
    //    // x and y go from 0 to 1


    //    // find equal divisor for the grid
    //    float incr = 1f / gridDimension;

    //    for (float x = 0f; x < 1f; x += incr)
    //    {
    //        for (float y = 0f; y < 1f; y += incr)
    //        {
    //            uvs.Add(new Vector2(x, y));
    //        }
    //    }

    //    if (debug)
    //    {
    //        Debug.LogFormat("vertices: {0}, uvs: {1}", vertices.Count, uvs.Count);
    //    }
    //    myMesh.uv = uvs.ToArray();
    //}

    ///// <summary>
    ///// Generates the normal vectors for each vertex.
    ///// </summary>
    ///// <param name="numVertices">The number of vertices in the mesh.</param>
    //void MakeNormals(int numVertices)
    //{
    //    for (int i = 0; i < numVertices; i++)
    //    {
    //        normals.Add(Vector3.up);
    //    }
    //}
}
