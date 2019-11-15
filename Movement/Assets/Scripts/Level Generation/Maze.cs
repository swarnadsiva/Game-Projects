using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Maze
{

    #region Public
    public readonly int _mazeSize;
    public readonly int _totalTiles;
    public readonly List<MazeTile> floorTiles; // TODO figure out access level
    public readonly List<List<MazeTile>> wallTiles; // TODO figure out access level
    #endregion

    #region Private
    private MazeTile[,] tiles;
    

    private int _wallProbability;
    #endregion

    /// <summary>
    /// Creates a new Maze object of the specified size.
    /// </summary>
    /// <param name="size"></param>
    public Maze(int size, int wallProbability = 3)
    {

        tiles = new MazeTile[size, size];
        _mazeSize = size;
        _totalTiles = size * size;
        _wallProbability = wallProbability;

        Generate();
    }

    public void Generate()
    {
        // make random tiles
        Randomize();

        // ensure tiles are reachable
        MakeReachable();

        // TODO check if there are enclosed spaces
        CheckEnclosedSpaces();

        
    }

    private void Randomize()
    {
        MazeTile newTile;
        int rand;

        for (int x = 0; x < _mazeSize; x++)
        {
            for (int y = 0; y < _mazeSize; y++)
            {
                HashSet<MazeTile.AttributeType> attributes = new HashSet<MazeTile.AttributeType>();

                // check if edge 
                if (x == 0 || x == _mazeSize - 1 || y == 0 || y == _mazeSize - 1)
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
                tiles[x,y] = newTile;
            }
        }
    }

    private void MakeReachable()
    {
        List<Vector2Int> surroundingPos = new List<Vector2Int>();
        List<MazeTile> surroundingTiles = new List<MazeTile>();
        HashSet<Vector2Int> traversedTilePos = new HashSet<Vector2Int>();

        MazeTile current;

        // iterate through each MazeTile
        for (int x = 0; x < _mazeSize; x++)
        {
            for (int y = 0; y < _mazeSize; y++)
            {
                current = tiles[x, y];

                // only make floor tiles reachable
                if (current._attributes.Contains(MazeTile.AttributeType.Floor))
                {
                    // add current to traversed
                    traversedTilePos.Add(current.position);

                    // get surrounding tiles
                    surroundingPos = current.GetUntraversedSurroundingPositions(_mazeSize, traversedTilePos);
                    surroundingTiles.Clear();

                    foreach (Vector2Int pos in surroundingPos)
                    {
                        surroundingTiles.Add(tiles[pos.x, pos.y]);
                    }

                    Vector2Int nearestWallPos;
                    if (!current.IsReachable(surroundingTiles, out nearestWallPos))
                    {
                        // change nearest wall to floor
                        tiles[nearestWallPos.x, nearestWallPos.y].ChangeToFloor();
                    }
                }
            }
        }
    }

    private void CheckEnclosedSpaces()
    {

    }

    private List<MazeTile> GetFloorTiles()
    {
        return null;
    }

    private List<List<MazeTile>> GetWallTileSections()
    {
        return null;
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        Debug.LogFormat("totalTiles {0}", _totalTiles);
        for (int x = 0; x < _mazeSize; x++)
        {
            for (int y = 0; y < _mazeSize; y++)
            {
                sb.Append(tiles[x, y].ToString());
                sb.Append("  ");
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }

}
