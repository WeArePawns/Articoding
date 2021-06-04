using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : BoardObject
{
    // DO STUFF I GUESS
    private void Awake()
    {
        typeName = Hash.ToHash(Time.realtimeSinceStartup.ToString(), Random.Range(float.MinValue, float.MaxValue).ToString());
        argsNames = new string[0];
    }
}
