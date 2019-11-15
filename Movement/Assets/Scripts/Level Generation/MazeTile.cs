using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeTile
{

    #region Public
    public enum AttributeType { Wall, Floor, Edge };
    public HashSet<AttributeType> _attributes;
    public readonly Vector2Int position;
    #endregion

    /// <summary>
    /// Creates a new MazeTile object with a set position.
    /// </summary>
    /// <param name="x">x-coordinate value.</param>
    /// <param name="y">y-coordinate value.</param>
    public MazeTile(int x, int y, HashSet<AttributeType> attributes)
    {
        position = new Vector2Int(x, y);
        _attributes = attributes;
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
            else // this is a wall
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

    }

}
