using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    NavMeshAgent agent; // ��������
    Animator animator;
    Transform player; // ��ҵ�Transform����
    public LayerMask whatIsPlayer; // ��ײ��������LayerMask
    [Header("���˵�����ֵ")]
    public float health;

    [Space]
    [Header("Ѳ�����")]
    [Header("Ѳ�߷�Χ")]

    public float walkPointRange;
    bool walkPointSet;//�Ƿ�������Ѳ�ߵ�
    Vector3 walkPoint;//Ѳ�ߵ�
    bool isWalking;//�Ƿ������ƶ�
    bool isRunning;//�Ƿ����ڱ���

    [Space]
    [Header("�������")]
    [Header("�������ʱ��")]
    public float timeBetweenAttacks;
    [Header("ÿ�ι���ʱ��")]
    public float timeAttack;
    [Header("������ʹ�õ�����(�ӵ�)")]
    public GameObject projectile;
    bool isAttack;
    bool alreadyAttacked;//�Ƿ��Ѿ�������

    [Space]
    [Header("״̬���")]
    [Header("�Ƿ��ǽ�ս����")]
    public bool isMeleeAttack;
    [Header("Զ�̹�����")]
    public Transform remoteAttackPoint;
    [Header("��Ұ��Χ�͹�����Χ")]
    public float sightRange;
    public float attackRange;
    //����Ƿ�����Ұ��Χ�͹�����Χ��
    public bool playerInSightRange;
    public bool playerInAttackRange;
    [Header("�ٶ�")]
    public float walkSpeed = 1.2f;
    public float runSpeed = 3f;
    bool isIdle;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform; // ���ҵ���Ϊ"PlayerObj"����Ϸ���壬����ȡ��Transform���
        agent = GetComponent<NavMeshAgent>(); // ��ȡ����ĵ����������
        animator = GetComponent<Animator>();
        Debug.Log(animator);
    }

    private void Update()
    {
        // �������Ƿ�����Ұ��Χ�򹥻���Χ��
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        // �������λ�ú�״̬������Ӧ����Ϊ
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

        if (!walkPointSet) SearchWalkPoint(); // ���û������Ѳ�ߵ㣬������Ѳ�ߵ�

        if (walkPointSet)
        {
            agent.speed = walkSpeed; // ������������ٶ�����Ϊ5
            //TODO����Ѳ���ٶȣ�������·����
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
            agent.SetDestination(walkPoint); // ����Ѳ�ߵ�ΪĿ���
        }


        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // �ѵ���Ѳ�ߵ�
        if (distanceToWalkPoint.magnitude < 1f)
        {
            //TODO:���Ŵ�������,��Ϣ3-5�룬Ҳ���Եȴ�������ִ�����ٽ�����һ��
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
        // ��ָ����Χ���������һ��Ѳ�ߵ�
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        // SamplePosition��ʾ�жϸõ��ǲ��ǵ���������ĵط�
        NavMeshHit hit;
        if (NavMesh.SamplePosition(walkPoint, out hit, walkPointRange, 1))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        //�ж��Ƿ��ڹ���
        if (isAttack) return;

        //TODO����׷���ٶȣ����ű��ܶ���
        agent.speed = runSpeed; // ������������ٶ�����Ϊ5
        animator.SetBool("Walk", false);
        animator.SetBool("Run", true);
        agent.SetDestination(player.position); // ���õ��������Ŀ���Ϊ���λ��
     
    }

    private void AttackPlayer()
    {
        // �ѵ���Ŀ��㸽��
        if ((transform.position - player.position).magnitude < 1.5f)
        {
            // ֹͣ�ƶ�
            agent.SetDestination(transform.position);
        }
      
        if (!alreadyAttacked)
        {
            //�������
            transform.LookAt(player);

            //�Ƿ��ǽ�ս����
            if (isMeleeAttack)
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);
                //TODO���Ž�ս��������
                animator.SetTrigger("Attack");
            }
            else
            {
                ///��������
                Rigidbody rb = Instantiate(projectile, remoteAttackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                rb.AddForce(transform.up * 8f, ForceMode.Impulse);
                ///�����������
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);

            //����ʱ��
            isAttack = true;
            animator.SetBool("isAttack", true);
            Invoke(nameof(ResetIsAttack), timeAttack);
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

    public void TakeDamage(int damage)
    {
        health -= damage;

        // �������ֵС�ڵ���0����ݻ�����
        //TODO: ������������
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // �ڱ༭���л��ƹ�����Χ��Բ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange); // �ڱ༭���л�����Ұ��Χ��Բ��
    }
}
