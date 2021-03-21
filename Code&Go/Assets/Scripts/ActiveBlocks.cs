using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CategoryBlocks
{
    //When false it deactivates the blocks instead of activating them
    public bool activate = true;
    public string[] activeBlocks;
}


[Serializable]
public class CategoryData
{
    public string categoryName;
    public CategoryBlocks blocksInfo;
}

[Serializable]
public class ActiveBlocks
{
    public CategoryData[] categories;
    public static ActiveBlocks FromJson(string text)
    {
        return JsonUtility.FromJson<ActiveBlocks>(text);
    }

    public Dictionary<string, CategoryBlocks> AsMap()
    {
        Dictionary<string, CategoryBlocks> map = new Dictionary<string, CategoryBlocks>();
        foreach (CategoryData c in categories)
            map[c.categoryName] = c.blocksInfo;

        return map;
    }
}
