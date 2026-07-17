using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Enemy/EnemyBaseData")]
public class EnemyDataBase : ScriptableObject
{
    public List<EnemyData> EneBaseList = new List<EnemyData>();
}
