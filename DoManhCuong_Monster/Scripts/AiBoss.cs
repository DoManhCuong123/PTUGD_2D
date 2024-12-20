using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBoss : MonoBehaviour
{
    public Transform player;              // Transform của Player
    public float detectionRange = 10f;     // Phạm vi phát hiện Player
    public float attackRange = 2f;         // Phạm vi tấn công Player
    public float speed = 3f;               // Tốc độ di chuyển của Boss
    public float attackDuration = 1f;      // Thời gian tấn công
    public GameObject victoryText;        // Dòng chữ "Victory"

    private Animator animator;
    private bool isAttacking = false;
    private bool isDead = false;
    private Rigidbody2D rb;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Đảm bảo scale của boss được lớn hơn người chơi
        transform.localScale = new Vector3(5f, 5f, 1f);  // Tăng kích thước Boss

    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Nếu Player ở trong tầm phát hiện
        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange)
            {
                StartAttack(); // Nếu player gần, tấn công
            }
            else
            {
                ChasePlayer(); // Nếu player xa hơn, đuổi theo
            }
        }
        else
        {
            Idle(); // Nếu Player ra khỏi tầm phát hiện, Boss đứng yên
        }
    }

    private void ChasePlayer()
    {
        if (isAttacking) return; // Nếu đang tấn công, không đuổi theo

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

        // Quay mặt về phía Player
        if (direction.x > 0)
            transform.localScale = new Vector3(5f, 5f, 1f); // Hướng phải
        else
            transform.localScale = new Vector3(-5f, 5f, 1f); // Hướng trái

        SetAnimationState("Walk");
    }

    private void StartAttack()
    {
        if (isAttacking) return; // Nếu đang tấn công, không tấn công thêm lần nữa

        isAttacking = true;
        rb.velocity = Vector2.zero; // Dừng di chuyển khi tấn công

        SetAnimationState("Attack");

        // Kết thúc tấn công sau một khoảng thời gian
        Invoke(nameof(EndAttack), attackDuration);
    }
    private void EndAttack()
    {
        isAttacking = false;
        SetAnimationState("Idle");
    }

    // Boss đứng yên khi không có gì xảy ra
    private void Idle()
    {
        rb.velocity = Vector2.zero; // Dừng di chuyển
        SetAnimationState("Idle");
    }

    // Thiết lập trạng thái hoạt ảnh của Boss
    private void SetAnimationState(string state)
    {
        if (animator == null) return;

        animator.SetBool("isIdle", state == "Idle");
        animator.SetBool("isWalking", state == "Walk");
        animator.SetBool("isAttacking", state == "Attack");
    }

    // Hàm gọi khi Boss chết
    public void Die()
    {
        isDead = true;
        isAttacking = false; // Dừng tấn côn
        detectionRange = 0f;
        Debug.Log("Boss died. detectionRange is now: " + detectionRange); // Kiểm tra giá trị của detectionRange

        rb.velocity = Vector2.zero;
        rb.simulated = false;  // Tắt physics khi chết

        SetAnimationState("Dead");

        // Hiển thị dòng chữ Victory khi chết
        if (victoryText != null)
        {
            victoryText.SetActive(true);
        }

        Destroy(gameObject, 2f);  // Hủy Boss sau 2 giây
    }
}
