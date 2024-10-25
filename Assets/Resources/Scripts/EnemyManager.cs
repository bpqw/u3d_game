using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Enemy;

public class EnemyManager : MonoBehaviour
{
    public EnemyPool enemyPool; // ���������
    public int enemyCount = 2; // ���ɵĵ�����Ŀ
    public Vector3 spawnAreaMin; // ���ɷ�Χ����С��
    public Vector3 spawnAreaMax; // ���ɷ�Χ������
    private List<GameObject> enemies = new List<GameObject>(); // �洢���е��˵��б�
    public float createEnemyTimer = 0;
    public float createEnemyTime = 2;

    void Start()
    {
        SpawnEnemies(); // ��Ϸ��ʼʱ���ɵ���
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
            GameObject enemy = enemyPool.GetEnemy(); // �Ӷ���ػ�ȡ����
            Vector3 spawnPosition = GetValidNavMeshPosition(enemy); // ��ȡ��Ч����λ��
            Debug.Log(spawnPosition);
            enemy.transform.position = spawnPosition; // ��������λ��
            enemies.Add(enemy); // �����ɵĵ�����ӵ��б���
        }
        Debug.Log("Total enemies: " + enemies.Count);
    }

    Vector3 GetValidNavMeshPosition(GameObject enemy)
    {
        // ������Ч�� NavMesh λ�õ��߼�
        Vector3 spawnPosition;
        int maxAttempts = 10; // ���Դ�������
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
                spawnPosition = hit.position; // ��Чλ��
            }
            else
            {
                spawnPosition = randomPosition; // �����
            }

            attempts++;
        } while (!IsValidSpawnPosition(enemy, spawnPosition) && attempts < maxAttempts);

        return spawnPosition;
    }

    bool IsValidSpawnPosition(GameObject enemy, Vector3 position)
    {
        // ��ȡ���˵Ĵ�С
        Vector3 enemySize = enemy.GetComponent<Renderer>().bounds.size;
        Vector3 boxSize = enemySize / 2; // `CheckBox` �����Ǻ��ӵ�һ���С
        Debug.Log(enemySize);
        // ����Ƿ����ص�����
        return !Physics.CheckBox(position, boxSize, Quaternion.identity);
    }

    public void EnemyDied(GameObject enemy)
    {
        enemies.Remove(enemy); // ���б����Ƴ������ĵ���
        enemyPool.ReturnEnemy(enemy); // �����˷��ص������
        Debug.Log("An enemy died. Remaining enemies: " + enemies.Count);
    }
}
