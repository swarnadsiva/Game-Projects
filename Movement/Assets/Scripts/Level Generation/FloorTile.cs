using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : AbstractTile
{

    public FloorTile(int x, int y) : base(x, y)
    {

    }

    public override bool IsEmpty()
    {
        return false;
    }

    public override string ToString()
    {
        return "<color=blue> o </color>";
    }
}
