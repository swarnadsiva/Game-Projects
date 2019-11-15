using UnityEngine;

public class PossessCommand : ICommand
{
    public void Execute(GameObject gameObject)
    {
        // possess the specified gameobject
        // all we are doing is changing the tag here
        gameObject.tag = "Player";
    }
}
