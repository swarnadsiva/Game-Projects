using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public int gridDimension = 10;
    public int tileSize = 5;
    public bool debug = true;
    public GameObject testVertex;

    MeshFilter mf;
    MeshCollider mc;
    Mesh myMesh;
    int gridSize;

    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;
    List<Vector3> normals;

    // Start is called before the first frame update
    void Awake()
    {
        MakeLists();

        // create components on gameobject
        mf = gameObject.GetComponent<MeshFilter>();
        mc = gameObject.GetComponent<MeshCollider>();

        gridSize = tileSize * gridDimension;

        MakeFloor();
    }



    void MakeLists()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        normals = new List<Vector3>();
    }

    void MakeFloor()
    {

        // create empty floor gameobject in scene

        myMesh = new Mesh();
        mf.mesh = myMesh;
        mc.sharedMesh = myMesh;

        // make vertices
        int numVertices = MakeVertices();


        // make triangles
        MakeTriangles(numVertices);

        // make UVs
        //MakeUVs(numVertices);

        // make normals
        //MakeNormals(numVertices);

    }

    int MakeVertices()
    {
        Vector3 pos;
        // create vertices for entire grid
        for (int z = 0; z < gridSize; z += tileSize)
        {
            for (int x = 0; x < gridSize; x += tileSize)
            {
                pos = new Vector3(x, 0, z);
                vertices.Add(pos);
                if (debug)
                {
                    Instantiate(testVertex, pos, Quaternion.identity);
                }
            }
        }

        myMesh.vertices = vertices.ToArray();
        return vertices.Count;
    }


    void MakeTriangles(int numVertices)
    {
        int topRight;
        int topLeft;
        int bottomRight;
        int bottomLeft;


        for (int i = gridDimension; i < numVertices; i += gridDimension) // go through the rows in the grid 
        {
            for (int j = i; j < i + gridDimension - 1; j++) // go through the columns in the grid
            {
                topLeft = j;
                topRight = j + 1;
                bottomLeft = j - gridDimension;
                bottomRight = topRight - gridDimension;

                // add first triangle
                triangles.Add(bottomLeft);
                triangles.Add(topLeft);
                triangles.Add(bottomRight);

                // add second triangle
                triangles.Add(topLeft);
                triangles.Add(topRight);
                triangles.Add(bottomRight);
                if (debug)
                {
                    Debug.LogFormat("topLeft: {0} topRight: {1} bottomLeft: {2} bottomRight: {3}", topLeft, topRight, bottomLeft, bottomRight);
                }
            }
        }

        myMesh.triangles = triangles.ToArray();

    }

    // TODO fix ordering of how we are adding UVs here
    void MakeUVs(int numVertices)
    {
        Vector2 uvTopRight = new Vector2(0, 1);
        Vector2 uvtopLeft = new Vector2(1, 1);
        Vector2 uvBottomRight = new Vector2(1, 0);
        Vector2 uvBottomLeft = new Vector2(0, 0);

        //   ^
        //   |
        //   |
        // z |_______>
        //      x
        // 
        // points are ordered from 0, 1, 2, ... in x direction,
        // then go up at gridDimension, gridDimension + 1, ...

        // add all bottom UVs in order
        for (int i = 0; i < numVertices; i += gridDimension)
        {
            for (int j = i; j < gridDimension - 1; j++)
            {
                uvs.Add(uvBottomLeft);
                uvs.Add(uvBottomRight);
            }
        }

        // now add all top UVs in correct order
        for (int i = gridDimension; i < numVertices - gridDimension; i += gridDimension)
        {
            for (int j = i; j < gridDimension - 1; j++)
            {
                uvs.Add(uvtopLeft);
                uvs.Add(uvTopRight);
            }
        }

        myMesh.uv = uvs.ToArray();
    }


    void MakeNormals(int numVertices)
    {
        for (int i = 0; i < numVertices; i++)
        {
            normals.Add(Vector3.up);
        }
    }
}
