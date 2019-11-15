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
    #endregion

    #region Private
    private MazeTile[,] _tiles;
    private HashSet<Vector2Int> _floorTiles;
    private HashSet<Vector2Int> _wallTiles; 
    private int _wallProbability;
    #endregion

    /// <summary>
    /// Creates a new Maze object of the specified size.
    /// </summary>
    /// <param name="mSize"></param>
    public Maze(int mSize, int tSize, int wallProbability = 3)
    {

        _tiles = new MazeTile[mSize, mSize];
        _wallProbability = wallProbability;
        _floorTiles = new HashSet<Vector2Int>();
        _wallTiles = new HashSet<Vector2Int>();

        tileSize = tSize;
        mazeSize = mSize;
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

        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                HashSet<MazeTile.AttributeType> attributes = new HashSet<MazeTile.AttributeType>();

                // check if edge 
                if (x == 0 || x == mazeSize - 1 || y == 0 || y == mazeSize - 1)
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

                newTile = new MazeTile(x, y, attributes);
                _tiles[x,y] = newTile;
            }
        }
    }

    private void MakeReachable()
    {
        List<Vector2Int> surroundingPos = new List<Vector2Int>();
        List<MazeTile> surroundingTiles = new List<MazeTile>();
        HashSet<Vector2Int> traversedTilePos = new HashSet<Vector2Int>();

        MazeTile current;
        MazeTile nearestWall;

        // iterate through each MazeTile
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                current = _tiles[x, y];

                // only make floor tiles reachable
                if (current._attributes.Contains(MazeTile.AttributeType.Floor))
                {
                    // add current to traversed (and to floorTiles)
                    traversedTilePos.Add(current.position);
                    _floorTiles.Add(current.position);

                    // get surrounding tiles
                    surroundingPos = current.GetUntraversedSurroundingPositions(mazeSize, traversedTilePos);
                    surroundingTiles.Clear();

                    foreach (Vector2Int pos in surroundingPos)
                    {
                        surroundingTiles.Add(_tiles[pos.x, pos.y]);
                    }

                    Vector2Int nearestWallPos;
                    if (!current.IsReachable(surroundingTiles, out nearestWallPos))
                    {
                        // change nearest wall to floor
                        nearestWall = _tiles[nearestWallPos.x, nearestWallPos.y];
                        nearestWall.ChangeToFloor();

                        // remove from wall tiles
                        _wallTiles.Remove(nearestWall.position);
                    }
                } else if (current._attributes.Contains(MazeTile.AttributeType.Wall))
                {
                    // add to wall tiles
                    _wallTiles.Add(current.position);
                }
            }
        }
    }

    public HashSet<Vector2Int> GetFloorTiles()
    {
        return _floorTiles;
    }

    public HashSet<Vector2Int> GetWallTiles()
    {
        return _wallTiles;
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        Debug.LogFormat("totalTiles {0}", totalTiles);
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                sb.Append(_tiles[x, y].ToString());
                sb.Append("  ");
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }

}
