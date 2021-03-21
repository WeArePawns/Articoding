using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActiveBlocks
{
    public string[] activeBlocks;
    public string[] activeCategories;

    public static ActiveBlocks FromJson(string text)
    {
        return JsonUtility.FromJson<ActiveBlocks>(text);
    }
}
