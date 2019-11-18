using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractTile
{
    protected List<AbstractTile> _neighbors;

    public enum Position { Top, Bottom, Left, Right };
    protected Dictionary<Position, AbstractTile> _neighbors2;

    public int X { get; }
    public int Y { get; }
    public bool IsVisited { get; set; }
    public int NumNeighbors { get => _neighbors.Count; }

    public AbstractTile(int x, int y)
    {
        X = x;
        Y = y;
        _neighbors = new List<AbstractTile>();
    }

    public void AddNeighbor(AbstractTile neighbor)
    {
        _neighbors.Add(neighbor);
    }

    public void AddNeighbor(Position pos, AbstractTile neighbor)
    {
        _neighbors2.Add(pos, neighbor);
    }

    public AbstractTile GetNeighborAt(int index)
    {
        return _neighbors[index];
    }

    public AbstractTile GetNeighborAt(Position pos)
    {
        AbstractTile tile = null;
        _neighbors2.TryGetValue(pos, out tile);
        return tile;
    }


    public abstract bool IsEmpty();
}
