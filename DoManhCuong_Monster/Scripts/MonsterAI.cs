using System.Collections;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public Transform player;                 // Transform của Player
    public Vector2 patrolTarget;             // Điểm tuần tra
    public float detectionRange = 5f;        // Phạm vi phát hiện Player
    public float attackRange = 1.5f;         // Phạm vi tấn công Player
    public float speed = 2f;                 // Tốc độ di chuyển
    public float idleTime = 2f;              // Thời gian chờ tại mỗi điểm

    private Vector2 startPosition;           // Vị trí ban đầu của quái
    private Animator animator;
    private bool isChasing = false;          // Quái đang đuổi theo Player
    private bool isAttacking = false;        // Quái đang tấn công
    private bool isDead = false;             // Quái đã chết chưa
    private Coroutine patrolRoutine;         // Coroutine tuần tra
    private Rigidbody2D rb;                  // Rigidbody2D của quái vật


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();    // Lấy Rigidbody2D của quái vật
        startPosition = transform.position; // Lưu vị trí ban đầu
        patrolRoutine = StartCoroutine(PatrolRoutine()); // Bắt đầu hành vi tuần tra
    }

    void Update()
    {
        if (isDead) 
        {
            return; 
        }
        // Không làm gì nếu quái đã chết

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                StopPatrol(); // Dừng tuần tra
                
                if (distanceToPlayer <= attackRange)
                {
                    StartAttack(); // Tấn công
                }
                else
                {
                    ChasePlayer(); // Đuổi theo Player
                }
                return;
            }
        }

        // Nếu Player ra khỏi tầm phát hiện, quay lại tuần tra
        if (isChasing || isAttacking)
        {
            ReturnToPatrol();
        }
    }

    private IEnumerator PatrolRoutine()
    {

        while (!isDead)
        {
            // Di chuyển đến điểm tuần tra
            yield return MoveToPosition(patrolTarget);

            // Chờ ở vị trí tuần tra
            SetAnimationState("Idle");
            yield return new WaitForSeconds(idleTime);

            // Quay lại vị trí ban đầu
            yield return MoveToPosition(startPosition);

            // Chờ ở vị trí ban đầu
            SetAnimationState("Idle");
            yield return new WaitForSeconds(idleTime);
        }
    }

    private IEnumerator MoveToPosition(Vector2 targetPosition)
    {
        SetAnimationState("Walk");

        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);


            // Quay mặt theo hướng di chuyển
            if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1); // Hướng phải
            else
                transform.localScale = new Vector3(-1, 1, 1); // Hướng trái

            yield return null;
        }

        rb.velocity = Vector2.zero; // Dừng quái vật tại vị trí mục tiêu
        transform.position = targetPosition; // Đảm bảo chính xác vị trí cuối
    }
    

    private void ChasePlayer()
    {
        isChasing = true;
        isAttacking = false;

        Vector2 direction = (player.position - transform.position).normalized;

        // Di chuyển chỉ theo trục X
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
        // Quay mặt về phía Player
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1); // Hướng phải
        else
            transform.localScale = new Vector3(-1, 1, 1); // Hướng trái

        SetAnimationState("Walk");
    }


    private void StartAttack()
    {
        isAttacking = true;
        isChasing = false;

        SetAnimationState("Attack");

        // Kết thúc tấn công sau 1 giây
        Invoke(nameof(EndAttack), 1f);
    }


    private void EndAttack()
    {
        isAttacking = false;
    }


    private void ReturnToPatrol()
    {
        isChasing = false;
        isAttacking = false;

        if (patrolRoutine == null)
        {
            patrolRoutine = StartCoroutine(PatrolRoutine());
        }
    }


    private void StopPatrol()
    {
        if (patrolRoutine != null)
        {
            StopCoroutine(patrolRoutine);
            patrolRoutine = null;
        }
    }

    private void SetAnimationState(string state)
    {
        if (animator == null) return;

        animator.SetBool("isIdle", state == "Idle");
        animator.SetBool("isWalking", state == "Walk");
        animator.SetBool("isAttacking", state == "Attack");
    }

    public void Die()
    {
        isChasing = false;
        isAttacking = false;
        isDead = true; // Đặt trạng thái chế
        StopPatrol();

        rb.velocity = Vector2.zero;
        rb.simulated = false;
        
        SetAnimationState("Dead");
        Destroy(gameObject);
    }
}
