using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractTile
{

    public enum Position { Top, Bottom, Left, Right };
    public int X { get; }
    public int Y { get; }
    public bool IsVisited { get => _isVisited; }
    public int NumNeighbors { get => _neighbors.Count; }

    protected Dictionary<Position, AbstractTile> _neighbors;
    protected bool _isVisited;


    public AbstractTile(int x, int y, bool visited = false)
    {
        X = x;
        Y = y;
        _isVisited = visited;
        _neighbors = new Dictionary<Position, AbstractTile>();
    }

    public void AddNeighbor(Position pos, AbstractTile neighbor)
    {
        if (_neighbors.ContainsKey(pos))
        {
            _neighbors[pos] = neighbor;
        } else
        {
            _neighbors.Add(pos, neighbor);
        }
    }

    public void SetNeighborsFromMaze(Maze maze)
    {
        if (X > 0)
        {
            AddNeighbor(Position.Left, maze.GetTileAt(X - 1, Y));
        }

        if (X < maze.myWidth - 1)
        {
            AddNeighbor(Position.Right, maze.GetTileAt(X + 1, Y));
        }

        if (Y > 0)
        {
            AddNeighbor(Position.Top, maze.GetTileAt(X, Y - 1));
        }

        if (Y < maze.myHeight - 1)
        {
            AddNeighbor(Position.Bottom, maze.GetTileAt(X, Y + 1));
        }
    }

    public AbstractTile GetNeighborAt(Position pos)
    {
        AbstractTile neighbor = null;
        _neighbors.TryGetValue(pos, out neighbor);
        return neighbor;
    }

    public virtual void DepthFirstTraversal<T>(MeshSection myMesh, Maze maze = null)
    {
        // mark visited true
        _isVisited = true;

        // add the current tile to the mesh
        myMesh.AddTile(this);
        for (Position p = Position.Top; p <= Position.Right; p++)
        {
            // get the next neighbor of the same type
            AbstractTile neighbor = GetNeighborAt(p);
            if (neighbor != null && !neighbor.IsVisited && neighbor is T)
            {
                // call DFT with neighbor as current
                neighbor.DepthFirstTraversal<T>(myMesh);
            }
        }
    }

    public abstract bool IsEmpty();
}
