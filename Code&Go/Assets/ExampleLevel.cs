using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: Interfaz ILevel generico para todo nivel
/*
 * Condicion de complecion
 * OnLevelFinished(), ...
 * etc
 */

public class ExampleLevel : Level
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject goal;

    public override bool IsCompleted()
    {
        return Mathf.Abs(player.transform.position.x - goal.transform.position.x) <= Mathf.Epsilon;
    }

    public override void OnLevelCompleted()
    {
        Debug.Log("Level completed");
    }

    public override void OnLevelStarted()
    {
        Debug.Log("Level started");
    }
}
