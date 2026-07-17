using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Arrribute Compatibility", menuName = "Scriptable Objects/Arrribute Compatibility")]
public class Arrribute : ScriptableObject
{
    [System.Serializable]//インスペクターに表示できるようにする
    public class CompatibilityEntry
    {
        public AttributeType attaker;//攻撃する側の属性
        public AttributeType defender;//受ける側の属性
        public float multiplier;//ダメージ倍率
    }

    public List<CompatibilityEntry> entries;//複数の相性を入れれる箱

    public float GetMultiplier(AttributeType attaker, AttributeType defender)
    {
        foreach (var entry in entries)//リストの中からentryとして1つずつ取る
        {
            if (entry.attaker == attaker &&  entry.defender == defender)
            {
                return entry.multiplier;//倍率(0.5倍,2倍)
            }
               
        }
        return 1.0f;//当てはまらなかったら等倍
    }
}
