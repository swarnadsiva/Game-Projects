using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static void LogException(string msg)
    {
        Debug.LogException(new System.Exception(msg));
    }
}
