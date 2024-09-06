using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPoolData
{
    [Header("Enemy To Spawn")]
    public EnemyType enemyType;

    [Header("Start Spawning")]
    public float timeTillStartSpawning = 0;
    [Header("Stop Spawning")]
    public bool canStopSpawning = false;
    public float timeWhenEndSpawning = 99;

    [Header("Amount")]
    public int startingAmountEnemiesToSpawn = 10;
    public int maxEnemiesToSpawn;

    [Header("Level")]
    public bool spawnAtGlobalLevel;
    [HideInInspector]
    public int currentLevel;
}
