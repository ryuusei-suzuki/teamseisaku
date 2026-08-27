using UnityEngine;
using System.Collections.Generic;
public class BossPreviewData
{
    public static List<BossPlanEntry> bossPlan;
}

[System.Serializable]
public class BossPlanEntry
{
    public EnemyData data;
    public Element subElement;
}
