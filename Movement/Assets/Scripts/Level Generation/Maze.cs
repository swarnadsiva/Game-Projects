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

        // initialize 
        _tiles = new AbstractTile[myWidth, myHeight];
        InitializeTiles();

        // make floor path & mesh section and add to list
        int startX = _borderThickness;
        int startY = _borderThickness;
        GenerateFloorPath((FloorTile)_tiles[startX, startY], _floor);
        MeshSections.Add(_floor);

        // make border wall path and add to list
        GeneratePath<WallTile>(_tiles[0, 0], _border);
        MeshSections.Add(_border);


        //MeshSections.Add(_internalWalls);
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
                    FloorTile floorNeighbor;

                    // connect floor tile neighbors that are 1 over 
                    // so we can traverse later to find a path
                    if (x >= _borderThickness && _tiles[x - 2, y] is FloorTile)
                    {
                        floorNeighbor = (FloorTile)_tiles[x - 2, y];
                        // connect to x - 2
                        newFloor.AddFloorNeighbor(AbstractTile.Position.Left, floorNeighbor);
                        // connect x - 2 to this
                        floorNeighbor.AddFloorNeighbor(AbstractTile.Position.Right, newFloor);
                    }
                    if (y >= _borderThickness && _tiles[x, y - 2] is FloorTile)
                    {
                        floorNeighbor = (FloorTile)_tiles[x, y - 2];
                        // connect to y - 2
                        newFloor.AddFloorNeighbor(AbstractTile.Position.Top, floorNeighbor);
                        // connect y - 2 to this
                        floorNeighbor.AddFloorNeighbor(AbstractTile.Position.Bottom, newFloor);
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


        //// set neighbors
        //// TODO put this in the first loop iteration so we only loop once
        //for (int x = 0; x < myWidth; x++)
        //{
        //    for (int y = 0; y < myHeight; y++)
        //    {
        //        _tiles[x, y].SetNeighborsFromMaze(this);
        //    }
        //}

    }

    public AbstractTile GetTileAt(int x, int y)
    {
        return _tiles[x, y];
    }

    public void SetTileAt(int x, int y, AbstractTile newTile)
    {
        _tiles[x, y] = newTile;
    }

    public void GenerateFloorPath(FloorTile root, MeshSection myMesh)
    {
        // traverse with the root, be sure to pass in the maze!!
        root.DepthFirstTraversal<FloorTile>(myMesh, this);
    }

    public void GeneratePath<T>(AbstractTile root, MeshSection myMesh)
    {
        root.DepthFirstTraversal<T>(myMesh);
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
