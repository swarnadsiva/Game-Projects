using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : AbstractTile
{
    #region Private
    private Dictionary<Position, FloorTile> _floorNeighbors; // dictionary to keep track of neighboring floor tiles (these will be one step away)
    private const int RANDOM_CHANCE = 5;
    private const int NUM_POSITIONS = 4;
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
        int randPos = Random.Range(0, NUM_POSITIONS);
        Position next = (Position)randPos;

        // set is visited to true
        _isVisited = true;

        // add tile to mesh
        myMesh.AddTile(this);

        for (int i = 0; i < NUM_POSITIONS; i++) // iterate through all positions
        {
            // get the next floor neighbor
            FloorTile floorNeighbor = GetFloorNeighborAt(next);
            if (floorNeighbor != null && !floorNeighbor.IsVisited)
            {
                // make a path to this floor tile

                // get tile in between to connect the neighboring floor mesh
                // determine which side neighbor to get
                int deltaX = floorNeighbor.X - X;
                int deltaY = floorNeighbor.Y - Y;
                AbstractTile neighbor = GetImmediateNeighbor(deltaX, deltaY);
                if (neighbor != null && neighbor.IsEmpty())
                {
                    // only create a new floor tile if there isn't already one
                    CreateNewFloorTile(neighbor.X, neighbor.Y, maze, myMesh);
                }

                // add random neighbor as part of the mesh to add variety
                if (Random.Range(0, RANDOM_CHANCE) == 1)
                {
                    // get a neighboring empty floor 
                    AddRandomFloorTile(maze, myMesh);
                }

                // call DFT with neighbor as current and pass in the maze
                floorNeighbor.DepthFirstTraversal<T>(myMesh, maze);
            }

            // go to next neighbor 
            int nextNum = ((int)next + 1) % NUM_POSITIONS;
            next = (Position)nextNum;
        }
    }

    private void CreateNewFloorTile(int x, int y, Maze maze, MeshSection myMesh)
    {
        // create a new visited floor tile at this position
        AbstractTile newFloor = new FloorTile(x, y, true);

        // set newFloor neighbors
        newFloor.SetNeighborsFromMaze(maze);

        // updte this new floor's neighbors 
        newFloor.UpdateNeighbors(maze.myWidth, maze.myHeight);

        // add the tile to the maze
        maze.SetTileAt(x, y, newFloor);

        // add the tile to the mesh
        myMesh.AddTile(newFloor);
    }

    private void AddRandomFloorTile(Maze maze, MeshSection myMesh)
    {
        Position p = Position.Top;
        bool done = false;
        while (!done && p <= Position.Right)
        {
            // get the next neighbor
            AbstractTile neighbor = GetNeighborAt(p);
            if (neighbor != null && neighbor.IsEmpty())
            {
                // call DFT with neighbor as current
                CreateNewFloorTile(neighbor.X, neighbor.Y, maze, myMesh);
                done = true;
            } else
            {
                p++;
            }
        }
    }

    private AbstractTile GetImmediateNeighbor(int deltaX, int deltaY)
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
            return null;
        }

        immediateNeighbor = GetNeighborAt(pos);
        if (immediateNeighbor is WallTile)
        {
            Debug.LogFormat("Immediate tile to {0} ({1}, {2}) is a wall tile!", pos.ToString(), X, Y);
        }
        return immediateNeighbor;
    }

    public override string ToString()
    {
        return "<color=blue> o </color>";
    }
}
