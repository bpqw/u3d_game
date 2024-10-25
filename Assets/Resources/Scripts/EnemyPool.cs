using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject Enemy; // ����Ԥ����
    public int initialPoolSize = 5; // ��ʼ�ش�С

    private List<GameObject> pool = new List<GameObject>(); // �����

    void Start()
    {
        // ��ʼ�������
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject enemy = Instantiate(Enemy);
            enemy.SetActive(false); // ����������Ϊ������״̬
            pool.Add(enemy); // ��������ӵ�����
        }
       
    }

    public GameObject GetEnemy()
    {
        foreach (GameObject enemy in pool)
        {
            if (!enemy.activeInHierarchy) // ����Ƿ���δ����ĵ���
            {
                enemy.SetActive(true); // �������
                return enemy;
            }
        }

        // ���û�п��õĵ��ˣ�����һ���µ�
        GameObject newEnemy = Instantiate(Enemy);
        pool.Add(newEnemy); // ��ӵ�����
        return newEnemy;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        // ���������ã������ص������
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.ResetEnemy(Vector3.zero); // ��������λ��
        }

        enemy.SetActive(false); // ����������Ϊ������״̬
    }
}
