using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeTile
{

    #region Public
    public enum AttributeType { Wall, Floor, Edge };
    public HashSet<AttributeType> _attributes;
    public readonly Vector2Int position; // this is the BOTTOM LEFT coordinate of the tile on a grid
    public HashSet<Vector3> _vertexPositions3D;
    public List<int[]> _triangleIndices;
    #endregion

    private readonly int _tileSize;
    private readonly int _wallHeight;

    /// <summary>
    /// Creates a new MazeTile object with a set position.
    /// </summary>
    /// <param name="x">x-coordinate value.</param>
    /// <param name="y">y-coordinate value.</param>
    public MazeTile(int x, int y, HashSet<AttributeType> attributes, int tileSize, int wallHeight = 0)
    {
        position = new Vector2Int(x, y);
        _attributes = attributes;
        _vertexPositions3D = new HashSet<Vector3>();
        _triangleIndices = new List<int[]>();
        _tileSize = tileSize;
        _wallHeight = wallHeight;

        Calculate3DVertexPositions();
        CalculateTriangles();
    }

    /// <summary>
    /// Calculates and stores the 3D vertex positions in world space of the maze tile.
    /// </summary>
    /// <param name="tileSize">Tile dimension in world space.</param>
    /// <param name="wallHeight">Wall height in world space.</param>
    private void Calculate3DVertexPositions()
    {
        _vertexPositions3D.Clear();

        //   ^
        //   |
        //   |
        // z |_______>
        //      x
        // 
        // points are ordered from 0, 1, 2, ... in x direction,
        // then go up by tileSize, and over by tileSize ...

        int bottom = position.y * _tileSize;
        int top = (position.y + 1) * _tileSize;
        int left = position.x * _tileSize;
        int right = (position.x + 1) * _tileSize;

        // z-axis is vertical, x-axis is horizontal in the world space
        // if floor, get four points from floor
        // ORDER OF ADD OPERATIONS IS IMPORTANT
        
        _vertexPositions3D.Add(new Vector3(left, 0, bottom)); // bottom left 0
        _vertexPositions3D.Add(new Vector3(left, 0, top)); // top left 1
        _vertexPositions3D.Add(new Vector3(right, 0, top)); // top right 2
        _vertexPositions3D.Add(new Vector3(right, 0, bottom)); // bottom right 3 

        // if wall, get four additional points with wallHeight
        if (_attributes.Contains(AttributeType.Wall))
        {
            _vertexPositions3D.Add(new Vector3(left, _wallHeight, bottom)); // bottom left 4
            _vertexPositions3D.Add(new Vector3(left, _wallHeight, top)); // top left 5
            _vertexPositions3D.Add(new Vector3(right, _wallHeight, top)); // top right 6
            _vertexPositions3D.Add(new Vector3(right, _wallHeight, bottom)); // bottom right 7
        }
    }


    private void CalculateTriangles()
    {
        // add all the triangles for this tile
        _triangleIndices.Clear();

        if (_attributes.Contains(AttributeType.Floor))
        {
            _triangleIndices.Add(new int[] { 0, 1, 3 }); // bottom left triangle
            _triangleIndices.Add(new int[] { 1, 2, 3 }); // top right triangle
        }else if (_attributes.Contains(AttributeType.Wall))
        {
            
            _triangleIndices.Add(new int[] { 4, 5, 7 }); // bottom left triangle
            _triangleIndices.Add(new int[] { 5, 6, 7 }); // top right triangle

            // connect top vertices to bottom vertices
            _triangleIndices.Add(new int[] { 0, 4, 3 }); // bottom left triangle
            _triangleIndices.Add(new int[] { 4, 7, 3 }); // top right triangle
            _triangleIndices.Add(new int[] { 3, 7, 2 }); // bottom left triangle
            _triangleIndices.Add(new int[] { 7, 6, 2 }); // top right triangle
            _triangleIndices.Add(new int[] { 2, 6, 1 }); // bottom left triangle
            _triangleIndices.Add(new int[] { 6, 5, 1 }); // top right triangle
            _triangleIndices.Add(new int[] { 1, 5, 0 }); // bottom left triangle
            _triangleIndices.Add(new int[] { 5, 4, 0 }); // top right triangle
        }
    }

    /// <summary>
    /// Determines the distance from this MazeTile to the other MazeTile.
    /// </summary>
    /// <param name="other">the other MazeTile.</param>
    /// <returns>Distance between MazeTiles as a float.</returns>
    public float DistanceFrom(MazeTile other)
    {
        return Vector2.Distance(position, other.position);
    }

    public override string ToString()
    {
        if (_attributes.Contains(AttributeType.Wall))
        {
            return "X";
        }
        else if (_attributes.Contains(AttributeType.Floor))
        {
            return "O";
        }
        else
        {
            return ""; // unassigned
        }
    }

    /// <summary>
    /// Determines if this tile has a surrounding floor tile.
    /// </summary>
    /// <param name="surrounding">MazeTile list of surrounding tiles.</param>
    /// <param name="nearestWall">Out parameter that will contain the nearest wall coordinates or this tiles coordinates if there are none.</param>
    /// <returns>True if a surrounding tile is reachable, false if not.</returns>
    public bool IsReachable(List<MazeTile> surrounding, out Vector2Int nearestWall)
    {
        bool reachable = false;
        nearestWall = position;
        if (!_attributes.Contains(AttributeType.Floor))
        {
            throw new System.Exception("MazeTile instance must have AttributeType.Floor");
        }

        int i = 0;
        while (i < surrounding.Count && !reachable)
        {
            // check if this is a floor
            if (surrounding[i]._attributes.Contains(AttributeType.Floor))
            {
                reachable = true;
            }
            else if (surrounding[i]._attributes.Contains(AttributeType.Wall)) // this is a wall
            {
                // only set nearest wall if it has not already been set
                if (nearestWall == position)
                {
                    nearestWall = new Vector2Int(surrounding[i].position.x, surrounding[i].position.y);
                }

                // increment index
                i++;
            }
        }

        return reachable;
    }

    /// <summary>
    /// Retrieves a list of Vector2 coordinates of surrounding tiles.
    /// Possible surrounding tiles include top, bottom, left and right (not diagonal).
    /// </summary>
    /// <param name="mazeSize">Size of the maze.</param>
    /// <returns>Vector2 list of surrounding tile coordinates.</returns>
    public List<Vector2Int> GetUntraversedSurroundingPositions(int mazeSize, HashSet<Vector2Int> traversed)
    {
        // check top, bottom, left and right
        List<Vector2Int> surrounding = new List<Vector2Int>();

        if (position.x > 0)
        {
            // add left
            AddIfUntraversed(new Vector2Int(position.x - 1, position.y), traversed, surrounding);
        }

        if (position.x < mazeSize - 1)
        {
            // add right
            AddIfUntraversed(new Vector2Int(position.x + 1, position.y), traversed, surrounding);
        }

        if (position.y > 0)
        {
            // add top
            AddIfUntraversed(new Vector2Int(position.x, position.y - 1), traversed, surrounding);
        }

        if (position.y < mazeSize - 1)
        {
            // add bottom
            AddIfUntraversed(new Vector2Int(position.x, position.y + 1), traversed, surrounding);
        }

       
        return surrounding;
    }

    /// <summary>
    /// Adds the specified coordinate to the list if it is not present 
    /// in the hashset.
    /// </summary>
    /// <param name="newCoordinate">Vector2Int tile coordinate to add.</param>
    /// <param name="traversed">HashSet of Vector2Int traversed tile coordinates.</param>
    /// <param name="surrounding">Vector2Int List of surrounding tile coordinates.</param>
    private void AddIfUntraversed(Vector2Int newCoordinate, HashSet<Vector2Int> traversed, List<Vector2Int> surrounding)
    {
        if (!traversed.Contains(newCoordinate))
        {
            surrounding.Add(newCoordinate);
        }
    }

    /// <summary>
    /// Changes the attributes of this tile to have AttributeType.Floor
    /// while ensuring that it no longer has AttributeType.Wall.
    /// </summary>
    public void ChangeToFloor()
    {
        _attributes.Remove(AttributeType.Wall);
        _attributes.Add(AttributeType.Floor);

        Calculate3DVertexPositions();
        CalculateTriangles();
    }

  
    public void FindNeighbors(HashSet<MazeTile> section, List<MazeTile> remainingTiles)
    {
        // find tiles next to this in remaining
        // TODO 
        // remainingTiles.Find(x => (x.position.Equals);
    }
}
