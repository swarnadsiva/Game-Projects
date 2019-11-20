using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : AbstractTile
{

    public WallTile(int x, int y) : base(x, y)
    {
    }

    public override bool IsEmpty()
    {
        return false;
    }

    public override string ToString()
    {
        return " x ";
    }
}
