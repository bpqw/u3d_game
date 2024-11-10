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

    public List<Vector3> skeletonSoldierPosition = new List<Vector3>();
   
    void Start()
    {
        skeletonSoldierPosition.Add(new Vector3(0f, 0.0f, 0f));
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
            int randomInt = Random.Range(0, 5);
            GameObject enemy = enemyPool.GetEnemy(); // 从对象池获取敌人
            Vector3 spawnPosition = skeletonSoldierPosition[0]; // 获取有效生成位置
            Debug.Log(spawnPosition);
            enemy.transform.position = spawnPosition; // 设置生成位置
            enemies.Add(enemy); // 将生成的敌人添加到列表中
        }
        Debug.Log("Total enemies: " + enemies.Count);
    }

    //根据范围随机生成enemy的出生位置
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


    //检测范围enemy出生坐标是否合法
    bool IsValidSpawnPosition(GameObject enemy, Vector3 position)
    {
        return true;
    }

    public void EnemyDied(GameObject enemy)
    {
        enemies.Remove(enemy); // 从列表中移除死亡的敌人
        enemyPool.ReturnEnemy(enemy); // 将敌人返回到对象池
        Debug.Log("An enemy died. Remaining enemies: " + enemies.Count);
    }
}
