using UnityEngine;

public class DepossessCommand : ICommand
{
    public void Execute(GameObject gameObject)
    {
        // set object tag to playable
        gameObject.tag = "Playable";
    }
}
