using UnityEngine;

public class InputManager : Manager {

    private void Start()
    {
        //PlayableCharacterUtilities.Setup(); // TODO move this somewhere more relevant
    }

    // Update is called once per frame
    void Update () {
        //// possess next playable character
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    GameObject currentPlayer = PlayableCharacterUtilities.GetCurrentPlayer();
        //    GameObject nextPlayer = PlayableCharacterUtilities.GetNextPlayable();
        //    ICommand command = new DepossessCommand();
        //    command.Execute(currentPlayer);

        //    command = new PossessCommand();
        //    command.Execute(nextPlayer);
        //}
	}
}
