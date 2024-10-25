using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject Enemy; // 敌人预制体
    public int initialPoolSize = 5; // 初始池大小

    private List<GameObject> pool = new List<GameObject>(); // 对象池

    void Start()
    {
        // 初始化对象池
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject enemy = Instantiate(Enemy);
            enemy.SetActive(false); // 将敌人设置为不激活状态
            pool.Add(enemy); // 将敌人添加到池中
        }
       
    }

    public GameObject GetEnemy()
    {
        foreach (GameObject enemy in pool)
        {
            if (!enemy.activeInHierarchy) // 检查是否有未激活的敌人
            {
                enemy.SetActive(true); // 激活敌人
                return enemy;
            }
        }

        // 如果没有可用的敌人，创建一个新的
        GameObject newEnemy = Instantiate(Enemy);
        pool.Add(newEnemy); // 添加到池中
        return newEnemy;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        // 将敌人重置，并返回到对象池
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.ResetEnemy(Vector3.zero); // 传入生成位置
        }

        enemy.SetActive(false); // 将敌人设置为不激活状态
    }
}
