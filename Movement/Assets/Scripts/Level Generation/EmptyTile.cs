using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyTile : AbstractTile
{

    public EmptyTile(int x, int y) : base(x, y)
    {

    }

    public override bool IsEmpty()
    {
        return true;
    }

    public override string ToString()
    {
        return "<color=grey> p </color>";
    }
}
