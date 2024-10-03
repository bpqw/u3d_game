using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Transform topPoint;      // 电梯顶部位置
    public Transform bottomPoint;   // 电梯底部位置
    public float moveSpeed = 2.0f;  // 电梯移动速度

    private bool goingUp = true;    // 电梯当前是否在移动
    private Vector3 targetPosition; // 电梯下一目标位置
    private bool flag = false;

    void Start()
    {
        // 电梯初始目标位置为顶部位置
        targetPosition = topPoint.position;
    }

    void Update()
    {
        // 移动电梯
        if (flag)
        {
            MoveElevator();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Cube") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            flag = true;
        }
    }
    void MoveElevator()
    {
        // 电梯移动到目标位置
        Vector3 target = goingUp ? topPoint.position : bottomPoint.position;

        // 平滑移动到目标位置
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        // 到达目标位置后改变方向
        if (transform.position == target)
        {
            goingUp = !goingUp;
            targetPosition = goingUp ? topPoint.position : bottomPoint.position;
        }
    }
}
