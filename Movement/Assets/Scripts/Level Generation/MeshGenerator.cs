using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    private MazeTile[] _mazeTiles = { };


    private HashSet<Vector2> _uvs;
    private HashSet<Vector3> _vertices;
    private List<Vector3> _normals;
    private List<int> _triangles;

    private bool _debug;
    private GameObject _testVertex;
  

    public readonly Mesh myMesh;

    /// <summary>
    /// Default constructor. Initializes mesh and lists.
    /// </summary>
    public MeshGenerator( GameObject testVertex = null, bool debug = false)
    {
        myMesh = new Mesh();
        _uvs = new HashSet<Vector2>();
        _vertices = new HashSet<Vector3>();
        _normals = new List<Vector3>();
        _triangles = new List<int>();

        _debug = debug;
        _testVertex = testVertex;
    }

    public void Generate(HashSet<MazeTile> mazeTiles)
    {
        int size = mazeTiles.Count;
        _mazeTiles = new MazeTile[size];
        mazeTiles.CopyTo(_mazeTiles);

        // use maze to make vertices
        MakeVertices();

        MakeTriangles();

    }

    /// <summary>
    /// Generates the vertices for the floor mesh.
    /// </summary>
    /// <returns>The vertex count.</returns>
    void MakeVertices()
    {

        // create vertices for entire grid
        for (int i = 0; i < _mazeTiles.Length; i++)
        {
            foreach (Vector3 vertex in _mazeTiles[i]._vertexPositions3D) 
            {
                if (_debug && !_vertices.Contains(vertex))
                {
                    Object.Instantiate(_testVertex, vertex, Quaternion.identity);
                }
                _vertices.Add(vertex); // only adds if it doesn't already contain it
            }
        }

        Vector3[] tempV = new Vector3[_vertices.Count];
        _vertices.CopyTo(tempV);

        myMesh.vertices = tempV;
    }

    /// <summary>
    /// Generates the triangles of the mesh.
    /// </summary>
    void MakeTriangles()
    {
        for (int i = 0; i < _mazeTiles.Length; i++)
        {
            List<int[]> tileTriangle = _mazeTiles[i]._triangleIndices;

            for (int j = 0; j < tileTriangle.Count; j++)
            {
                int[] triangle = tileTriangle[j];
                _triangles.Add(triangle[0]);
                _triangles.Add(triangle[1]);
                _triangles.Add(triangle[2]);
            }
        }

        myMesh.triangles = _triangles.ToArray();
        if (_debug)
        {
            Debug.LogFormat("triangle vertices in mesh {0}", myMesh.triangles.Length);
        }
    }

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
