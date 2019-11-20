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
    public List<MeshSection> MeshSections { get; }
    #endregion

    #region Private
    private int _borderThickness = 1;
    private AbstractTile[,] _tiles;

    private MeshSection _floor;
    private MeshSection _border;
    private MeshSection _internalWalls;
    #endregion

    public Maze(int width, int height, int borderThickness, int tileSize, int wallHeight, Material floorMat, Material[] wallMats)
    {
        // set maze size to double plus size of borders
        _borderThickness = borderThickness;
        myWidth = width + (width - 1) + (2 * _borderThickness);
        myHeight = height + (height - 1) + (2 * _borderThickness);
        myTileSize = tileSize;
        myWallHeight = wallHeight;

        // initialize mesh sections
        MeshSections = new List<MeshSection>();
        _floor = new MeshSection(myTileSize, floorMat, "Floor");
        _border = new MeshSection(myTileSize, wallMats[0], "Border Walls", myWallHeight);
        _internalWalls = new MeshSection(myTileSize, wallMats[1], "Internal Walls", myWallHeight);

        // initialize tile array
        _tiles = new AbstractTile[myWidth, myHeight];
        InitializeTiles();

        // set neighbors
        SetNeighbors();

        // make floor path & mesh section and add to list
        GeneratePath<FloorTile>(_tiles[_borderThickness, _borderThickness], _floor, true);
        MeshSections.Add(_floor);

        // make border wall path and add to list
        GeneratePath<WallTile>(_tiles[0, 0], _border);
        MeshSections.Add(_border);

        // lastly, make the internal walls and add to the list
        MakeInternalWallSections();
        MeshSections.Add(_internalWalls);
    }

    /// <summary>
    /// Initializes tiles to default setup values.
    /// </summary>
    private void InitializeTiles()
    {
        // next x and y floor tiles will start after the specified
        // border wall thickness
        int nextXFloor = _borderThickness;
        int nextYFloor = _borderThickness;


        // initialize _tiles
        AbstractTile current;
        for (int x = 0; x < myWidth; x++)
        {
            for (int y = 0; y < myHeight; y++)
            {
                // check if this should be a floor tile 
                if (x == nextXFloor && y == nextYFloor && x < myWidth - _borderThickness && y < myHeight - _borderThickness)
                {
                    // add a floor tile
                    FloorTile newFloor = new FloorTile(x, y);

                    // connect floor tile neighbors that are 1 over 
                    // so we can traverse later to find a path
                    if (x >= _borderThickness && _tiles[x - 2, y] is FloorTile)
                    {
                        // connect to x - 2
                        newFloor.AddFloorNeighbor(AbstractTile.Position.Left, (FloorTile)_tiles[x - 2, y]);
                        // connect x - 2 to this
                        ((FloorTile)_tiles[x - 2, y]).AddFloorNeighbor(AbstractTile.Position.Right, newFloor);
                    }
                    if (y >= _borderThickness && _tiles[x, y - 2] is FloorTile)
                    {
                        // connect to y - 2
                        newFloor.AddFloorNeighbor(AbstractTile.Position.Top, (FloorTile)_tiles[x, y - 2]);
                        // connect y - 2 to this
                        ((FloorTile)_tiles[x, y - 2]).AddFloorNeighbor(AbstractTile.Position.Bottom, newFloor);
                    }

                    current = newFloor;

                    // increment next expected floor tile
                    nextYFloor += 2;
                }
                else if (x < _borderThickness || x >= myWidth - _borderThickness || y < _borderThickness || y >= myHeight - _borderThickness)
                {
                    // this is a border wall tile
                    current = new WallTile(x, y);
                }
                else
                {
                    // add an empty tile for now
                    current = new EmptyTile(x, y);
                }

                // set the current tile to this position
                _tiles[x, y] = current;
            }

            // reset next y that will be a floor tile
            nextYFloor = _borderThickness;

            // increment next x that will be a floor tile
            if (x == nextXFloor) { nextXFloor += 2; }
        }
    }

    private void SetNeighbors()
    {
        // set neighbors
        // TODO put this in the first loop iteration so we only loop once
        for (int x = 0; x < myWidth; x++)
        {
            for (int y = 0; y < myHeight; y++)
            {
                _tiles[x, y].SetNeighborsFromMaze(this);
            }
        }
    }

    private void MakeInternalWallSections()
    {
        for(int x = _borderThickness; x < myWidth - _borderThickness - 1; x++)
        {
            for (int y = _borderThickness; y < myHeight - _borderThickness - 1; y++)
            {
                if (_tiles[x,y].IsEmpty() && !_tiles[x,y].IsVisited)
                {
                    // generate a path and add it to the section
                    GeneratePath<EmptyTile>(_tiles[x,y], _internalWalls);
                }
            }
        }
    }

    private void GeneratePath<T>(AbstractTile root, MeshSection myMesh, bool passMaze = false)
    {
        if (passMaze)
        {
            root.DepthFirstTraversal<T>(myMesh, this);
        } else
        {
            root.DepthFirstTraversal<T>(myMesh);
        }
    }

    public AbstractTile GetTileAt(int x, int y)
    {
        return _tiles[x, y];
    }

    public void SetTileAt(int x, int y, AbstractTile newTile)
    {
        _tiles[x, y] = newTile;
    }

    public Vector3 GetRandomTilePositionInWorldSpace()
    {
        int randX = 0;
        int randY = 0;
        while (!(_tiles[randX, randY] is FloorTile))
        {
            randX = Random.Range(_borderThickness, myWidth - _borderThickness);
            randY = Random.Range(_borderThickness, myHeight - _borderThickness);
        }
        return new Vector3(randX * myTileSize + myTileSize / 2, 0f, randY * myTileSize + myTileSize / 2); // get the center of this tile in world space coordinates
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
}
