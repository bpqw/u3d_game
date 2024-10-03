using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Transform topPoint;      // ���ݶ���λ��
    public Transform bottomPoint;   // ���ݵײ�λ��
    private float moveSpeed = 3.5f;  // �����ƶ��ٶ�
    private bool goingLeft = true;    // ���ݵ�ǰ�Ƿ��������ƶ�
    private Vector3 targetPosition; // ��һĿ��λ
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = goingLeft ? topPoint.position : bottomPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveAtkWall();
    }
    void MoveAtkWall()
    {

        // ƽ���ƶ�������Ŀ��λ��
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // ����Ŀ��λ�ú��л�����
        if (transform.position == targetPosition)
        {
            goingLeft = !goingLeft;
            targetPosition = goingLeft ? topPoint.position : bottomPoint.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            LevelManager.Instance.Loss();
        }
    }
}
