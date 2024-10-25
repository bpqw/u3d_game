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
        //��ʼ״̬�·�Ϊ��Ϣ״̬��Ѳ��״̬
        if(state == EnemyState.NormalState)
        {
            //�Զ��л���Ϣ״̬��Ѳ��״̬
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
        //���Ե�Ѫ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
  

    }

    //���ɶ�ά�������
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
            // ���ѡ��һ��ʳ��Ԥ����
            int randomIndex = Random.Range(0, foodPrefabs.Length);
            GameObject foodToDrop = foodPrefabs[randomIndex];

            // �ڵ�������λ������ʳ��
            Instantiate(foodToDrop, transform.position, Quaternion.identity);
        }
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>(); // ���� EnemyManager
        if (enemyManager != null)
        {
            enemyManager.EnemyDied(this.gameObject); // ֪ͨ�������õ���������
        }

    }

    public void ResetEnemy(Vector3 spawnPosition)
    {
        HP = maxHP; // ��������ֵ
        transform.position = spawnPosition; // ����λ�õ������ɵ�
        gameObject.SetActive(true); // ���¼������
    }
}
