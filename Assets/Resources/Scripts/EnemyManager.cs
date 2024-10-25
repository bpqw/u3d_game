using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Enemy;

public class EnemyManager : MonoBehaviour
{
    public EnemyPool enemyPool; // 对象池引用
    public int enemyCount = 2; // 生成的敌人数目
    public Vector3 spawnAreaMin; // 生成范围的最小点
    public Vector3 spawnAreaMax; // 生成范围的最大点
    private List<GameObject> enemies = new List<GameObject>(); // 存储所有敌人的列表
    public float createEnemyTimer = 0;
    public float createEnemyTime = 2;

    void Start()
    {
        SpawnEnemies(); // 游戏开始时生成敌人
    }

    private void FixedUpdate()
    {
        if (enemies.Count < enemyCount)
        {
            createEnemyTimer += Time.deltaTime;

            if (createEnemyTimer > createEnemyTime)
            {
                SpawnEnemies();
            }
        }
        else
        {
            createEnemyTimer = 0;
        }
    }
    void SpawnEnemies()
    {
        while (enemies.Count < enemyCount)
        {
            GameObject enemy = enemyPool.GetEnemy(); // 从对象池获取敌人
            Vector3 spawnPosition = GetValidNavMeshPosition(enemy); // 获取有效生成位置
            Debug.Log(spawnPosition);
            enemy.transform.position = spawnPosition; // 设置生成位置
            enemies.Add(enemy); // 将生成的敌人添加到列表中
        }
        Debug.Log("Total enemies: " + enemies.Count);
    }

    Vector3 GetValidNavMeshPosition(GameObject enemy)
    {
        // 生成有效的 NavMesh 位置的逻辑
        Vector3 spawnPosition;
        int maxAttempts = 10; // 尝试次数限制
        int attempts = 0;
        do
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                spawnAreaMin.y,
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 2.0f, NavMesh.AllAreas))
            {
                spawnPosition = hit.position; // 有效位置
            }
            else
            {
                spawnPosition = randomPosition; // 随机点
            }

            attempts++;
        } while (!IsValidSpawnPosition(enemy, spawnPosition) && attempts < maxAttempts);

        return spawnPosition;
    }

    bool IsValidSpawnPosition(GameObject enemy, Vector3 position)
    {
        // 获取敌人的大小
        Vector3 enemySize = enemy.GetComponent<Renderer>().bounds.size;
        Vector3 boxSize = enemySize / 2; // `CheckBox` 参数是盒子的一半大小
        Debug.Log(enemySize);
        // 检查是否有重叠物体
        return !Physics.CheckBox(position, boxSize, Quaternion.identity);
    }

    public void EnemyDied(GameObject enemy)
    {
        enemies.Remove(enemy); // 从列表中移除死亡的敌人
        enemyPool.ReturnEnemy(enemy); // 将敌人返回到对象池
        Debug.Log("An enemy died. Remaining enemies: " + enemies.Count);
    }
}
