using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyDataBase EnemyDataBase;

    public void AddEnemyData(EnemyData ene)
    {
        EnemyDataBase.EneBaseList.Add(ene);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}