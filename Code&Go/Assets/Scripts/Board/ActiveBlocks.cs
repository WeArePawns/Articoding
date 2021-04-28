using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlockInfo
{
    public string blockName;
    public int maxUses = Int16.MaxValue;
}

[Serializable]
public class CategoryBlocksInfo
{
    //When false it deactivates the blocks instead of activating them
    public bool activate = true;
    public BlockInfo[] activeBlocks;
}

public class CategoryBlocks
{
    public bool activate = true;
    public Dictionary<string, int> activeBlocks;
}


[Serializable]
public class CategoryData
{
    public string categoryName;
    public CategoryBlocksInfo blocksInfo;
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
        foreach (CategoryData c in categories)//For each category
        {
            map[c.categoryName] = new CategoryBlocks();
            map[c.categoryName].activate = c.blocksInfo.activate;
            map[c.categoryName].activeBlocks = new Dictionary<string, int>();
            foreach (BlockInfo b in c.blocksInfo.activeBlocks)//For each block in a category
                map[c.categoryName].activeBlocks[b.blockName] = b.maxUses;
        }

        return map;
    }
}
