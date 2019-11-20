using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyMovement : AIMovement
{
    
    // Start is called before the first frame update
    protected override void Start()
    {
        useAIMovement = true;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
