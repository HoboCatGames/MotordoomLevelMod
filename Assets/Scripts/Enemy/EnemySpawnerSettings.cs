using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/EnemySpawnSettings")]
public class EnemySpawnerSettings : ScriptableObject
{
    public bool spawnEnemies = true;
    [Header("Enemy Settings")]
    public float healthIncreasePercentage = 4;
    public float damageIncreasePercentage = 5;

    [Header("Enemies")]
    public EnemyPoolData[] enemyPools;

    [Header("Bosses")]
    public List<GameObject> bosses;
    public bool spawnRandomBoss = false;
    public BossType spawnThisBoss;

    [Header("Spawn Area")]
    [SerializeField] Vector2 minArea;
    [SerializeField] Vector2 maxArea;
    [SerializeField] float height;

    [Header("Spawn Time")]
    public float startingTimeTillNextSpawn = 0.1f;
    public float spawnTimeFactorPerLevel = 10;

    public Vector3 CreateSpawnPoint()
    {
        Vector3 randomPosition;
        randomPosition.x = Random.Range(minArea.x, maxArea.x);
        randomPosition.z = Random.Range(minArea.y, maxArea.y);
        randomPosition.y = height;
        return randomPosition;
    }
}
