using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSection
{
    public List<Vector3> Vertices { get; }
    public List<Vector3> Normals { get; }
    public List<Vector2> Uvs { get; }
    public List<int> Triangles { get; }

    private int _tileSize;
    private int _wallHeight;

    public MeshSection(int tileSize, int wallHeight = 0)
    {
        Vertices = new List<Vector3>();
        Normals = new List<Vector3>();
        Uvs = new List<Vector2>();
        Triangles = new List<int>();

        _tileSize = tileSize;
        _wallHeight = wallHeight;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newTile">Tile to add.</param>
    public void AddTile(AbstractTile newTile)
    {

        int bottom, top, left, right;
        // treat tile x and y position as bottom left world space position
        bottom = newTile.Y * _tileSize;
        top = (newTile.Y + 1) * _tileSize;
        left = newTile.X * _tileSize;
        right = (newTile.X + 1) * _tileSize;

        // always add in clockwise order: BL, TL, TR, BR
        int start = Vertices.Count;

        Vertices.Add(new Vector3(left, _wallHeight, bottom));
        Vertices.Add(new Vector3(left, _wallHeight, top));
        Vertices.Add(new Vector3(right, _wallHeight, top));
        Vertices.Add(new Vector3(right, _wallHeight, bottom));

        AddTriangles(start);
        AddNormals(Vector3.up);
        AddUVs();

        if (_wallHeight > 0)
        {
            // need to add corresponding bottom vertices, uvs, normals and triangles

            // add triangles starting from left facing to front, to right, to back facing
            // TODO optimize so we aren't adding repeat triangles!
            // LEFT quad
            start = Vertices.Count;
            Vertices.Add(new Vector3(left, 0, top));
            Vertices.Add(new Vector3(left, _wallHeight, top));
            Vertices.Add(new Vector3(left, _wallHeight, bottom));
            Vertices.Add(new Vector3(left, 0, bottom));

            AddTriangles(start);
            AddNormals(Vector3.left);
            AddUVs();

            // FRONT quad
            start = Vertices.Count;
            Vertices.Add(new Vector3(right, 0, top));
            Vertices.Add(new Vector3(right, _wallHeight, top));
            Vertices.Add(new Vector3(left, _wallHeight, top));
            Vertices.Add(new Vector3(left, 0, top));

            AddTriangles(start);
            AddNormals(Vector3.forward);
            AddUVs();

            // RIGHT quad
            start = Vertices.Count;
            Vertices.Add(new Vector3(right, 0, bottom));
            Vertices.Add(new Vector3(right, _wallHeight, bottom));
            Vertices.Add(new Vector3(right, _wallHeight, top));
            Vertices.Add(new Vector3(right, 0, top));

            AddTriangles(start);
            AddNormals(Vector3.right);
            AddUVs();

            // BACK quad
            start = Vertices.Count;
            Vertices.Add(new Vector3(left, 0, bottom));
            Vertices.Add(new Vector3(left, _wallHeight, bottom));
            Vertices.Add(new Vector3(right, _wallHeight, bottom));
            Vertices.Add(new Vector3(right, 0, bottom));

            AddTriangles(start);
            AddNormals(Vector3.back);
            AddUVs();
        }
    }

    private void AddTriangles(int start)
    {
        // add both triangles of this quad
        Triangles.Add(start);
        Triangles.Add(start + 1);
        Triangles.Add(start + 3);

        Triangles.Add(start + 1);
        Triangles.Add(start + 2);
        Triangles.Add(start + 3);
    }

    private void AddUVs()
    {
        Uvs.Add(new Vector2(0, 0));
        Uvs.Add(new Vector2(0, 1));
        Uvs.Add(new Vector2(1, 1));
        Uvs.Add(new Vector2(1, 0));
    }

    private void AddNormals(Vector3 direction)
    {
        for (int i = 0; i < 4; i++)
        {
            Normals.Add(direction);
        }
    }
}
