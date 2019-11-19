using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : AbstractTile
{
    #region Private
    private Dictionary<Position, FloorTile> _floorNeighbors; // dictionary to keep track of neighboring floor tiles (these will be one step away)
    #endregion

    #region Public
    public int NumFloorNeighbors { get => _floorNeighbors.Count; }
    #endregion

    public FloorTile(int x, int y, bool visited = false) : base(x, y, visited)
    {
        _floorNeighbors = new Dictionary<Position, FloorTile>();
    }

    public void AddFloorNeighbor(Position pos, FloorTile neighbor)
    {
        _floorNeighbors.Add(pos, neighbor);
    }

    public FloorTile GetFloorNeighborAt(Position pos)
    {
        FloorTile tile = null;
        _floorNeighbors.TryGetValue(pos, out tile);
        return tile;
    }

    public override bool IsEmpty()
    {
        return false;
    }

    public override void DepthFirstTraversal<T>(MeshSection myMesh, Maze maze = null)
    {
        // pick random floor position
        int randPos = Random.Range((int)Position.Top, (int)Position.Right + 1);
        Position next = (Position)randPos;

        // set is visited to true
        _isVisited = true;

        // add tile to mesh
        myMesh.AddTile(this);

        for (int i = 0; i < 4; i++) // iterate through all positions
        {
            // get the next floor neighbor
            FloorTile floorNeighbor = GetFloorNeighborAt(next);
            if (floorNeighbor != null && !floorNeighbor.IsVisited)
            {
                // get tile in between to connect the neighboring floor mesh
                // determine which side neighbor to get
                int deltaX = floorNeighbor.X - X;
                int deltaY = floorNeighbor.Y - Y;
                (int x, int y) = GetImmediateNeighbor(deltaX, deltaY, this);

                // create a new visited floor tile at this position
                AbstractTile newFloor = new FloorTile(x, y, true);

                // add the tile to the maze
                maze.SetTileAt(x, y, newFloor);

                // set newFloor neighbors
                newFloor.SetNeighborsFromMaze(maze);

                // add the tile to the mesh
                myMesh.AddTile(newFloor);

                // call DFT with neighbor as current and pass in the maze
                floorNeighbor.DepthFirstTraversal<T>(myMesh, maze);
            }

            // TODO add random neighbor as part of the mesh to add variety

            // go to next neighbor 
            next = (Position)(((int)next + 1) % NumFloorNeighbors);
        }
    }

    private (int x, int y) GetImmediateNeighbor(int deltaX, int deltaY, FloorTile current)
    {
        AbstractTile immediateNeighbor;
        Position pos;

        if (deltaX > 0)
        {
            // get immediate neighbor at right
            pos = Position.Right;
        }
        else if (deltaX < 0)
        {
            // get immediate neighbor at left
            pos = Position.Left;
        }
        else if (deltaY > 0)
        {
            // get immediate neighbor at top
            pos = Position.Top;
        }
        else if (deltaY < 0)
        {
            // get immediate neighbor at bottom
            pos = Position.Bottom;
        }
        else
        {
            Utilities.LogException("Neighboring tile is not in a correct spot!");
            immediateNeighbor = null;
            return (-1, -1);
        }

        immediateNeighbor = GetNeighborAt(pos);
        return (immediateNeighbor.X, immediateNeighbor.Y);
    }

    public override string ToString()
    {
        return "<color=blue> o </color>";
    }
}
