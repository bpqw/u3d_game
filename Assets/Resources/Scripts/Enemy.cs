using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour, IDamageable
{
    NavMeshAgent agent; // 导航代理
    Animator animator;
    Transform player; // 玩家的Transform对象
    public LayerMask playerLayer; // 碰撞检测所需的LayerMask
    [Header("敌人的生命值")]
    public float health;

    [Space]
    [Header("巡逻相关")]
    [Header("巡逻范围")]

    public float walkPointRange;
    bool walkPointSet;//是否设置了巡逻点
    Vector3 walkPoint;//巡逻点
    bool isWalking;//是否正在移动
    bool isRunning;//是否正在奔跑
    bool isDamage;//是否处于被攻击后的僵直状态
    bool Die = false;

    [Space]
    [Header("攻击相关")]
    [Header("攻击间隔时间")]
    public float timeBetweenAttacks;
    [Header("每次攻击时间")]
    public float timeAttack;
    [Header("攻击力")]
    public float attackDamage;
    [Header("攻击所使用的物体(子弹)")]
    public GameObject projectile;
    bool isAttack;
    bool alreadyAttacked;//是否已经攻击过

    [Space]
    [Header("状态相关")]
    [Header("是否是近战攻击")]
    public bool isMeleeAttack;
    [Header("远程攻击点")]
    public Transform remoteAttackPoint;
    [Header("视野范围和攻击范围")]
    public float sightRange;
    public float attackRange;
    //玩家是否在视野范围和攻击范围内
    public bool playerInSightRange;
    public bool playerInAttackRange;
    [Header("速度")]
    public float walkSpeed = 1.2f;
    public float runSpeed = 3f;
    bool isIdle;
    [Header("受击僵直时长")]
    public float timeDamage;
    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform; // 查找到名为"PlayerObj"的游戏物体，并获取其Transform组件
        agent = GetComponent<NavMeshAgent>(); // 获取自身的导航代理组件
        animator = GetComponent<Animator>();
        Debug.Log(animator);
    }

    private void Update()
    {
        // 检查玩家是否在视野范围或攻击范围内
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        // 根据玩家位置和状态进行相应的行为
        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (alreadyAttacked)
        {
            return;
        }

        if (!walkPointSet) SearchWalkPoint(); // 如果没有设置巡逻点，则搜索巡逻点

        if (walkPointSet)
        {
            agent.speed = walkSpeed; // 将导航代理的速度设置为5
            //TODO设置巡逻速度，播放走路动画
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
            agent.SetDestination(walkPoint); // 设置巡逻点为目标点
            if (!agent.hasPath) 
            { 
                walkPointSet = false;
            }
        }


        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // 已到达巡逻点
        if (distanceToWalkPoint.magnitude < 1f)
        {
            //TODO:播放待机动画,休息3-5秒，也可以等待机动画执行完再进行下一步
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);

            if (!isIdle)
            {
                Invoke(nameof(SetWalkPointSet), Random.Range(3, 6));
            }

            isIdle = true;
        }
    }

    void SetWalkPointSet()
    {
        isIdle = false;
        walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // 在指定范围内随机生成一个巡逻点
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        // SamplePosition表示判断该点是不是导航允许到达的地方
        NavMeshHit hit;
        if (NavMesh.SamplePosition(walkPoint, out hit, walkPointRange, 1))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        //判断是否在攻击
        if (isAttack) return;

        if (isDamage)
        {
            return;
        }

        // 创建一个路径请求
        NavMeshPath path = new NavMeshPath();

        // 计算从敌人到玩家的路径
        if (NavMesh.CalculatePath(transform.position, player.position, NavMesh.AllAreas, path))
        {
            // 如果存在路径，设置追击速度，播放奔跑动画
            agent.speed = runSpeed; // 将导航代理的速度设置为5
            animator.SetBool("Walk", false);
            animator.SetBool("Run", true);
            agent.SetDestination(player.position); // 设置导航代理的目标点为玩家位置
        }
        else
        {
            // 如果不存在路径，敌人将不会追击玩家
            Patroling();
        }
    }

    private void AttackPlayer()
    {
        if (isDamage || Die)
        {
            return;
        }

        // 已到达目标点附近
        if ((transform.position - player.position).magnitude < 1.5f)
        {
            // 停止移动
            agent.SetDestination(transform.position);
        }
      
        if (!alreadyAttacked)
        {
            //面向玩家
            transform.LookAt(player);

            //是否是近战攻击
            if (isMeleeAttack)
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);
                //TODO播放近战攻击动画
                animator.SetTrigger("Attack");
                Invoke(nameof(Attack), 1.0f);
            }
            else
            {
                ///攻击代码
                Rigidbody rb = Instantiate(projectile, remoteAttackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                rb.AddForce(transform.up * 8f, ForceMode.Impulse);
                ///攻击代码结束
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);

            //攻击时间
            isAttack = true;
            animator.SetBool("isAttack", true);
            Invoke(nameof(ResetIsAttack), timeAttack);
        }
    }
    private void Attack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, playerLayer);

        foreach (var hitCollider in hitColliders)
        {
            // Check if object is in front of player using dot product
            Vector3 directionToTarget = (hitCollider.transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToTarget);

            // Only damage enemies in front of player (dot product > 0.5 means within ~90 degrees)
            if (dotProduct > 0.5f)
            {
                // Try to get IDamageable interface
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                }
            }
        }
    }
    private void ResetIsAttack()
    {
        isAttack = false;
        animator.SetBool("isAttack", false);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    private void ResetDamage()
    {
        isDamage = false;
        animator.SetBool("isDamage", false);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // 在编辑器中绘制攻击范围的圆
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange); // 在编辑器中绘制视野范围的圆形
    }

    public void TakeDamage(float damage)
    {
        if (isDamage)
        {
            return;
        }

        health -= damage;
        Debug.Log("Enemy took " + damage + " damage, remaining health: " + health);

        if (health <= 0)
        {
            Die = true;
            animator.SetTrigger("Die");
            Invoke(nameof(DestroyEnemy), 2.0f);
            return;
        }
       
        isDamage = true;
        animator.SetTrigger("Damage");
        animator.SetBool("isDamage", true);
        Invoke(nameof(ResetDamage), timeDamage);
        // 如果生命值小于等于0，则摧毁自身
        //TODO: 播放死亡动画
        
    }
}
