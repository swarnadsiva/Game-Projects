using UnityEngine;

public class GameController : Manager {

    private bool gameSetup = false;
    private LevelGenerator levelGenerator;

    public GameObject player;

    protected override void Awake()
    {
        
        levelGenerator = FindObjectOfType<LevelGenerator>();
        base.Awake();
    }

    private void Update()
    {
        if (levelGenerator.Loaded && !gameSetup)
        {
            // get a player spawn point and instantiate player there
            Vector3 pos = levelGenerator.GetSpawnPoint();

            Instantiate(player, pos, Quaternion.identity);

            gameSetup = true;
        }
    }


}
