using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Maze
{

    #region Public
    public readonly int mazeSize = 0;
    public readonly int tileSize = 0;
    public readonly int totalTiles = 0;
    public readonly int wallHeight = 0;
    #endregion

    #region Private
    private MazeTile[,] _tiles;
    private HashSet<MazeTile> _floorTiles;
    private HashSet<MazeTile> _wallTiles; 
    private int _wallProbability;
    #endregion

    /// <summary>
    /// Creates a new Maze object of the specified size.
    /// </summary>
    /// <param name="mSize"></param>
    public Maze(int mSize, int tSize, int wHeight, int wallProbability = 3)
    {

        _tiles = new MazeTile[mSize, mSize];
        _wallProbability = wallProbability;
        _floorTiles = new HashSet<MazeTile>();
        _wallTiles = new HashSet<MazeTile>();

        tileSize = tSize;
        mazeSize = mSize;
        wallHeight = wHeight;
        totalTiles = mSize * mSize;
        Generate();
    }

    public void Generate()
    {
        // ensure these are clear
        _floorTiles.Clear();
        _wallTiles.Clear();

        // make random tiles
        Randomize();

        // ensure tiles are reachable
        MakeReachable();

        
    }

    private void Randomize()
    {
        MazeTile newTile;
        int rand;

        // we are looking at our maze top down, z-axis is vertical, x-axis is horizonatal
        for (int x = 0; x < mazeSize; x++)
        {
            for (int z = 0; z < mazeSize; z++)
            {
                HashSet<MazeTile.AttributeType> attributes = new HashSet<MazeTile.AttributeType>();

                // check if edge 
                if (x == 0 || x == mazeSize - 1 || z == 0 || z == mazeSize - 1)
                {
                    attributes.Add(MazeTile.AttributeType.Edge);
                }

                rand = Random.Range(0, _wallProbability);
                if (rand == 0) // 1 out of _wallProbability to be a wall
                {
                    // make this tile a wall
                    attributes.Add(MazeTile.AttributeType.Wall);
                } else
                {
                    // make this a floor
                    attributes.Add(MazeTile.AttributeType.Floor);
                }

                newTile = new MazeTile(x, z, attributes, tileSize, wallHeight);
                _tiles[x , z] = newTile;
            }
        }
    }

    private void MakeReachable()
    {
        List<Vector2Int> surroundingPos = new List<Vector2Int>();
        List<MazeTile> surroundingTiles = new List<MazeTile>();
        HashSet<Vector2Int> traversedTilePos = new HashSet<Vector2Int>();

        MazeTile current;
        //MazeTile nearestWall;
        Vector2Int nearestWallPos;

        // iterate through each MazeTile, z-axis is vertical, x-axis is horizonatal
        for (int x = 0; x < mazeSize; x++)
        {
            for (int z = 0; z < mazeSize; z++)
            {
                current = _tiles[x, z];

                // only make floor tiles reachable
                if (current._attributes.Contains(MazeTile.AttributeType.Floor))
                {
                    // add current to traversed (and to floorTiles)
                    traversedTilePos.Add(current.position);
                    _floorTiles.Add(current);

                    // get surrounding tiles
                    surroundingPos = current.GetUntraversedSurroundingPositions(mazeSize, traversedTilePos);
                    surroundingTiles.Clear();

                    foreach (Vector2Int pos in surroundingPos)
                    {
                        surroundingTiles.Add(_tiles[pos.x, pos.y]);
                    }

                    
                    if (!current.IsReachable(surroundingTiles, out nearestWallPos))
                    {
                        // change nearest wall to floor
                        //nearestWall = _tiles[nearestWallPos.x, nearestWallPos.y];
                        _tiles[nearestWallPos.x, nearestWallPos.y].ChangeToFloor();

                        // remove from wall tiles
                        _wallTiles.Remove(_tiles[nearestWallPos.x, nearestWallPos.y]);
                        _floorTiles.Add(_tiles[nearestWallPos.x, nearestWallPos.y]);
                    }
                } else if (current._attributes.Contains(MazeTile.AttributeType.Wall))
                {
                    // add to wall tiles
                    _wallTiles.Add(current);
                }
            }
        }
    }

    public HashSet<MazeTile> GetFloorTiles()
    {
        return _floorTiles;
    }

    public HashSet<MazeTile> GetWallTiles()
    {
        return _wallTiles;
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        Debug.LogFormat("totalTiles {0}", totalTiles);
        // ensure the maze is as it appears with z-axis vertical, x-axis horizontal
        // when looking top down and axis starts on bottom left
        for (int x = 0; x < mazeSize; x++)
        {
            for (int z = 0; z < mazeSize; z++)
            {
                sb.Append(_tiles[x, z].ToString());
                sb.Append("  ");
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }

    public List<MazeTile[]> GetTileSections(List<MazeTile> allTiles)
    {
        List<MazeTile[]> tileSections = new List<MazeTile[]>();
        List<MazeTile> remainingTiles = new List<MazeTile>();

        // copy all tiles to remaining tiles
        foreach (MazeTile tile in allTiles)
        {
            remainingTiles.Add(tile); // deep copy
        }

       
        while (remainingTiles.Count > 0)
        {
            // create a hashset for this section
            HashSet<MazeTile> section = new HashSet<MazeTile>();

            // start with first tile
            MazeTile tile = remainingTiles[0];
            section.Add(tile);

            tile.FindNeighbors(section, remainingTiles);
        }

        return tileSections;
    }

}
