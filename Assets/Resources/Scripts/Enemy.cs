using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public enum EnemyState
    {
        NormalState,
        FightingState,
        MovingState,
        RestingState
    }

    private NavMeshAgent enemyAgent;
    private EnemyState state = EnemyState.NormalState;
    private EnemyState childState = EnemyState.RestingState;

    public GameObject[] foodPrefabs;
    public float restTime = 2;
    private float restTimer = 0;

    public int HP = 100;
    public int maxHP = 100;

    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //初始状态下分为休息状态和巡逻状态
        if(state == EnemyState.NormalState)
        {
            //自动切换休息状态和巡逻状态
            if(childState == EnemyState.RestingState)
            {
                restTimer += Time.deltaTime;

                if(restTimer > restTime)
                {
                    Vector3 randomPositon = FindRandomPosition();
                    enemyAgent.SetDestination(randomPositon);
                    childState = EnemyState.MovingState;
                }
            }else if(childState == EnemyState.MovingState)
            {
                if(enemyAgent.remainingDistance <= 0)
                {
                    restTimer = 0;
                    childState = EnemyState.RestingState;  

                }
            }
        }
        //测试掉血
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
  

    }

    //生成二维随机方向
    Vector3 FindRandomPosition()
    {
        Vector3 randomDir = new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1, 1f));
        return transform.position + randomDir.normalized * Random .Range(2, 5);
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        Debug.Log("Enemy took damage. Current HP: " + HP);

        if (HP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (foodPrefabs.Length > 0)
        {
            // 随机选择一个食物预制体
            int randomIndex = Random.Range(0, foodPrefabs.Length);
            GameObject foodToDrop = foodPrefabs[randomIndex];

            // 在敌人所在位置生成食物
            Instantiate(foodToDrop, transform.position, Quaternion.identity);
        }
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>(); // 查找 EnemyManager
        if (enemyManager != null)
        {
            enemyManager.EnemyDied(this.gameObject); // 通知管理器该敌人已死亡
        }

    }

    public void ResetEnemy(Vector3 spawnPosition)
    {
        HP = maxHP; // 重置生命值
        transform.position = spawnPosition; // 重置位置到新生成点
        gameObject.SetActive(true); // 重新激活敌人
    }
}
