using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Maze
{

    #region Public
    public readonly int myWidth = 0;
    public readonly int myHeight = 0;
    public readonly int myTileSize = 0;
    public readonly int myWallHeight = 0;
    #endregion

    #region Private

    private int _borderThickness = 1;
    private AbstractTile[,] _tiles;

    private MeshSection _floor;
    private MeshSection _boundary;
    private MeshSection _internalWalls;
    //private int numFloorTiles = 0;
    
    public List<MeshSection> MeshSections { get; }

    #endregion

    public Maze(int width, int height, int borderThickness, int tileSize, int wallHeight)
    {
        // set maze size to double plus size of borders
        _borderThickness = borderThickness;
        myWidth = width + (width - 1) + (2 * _borderThickness);
        myHeight = height + (height - 1) + (2 * _borderThickness);
        myTileSize = tileSize;
        myWallHeight = wallHeight;

        // initialize mesh sections
        MeshSections = new List<MeshSection>();

        // initialize 
        _tiles = new AbstractTile[myWidth, myHeight];
        _boundary = new MeshSection(myTileSize, myWallHeight);
        InitializeTiles();

        // make floor path & mesh section
        int startX = _borderThickness;
        int startY = _borderThickness;

        _floor = GeneratePath<FloorTile>(_tiles[startX, startY]);
        _internalWalls = new MeshSection(myTileSize, myWallHeight);

        // TODO optimize iterate through tiles and make all empty tiles walls
        MakeEmptySpotsWalls();


        MeshSections.Add(_floor);
        MeshSections.Add(_boundary);
        MeshSections.Add(_internalWalls);
    }

    /// <summary>
    /// Initializes tiles to default setup values.
    /// </summary>
    private void InitializeTiles()
    {
        int nextXFloor = _borderThickness;
        int nextYFloor = _borderThickness;

        // make wall section

        // initialize _tiles
        FloorTile current;
        for (int x = 0; x < myWidth; x++)
        {
            for (int y = 0; y < myHeight; y++)
            {
                if (x == nextXFloor && y == nextYFloor && x < myWidth - _borderThickness && y < myHeight - _borderThickness)
                {
                    // add a floor tile
                    current = new FloorTile(x, y);
                    _tiles[x, y] = current;

                    // connect floor tiles
                    if (x >= _borderThickness)
                    {
                        // connect to x - 2
                        current.AddNeighbor(_tiles[x - 2, y]);
                        // connect x - 2 to this
                        _tiles[x - 2, y].AddNeighbor(current);
                    }

                    if (y >= _borderThickness)
                    {
                        // connect to y - 2
                        current.AddNeighbor(_tiles[x, y - 2]);

                        // connect y - 2 to this
                        _tiles[x, y - 2].AddNeighbor(current);
                    }

                    nextYFloor += 2;
                }
                else if (x < _borderThickness || x >= myWidth - _borderThickness || y < _borderThickness || y >= myHeight - _borderThickness)
                {
                    // this is a boundary wall
                    _tiles[x, y] = new WallTile(x, y);
                    _boundary.AddTile(_tiles[x, y]);
                }
                else
                {
                    // add an empty tile for now
                    _tiles[x, y] = new EmptyTile(x, y);
                }
            }

            // reset next y that will be a floor tile
            nextYFloor = _borderThickness;

            if (x == nextXFloor) { nextXFloor += 2; }
        }

    }


    private void DepthFirstTraversal<T>(AbstractTile current, MeshSection myMesh)
    {
        // pick random number
        int rand = Random.Range(0, current.NumNeighbors);
        int next = rand;

        // set is visited to true
        current.IsVisited = true;

        // add tile to mesh
        myMesh.AddTile(current);

        for (int i = 0; i < current.NumNeighbors; i++)
        {
            // get the next neighbor
            AbstractTile neighbor = current.GetNeighborAt(next);
            if (!neighbor.IsVisited && neighbor is T)
            {
                // get tile in between to add to mesh
                int deltaX = neighbor.X - current.X;
                int deltaY = neighbor.Y - current.Y;

                (int x, int y) pos = GetFloorPosition(deltaX, deltaY, current);

                _tiles[pos.x, pos.y] = new FloorTile(pos.x, pos.y);
                myMesh.AddTile(_tiles[pos.x, pos.y]);

                // call DFT with neighbor as current
                DepthFirstTraversal<T>(neighbor, myMesh);
            }

            // TODO add random neighbor as part of the mesh to add variety

            // go to next neighbor if visited 
            next = (next + 1) % current.NumNeighbors;
        }
    }

    private (int x, int y) GetFloorPosition(int deltaX, int deltaY, AbstractTile current)
    {
        if (deltaX > 0)
        {
            // add one more right
            deltaX = current.X + 1;
            deltaY = current.Y;

        }
        else if (deltaX < 0)
        {
            // add one more left
            deltaX = current.X - 1;
            deltaY = current.Y;
        }
        else if (deltaY > 0)
        {
            // add one more top
            deltaX = current.X;
            deltaY = current.Y + 1;
        }
        else if (deltaY < 0)
        {
            // add one more bottom
            deltaX = current.X;
            deltaY = current.Y - 1;
        }
        else
        {
            Utilities.LogException("Neighboring tile is not in a correct spot!");
        }

        return (x: deltaX, y: deltaY);
    }

    public MeshSection GeneratePath<T>(AbstractTile root)
    {
        // create new mesh section
        MeshSection myMesh = new MeshSection(myTileSize);

        // traverse with the root
        DepthFirstTraversal<FloorTile>(root, myMesh);

        return myMesh;
    }

    private void MakeEmptySpotsWalls()
    {
        for (int x = 0; x < myWidth; x++)
        {
            for (int y = 0; y < myHeight; y++)
            {
                if (_tiles[x, y] is EmptyTile)
                {
                    _tiles[x, y] = new WallTile(x, y);
                    _internalWalls.AddTile((WallTile)_tiles[x, y]);
                }
            }
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        Debug.LogFormat("totalTiles {0}", _tiles.Length);
        // ensure the maze is as it appears with z-axis vertical, x-axis horizontal
        // when looking top down and axis starts on bottom left
        for (int x = 0; x < myWidth; x++)
        {
            for (int z = 0; z < myHeight; z++)
            {
                sb.Append(_tiles[x, z].ToString());
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }

    //    /// <summary>
    //    /// Creates a new Maze object of the specified size.
    //    /// </summary>
    //    /// <param name="mSize"></param>
    //    public Maze(int mSize, int tSize, int wHeight, int wallProbability = 3)
    //    {

    //        _tiles = new MazeTile[mSize, mSize];
    //        _wallProbability = wallProbability;
    //        _floorTiles = new HashSet<MazeTile>();
    //        _wallTiles = new HashSet<MazeTile>();

    //        tileSize = tSize;
    //        mazeSize = mSize;
    //        wallHeight = wHeight;
    //        totalTiles = mSize * mSize;
    //        Generate();
    //    }

    //    public void Generate()
    //    {
    //        // ensure these are clear
    //        _floorTiles.Clear();
    //        _wallTiles.Clear();

    //        // make random tiles
    //        Randomize();

    //        // ensure tiles are reachable
    //        MakeReachable();

    //    }

    //    private void Randomize()
    //    {
    //        MazeTile newTile;
    //        int rand;

    //        // we are looking at our maze top down, z-axis is vertical, x-axis is horizonatal
    //        for (int x = 0; x < mazeSize; x++)
    //        {
    //            for (int z = 0; z < mazeSize; z++)
    //            {
    //                HashSet<MazeTile.AttributeType> attributes = new HashSet<MazeTile.AttributeType>();

    //                // check if edge 
    //                if (x == 0 || x == mazeSize - 1 || z == 0 || z == mazeSize - 1)
    //                {
    //                    attributes.Add(MazeTile.AttributeType.Edge);
    //                }

    //                rand = Random.Range(0, _wallProbability);
    //                if (rand == 0) // 1 out of _wallProbability to be a wall
    //                {
    //                    // make this tile a wall
    //                    attributes.Add(MazeTile.AttributeType.Wall);
    //                } else
    //                {
    //                    // make this a floor
    //                    attributes.Add(MazeTile.AttributeType.Floor);
    //                }

    //                newTile = new MazeTile(x, z, attributes, tileSize, wallHeight);
    //                _tiles[x , z] = newTile;
    //            }
    //        }
    //    }

    //    private void MakeReachable()
    //    {
    //        List<Vector2Int> surroundingPos = new List<Vector2Int>();
    //        List<MazeTile> surroundingTiles = new List<MazeTile>();
    //        HashSet<Vector2Int> traversedTilePos = new HashSet<Vector2Int>();

    //        MazeTile current;
    //        //MazeTile nearestWall;
    //        Vector2Int nearestWallPos;

    //        // iterate through each MazeTile, z-axis is vertical, x-axis is horizonatal
    //        for (int x = 0; x < mazeSize; x++)
    //        {
    //            for (int z = 0; z < mazeSize; z++)
    //            {
    //                current = _tiles[x, z];

    //                // only make floor tiles reachable
    //                if (current._attributes.Contains(MazeTile.AttributeType.Floor))
    //                {
    //                    // add current to traversed (and to floorTiles)
    //                    traversedTilePos.Add(current.position);
    //                    _floorTiles.Add(current);

    //                    // get surrounding tiles
    //                    surroundingPos = current.GetUntraversedSurroundingPositions(mazeSize, traversedTilePos);
    //                    surroundingTiles.Clear();

    //                    foreach (Vector2Int pos in surroundingPos)
    //                    {
    //                        surroundingTiles.Add(_tiles[pos.x, pos.y]);
    //                    }


    //                    if (!current.IsReachable(surroundingTiles, out nearestWallPos))
    //                    {
    //                        // change nearest wall to floor
    //                        //nearestWall = _tiles[nearestWallPos.x, nearestWallPos.y];
    //                        _tiles[nearestWallPos.x, nearestWallPos.y].ChangeToFloor();

    //                        // remove from wall tiles
    //                        _wallTiles.Remove(_tiles[nearestWallPos.x, nearestWallPos.y]);
    //                        _floorTiles.Add(_tiles[nearestWallPos.x, nearestWallPos.y]);
    //                    }
    //                } else if (current._attributes.Contains(MazeTile.AttributeType.Wall))
    //                {
    //                    // add to wall tiles
    //                    _wallTiles.Add(current);
    //                }
    //            }
    //        }
    //    }

    //    public HashSet<MazeTile> GetFloorTiles()
    //    {
    //        return _floorTiles;
    //    }

    //    public HashSet<MazeTile> GetWallTiles()
    //    {
    //        return _wallTiles;
    //    }


    //    public List<MazeTile[]> GetTileSections(List<MazeTile> allTiles)
    //    {
    //        List<MazeTile[]> tileSections = new List<MazeTile[]>();
    //        List<MazeTile> remainingTiles = new List<MazeTile>();

    //        // copy all tiles to remaining tiles
    //        foreach (MazeTile tile in allTiles)
    //        {
    //            remainingTiles.Add(tile); // deep copy
    //        }


    //        while (remainingTiles.Count > 0)
    //        {
    //            // create a hashset for this section
    //            HashSet<MazeTile> section = new HashSet<MazeTile>();

    //            // start with first tile
    //            MazeTile tile = remainingTiles[0];
    //            section.Add(tile);

    //            tile.FindNeighbors(section, remainingTiles);
    //        }

    //        return tileSections;
    //    }

}
