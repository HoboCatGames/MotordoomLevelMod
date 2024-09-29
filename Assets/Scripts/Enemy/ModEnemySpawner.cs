using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModEnemySpawner : MonoBehaviour
{
    public EnemySpawnerSettings enemySpawnSettings_Career;
    public EnemySpawnerSettings enemySpawnSettings_Easy;
    public EnemySpawnerSettings enemySpawnSettings_Medium;
    public EnemySpawnerSettings enemySpawnSettings_Hard;

    public Transform[] bossSpawnLocations;
}
